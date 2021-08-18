using Microsoft.Extensions.Options;
using NATS.Client;
using NATS.Client.JetStream;
using NBB.Messaging.Abstractions;
using NBB.Messaging.Nats.Internal;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static NATS.Client.JetStream.ConsumerConfiguration;
using static NATS.Client.JetStream.PushSubscribeOptions;

namespace NBB.Messaging.Nats
{
    public class JetstreamMessagingTransport : IMessagingTransport
    {
        private readonly JetstreamConnectionManager _jetstreamConnectionManager;
        private readonly IOptions<NatsOptions> _natsOptions;

        public JetstreamMessagingTransport(JetstreamConnectionManager stanConnectionManager, IOptions<NatsOptions> natsOptions)
        {
            _jetstreamConnectionManager = stanConnectionManager;
            _natsOptions = natsOptions;
        }

        public Task<IDisposable> SubscribeAsync(string topic, Func<byte[], Task> handler,
            SubscriptionTransportOptions options = null,
            CancellationToken cancellationToken = default)
        {
            PushSubscribeOptionsBuilder psoBuilder = PushSubscribeOptions.Builder();
                 //.WithStream(_natsOptions.Value.Stream).WithDeliverSubject("deliver-ack");

            //var opts = ConnectionFactory.GetDefaultOptions();
            var subscriberOptions = options ?? SubscriptionTransportOptions.Default;
            if (subscriberOptions.IsDurable)
            {
                psoBuilder.WithDurable(_natsOptions.Value.DurableName);
            }

            ConsumerConfigurationBuilder ccBuilder = ConsumerConfiguration.Builder()
                .WithAckWait(subscriberOptions.AckWait ?? _natsOptions.Value.AckWait ?? 50000)
                .WithMaxAckPending(subscriberOptions.MaxConcurrentMessages);

            if (!subscriberOptions.DeliverNewMessagesOnly)
            {
                ccBuilder.WithDeliverPolicy(DeliverPolicy.All);
            }

            // TODO: handle hardcoded value
            ccBuilder.WithFilterSubject("LSNG.LIVIU.NBB.Contracts.PublishedLanguage.*");

            psoBuilder.WithConfiguration(ccBuilder.Build());

            PushSubscribeOptions pso = psoBuilder.Build();

            // https://github.com/nats-io/stan.go#subscriber-rate-limiting
            //opts.MaxAckPending = subscriberOptions.MaxConcurrentMessages;
            //opts.AckWait = subscriberOptions.AckWait ?? _natsOptions.Value.AckWait ?? 50000;
            //opts.ManualAcks = true;

            //CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            async void JetstreamMsgHandler(object obj, MsgHandlerEventArgs args)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                Console.WriteLine(args.Message.Subject);
                //await handler(args.Message.Data);

                //if (args.Message.IsJetStream)
                //{
                //    args.Message.Ack();
                //}

                args.Message.Ack();
            }


            void StreamMustExist(IConnection c)
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();

                try
                {
                    jsm.GetStreamInfo(_natsOptions.Value.Stream); // this throws if the stream does not exist
                    return;
                }
                catch (NATSJetStreamException) { /* stream does not exist */ }

                StreamConfiguration sc = StreamConfiguration.Builder()
                    .WithName(_natsOptions.Value.Stream)
                    .WithStorageType(StorageType.File)
                    .WithSubjects("DEV.NBB.Contracts.PublishedLanguage.*")
                    .Build();
                jsm.AddStream(sc);


                try
                {
                    c.CreateJetStreamManagementContext().GetStreamInfo(_natsOptions.Value.Stream); // this throws if the stream does not exist
                }
                catch (NATSJetStreamException)
                {
                    throw new Exception("Stream Must Already Exist!");
                }
            }

            IDisposable subscription = null;
            int red = 0;
            int count = 0;

            _jetstreamConnectionManager.Execute(connection =>
            {
                //subscription = subscriberOptions.UseGroup
                //    ? stanConnection.Subscribe(topic, _natsOptions.Value.QGroup, opts, StanMsgHandler)
                //    : stanConnection.Subscribe(topic, opts, StanMsgHandler);

                StreamMustExist(connection);
                IJetStream js = connection.CreateJetStreamContext();

                subscription = subscriberOptions.UseGroup
                                               ? js.PushSubscribeAsync(topic, _natsOptions.Value.QGroup, JetstreamMsgHandler, false, pso) :
                                                 js.PushSubscribeAsync(topic, JetstreamMsgHandler, false, pso);

                connection.Flush(500);
            });

            return Task.FromResult(subscription);
        }

        private static Msg GetNextMessage(IJetStreamPushSyncSubscription sub)
        {
            try
            {
                return sub.NextMessage(500);
            }
            catch (NATSTimeoutException)
            {
                // probably no messages in stream
                return null;
            }
        }


        public Task PublishAsync(string topic, byte[] message, CancellationToken cancellationToken = default)
        {
            void CreateStreamWhenDoesNotExist(IConnection c)
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();

                try
                {
                    jsm.GetStreamInfo(_natsOptions.Value.Stream); // this throws if the stream does not exist
                    return;
                }
                catch (NATSJetStreamException) { /* stream does not exist */ }

                StreamConfiguration sc = StreamConfiguration.Builder()
                    .WithName(_natsOptions.Value.Stream)
                    .WithStorageType(StorageType.File)
                    .WithSubjects(topic)
                    .Build();
                jsm.AddStream(sc);
            }


            return _jetstreamConnectionManager.ExecuteAsync(connection =>
            {
                //CreateStreamWhenDoesNotExist(connection);
                IJetStream js = connection.CreateJetStreamContext();
                Msg msg = new(topic, null, new MsgHeader(), message);
                PublishAck pa = js.Publish(msg);
                return Task.CompletedTask;
            });
        }
    }
}