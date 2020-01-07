using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NBB.Messaging.DataContracts;
using NBB.Tenancy.Abstractions;
using NBB.Tenancy.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.Messaging.Abstractions
{
    public class MessageBusSubscriber<TMessage> : IMessageBusSubscriber<TMessage>
    {
        private readonly ITopicRegistry _topicRegistry;
        private readonly List<Func<MessagingEnvelope<TMessage>, Task>> _handlers
            = new List<Func<MessagingEnvelope<TMessage>, Task>>();
        private readonly IMessageSerDes _messageSerDes;
        private bool _subscribedToTopic;
        private readonly IMessagingTopicSubscriber _topicSubscriber;
        private readonly ILogger<MessageBusSubscriber<TMessage>> _logger;
        private IEnumerable<string> _topicNames;
        private MessagingSubscriberOptions _subscriberOptions;
        private readonly IConfiguration _configuration;
        private readonly ITenantConfig _tenantConfig;

        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public MessageBusSubscriber(ITopicRegistry topicRegistry, IMessageSerDes messageSerDes,
            IMessagingTopicSubscriber topicSubscriber, ILogger<MessageBusSubscriber<TMessage>> logger,
            IConfiguration configuration, ITenantConfig tenantConfig)
        {
            _topicRegistry = topicRegistry;
            _messageSerDes = messageSerDes;
            _topicSubscriber = topicSubscriber;
            _logger = logger;
            _configuration = configuration;
            _tenantConfig = tenantConfig;
        }

        public async Task SubscribeAsync(Func<MessagingEnvelope<TMessage>, Task> handler, CancellationToken cancellationToken = default, string topicName = null, MessagingSubscriberOptions options = null)
        {
            var subscriptions = new List<Task>();
            _handlers.Add(handler);

            if (!_subscribedToTopic)
            {
                //lock (this)
                await semaphoreSlim.WaitAsync();
                {
                    if (!_subscribedToTopic)
                    {
                        try
                        {
                            _subscribedToTopic = true;
                            //_topicName = _topicRegistry.GetTopicForName(topicName) ??
                            //             _topicRegistry.GetTopicForMessageType(typeof(TMessage));
                            // _topicNames = _topicRegistry.GetTopicsForMessageType(typeof(TMessage));

                            _subscriberOptions = options;
                            // foreach (var topic in _topicNames)

                            var workerType = _configuration.GetSection("Messaging")["WorkerType"];

                            if (workerType == WorkerTenancyType.Dedicated.ToString())
                            {
                                var dedicatedTopics = _tenantConfig.GetOneHundredTenants()
                                    .Where(id => _tenantConfig.GetTenantType(id) == TenantType.Dedicated).ToList();
                                for (int i = 0; i < dedicatedTopics.Count; i++)
                                {
                                    var newTopicName = _topicRegistry.GetTopicForTopicPrefix(typeof(TMessage), dedicatedTopics[i]);
                                    subscriptions.Add(_topicSubscriber.SubscribeAsync(newTopicName, HandleMessage, cancellationToken, options));
                                    //_topicSubscriber.SubscribeAsync(newTopicName, HandleMessage, cancellationToken, options);
                                }

                                await Task.WhenAll(subscriptions);
                            }

                            else if (workerType == WorkerTenancyType.Shared.ToString())
                            {
                                var newTopicName = _topicRegistry.GetTopicForTopicPrefix(typeof(TMessage), _topicRegistry.GetSharedTopicPrefix());
                                await _topicSubscriber.SubscribeAsync(newTopicName, HandleMessage, cancellationToken, options);
                            }

                            else if (workerType == WorkerTenancyType.Mono.ToString())
                            {
                                var tenantId = _configuration.GetSection("Messaging")["TenantPrefix"];
                                var tenantType = _tenantConfig.GetTenantType(tenantId);
                                if (tenantType == TenantType.Dedicated)
                                {
                                    var newTopicName = _topicRegistry.GetTopicForTopicPrefix(typeof(TMessage), tenantId);
                                    await _topicSubscriber.SubscribeAsync(newTopicName, HandleMessage, cancellationToken, options);
                                }
                                else if (tenantType == TenantType.Shared)
                                {
                                    throw new TenancyException($"The tenant {tenantId} is allowed to subscribe only to shared topics. " +
                                        $"It is trying to subscribe to a dedicated topic");
                                }
                            }


                            //if (tenantType == TenantType.Shared && workerType == WorkerTenancyType.Mono)
                            //{
                            //    throw new TenancyException($"The tenant {tenantId} is allowed to subscribe only to shared topics. " +
                            //        $"It is trying to subscribe to a dedicated topic");
                            //}

                            //if (tenantType == TenantType.Shared && workerType == WorkerTenancyType.Dedicated)
                            //{
                            //    throw new TenancyException($"The tenant {tenantId} is allowed to subscribe only to shared topics. " +
                            //       $"It is trying to subscribe to a dedicated topic");
                            //}

                            //if (tenantType == TenantType.Shared && workerType == WorkerTenancyType.Shared)
                            //{
                            //    var newTopicName = _topicRegistry.GetTopicForTopicPrefix(typeof(TMessage), _topicRegistry.GetSharedTopicPrefix());
                            //    _topicSubscriber.SubscribeAsync(newTopicName, HandleMessage, cancellationToken, options);
                            //}

                            //if (tenantType == TenantType.Dedicated && workerType == WorkerTenancyType.Mono)
                            //{
                            //    var newTopicName = _topicRegistry.GetTopicForTopicPrefix(typeof(TMessage), tenantId);
                            //    _topicSubscriber.SubscribeAsync(newTopicName, HandleMessage, cancellationToken, options);
                            //}

                            //if (tenantType == TenantType.Dedicated && workerType == WorkerTenancyType.Shared)
                            //{
                            //    throw new TenancyException($"The tenant {tenantId} is allowed to subscribe only to shared topics. " +
                            //      $"It is trying to subscribe to a dedicated topic");
                            //}


                            //if (tenantType == TenantType.Dedicated)
                            //{
                            //    var newTopicName = _topicRegistry.GetTopicForName(typeof(TMessage), tenantId);
                            //    _topicSubscriber.SubscribeAsync(newTopicName, HandleMessage, cancellationToken, options);
                            //}
                            //else if (tenantType == TenantType.Multi)
                            //{
                            //    //confugurare per worker
                            //    //Read multi ids from configuration file
                            //    //var ids =   ia toti tenantii din config si verifica daca sunt multi
                            //    var ids = _tenantStore.GetTenantIds().Where(id => _tenantStore.GetTenantType(id) == TenantType.Multi);
                            //    foreach (var id in ids)
                            //    {
                            //        var newTopicName = _topicRegistry.GetTopicForTopicPrefix(typeof(TMessage), id);
                            //        _topicSubscriber.SubscribeAsync(newTopicName, HandleMessage, cancellationToken, options);
                            //    }
                            //}
                            //else if (tenantType == TenantType.Shared)
                            //{
                            //    var newTopicName = _topicRegistry.GetTopicForTopicPrefix(typeof(TMessage), _topicRegistry.GetSharedTopicPrefix());
                            //    _topicSubscriber.SubscribeAsync(newTopicName, HandleMessage, cancellationToken, options);
                            //}

                            //var newTopicName = _topicRegistry.GetTopicForName(typeof(TMessage), tenantId);
                            //_topicSubscriber.SubscribeAsync(newTopicName, HandleMessage, cancellationToken, options);
                        }

                        finally
                        {
                            semaphoreSlim.Release();
                        }
                    }
                }
            }
        }

        async Task HandleMessage(string msg)
        {
            MessagingEnvelope<TMessage> deserializedMessage = null;
            try
            {
                deserializedMessage = _messageSerDes.DeserializeMessageEnvelope<TMessage>(msg, _subscriberOptions?.SerDes);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "MessageBusSubscriber encountered an error when deserializing a message from topic {TopicName}.\n {Error}",
                    _topicNames, ex);
                //TODO: push to DLQ
            }


            if (deserializedMessage != null)
            {
                foreach (var handler in _handlers.ToList())
                {
                    await handler(deserializedMessage);
                }
            }
        }

        public async Task UnSubscribeAsync(Func<MessagingEnvelope<TMessage>, Task> handler, CancellationToken cancellationToken = default)
        {
            _handlers.Remove(handler);
            if (_handlers.Count == 0)
                await _topicSubscriber.UnSubscribeAsync(cancellationToken);
        }
    }
}
