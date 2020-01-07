using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NBB.Core.Abstractions;
using NBB.Messaging.DataContracts;
using NBB.Tenancy.Abstractions;
using NBB.Tenancy.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.Messaging.Abstractions
{
    public class MessageBusPublisher : IMessageBusPublisher
    {
        private readonly ITopicRegistry _topicRegistry;
        private readonly IMessageSerDes _messageSerDes;
        private readonly IConfiguration _configuration;
        private readonly IMessagingTopicPublisher _topicPublisher;
        private readonly ILogger<MessageBusPublisher> _logger;
        private readonly ITenantConfig _tenantConfig;

        public MessageBusPublisher(IMessagingTopicPublisher topicPublisher, ITopicRegistry topicRegistry, IMessageSerDes messageSerDes,
            IConfiguration configuration, ILogger<MessageBusPublisher> logger, ITenantConfig tenantConfig)
        {
            _topicRegistry = topicRegistry;
            _messageSerDes = messageSerDes;
            _configuration = configuration;
            _topicPublisher = topicPublisher;
            _logger = logger;
            _tenantConfig = tenantConfig;
        }

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default, Action<MessagingEnvelope> envelopeCustomizer = null, string topicName = null)
        {
            var outgoingEnvelope = PrepareMessageEnvelope(message, envelopeCustomizer);
            var tenantId = outgoingEnvelope.Headers.TryGetValue("TenantId", out var val) ? val : null;
            var workerType = _configuration.GetSection("Messaging")["WorkerType"];
            var key = (message as IKeyProvider)?.Key;
            var value = _messageSerDes.SerializeMessageEnvelope(outgoingEnvelope);
            var tenantType = _tenantConfig.GetTenantType(tenantId);
            //var newTopicName = _topicRegistry.GetTopicForName(topicName) ??
            //                   _topicRegistry.GetTopicForMessageType(message.GetType());

            //var topics = _topicRegistry.GetTopicsForMessageType(message.GetType());
            //foreach (var topic in topics)
            //await _topicPublisher.PublishAsync(topics.ToArray()[0], key, value, cancellationToken);

            //var newTopicName = _topicRegistry.GetTopicForTopicPrefix(message.GetType(), tenantId);

            if (tenantType == TenantType.Shared && workerType == WorkerTenancyType.Mono.ToString())
            {
                throw new TenancyException($"The tenant {tenantId} is allowed to publish only to shared topics. " +
                    $"It is trying to publish to a dedicated topic");
            }

            else if (tenantType == TenantType.Shared && workerType == WorkerTenancyType.Dedicated.ToString())
            {
                throw new TenancyException($"The tenant {tenantId} is allowed to publish only to shared topics. " +
                     $"It is trying to publish to a dedicated topic");
            }

            else if (tenantType == TenantType.Shared && workerType == WorkerTenancyType.Shared.ToString())
            {
                var newTopicName = _topicRegistry.GetTopicForTopicPrefix(message.GetType(), _topicRegistry.GetSharedTopicPrefix());
                await _topicPublisher.PublishAsync(newTopicName, key, value, cancellationToken);
            }

            else if (tenantType == TenantType.Dedicated && workerType == WorkerTenancyType.Mono.ToString())
            {
                var newTopicName = _topicRegistry.GetTopicForTopicPrefix(message.GetType(), tenantId);
                await _topicPublisher.PublishAsync(newTopicName, key, value, cancellationToken);
            }

            else if (tenantType == TenantType.Dedicated && workerType == WorkerTenancyType.Dedicated.ToString())
            {
                var newTopicName = _topicRegistry.GetTopicForTopicPrefix(message.GetType(), tenantId);
                await _topicPublisher.PublishAsync(newTopicName, key, value, cancellationToken);
            }

            else if (tenantType == TenantType.Dedicated && workerType == WorkerTenancyType.Shared.ToString())
            {
                throw new TenancyException($"The tenant {tenantId} is allowed to publish only to dedicated topics. " +
                  $"It is trying to publish to a shared topic");
            }

            //{
            //    var newTopicName = _topicRegistry.GetTopicForTopicPrefix(message.GetType(), tenantId);
            //    await _topicPublisher.PublishAsync(newTopicName, key, value, cancellationToken);
            //}
            //else if (tenantType == TenantType.Shared)
            //{
            //    var newTopicName = _topicRegistry.GetTopicForTopicPrefix(message.GetType(), _topicRegistry.GetSharedTopicPrefix());
            //    await _topicPublisher.PublishAsync(newTopicName, key, value, cancellationToken);
            //}

            // await _topicPublisher.PublishAsync(newTopicName, key, value, cancellationToken);

            await Task.Yield();
        }

        private MessagingEnvelope<TMessage> PrepareMessageEnvelope<TMessage>(TMessage message, Action<MessagingEnvelope> customizer = null)
        {
            var outgoingEnvelope = new MessagingEnvelope<TMessage>(new Dictionary<string, string>
            {
                [MessagingHeaders.MessageId] = Guid.NewGuid().ToString(),
                [MessagingHeaders.PublishTime] = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            }, message);

            if (message is IKeyProvider messageKeyProvider)
            {
                outgoingEnvelope.Headers[MessagingHeaders.StreamId] = messageKeyProvider.Key;
            }

            var sourceId = GetSourceId();
            if (!string.IsNullOrWhiteSpace(sourceId))
            {
                outgoingEnvelope.Headers[MessagingHeaders.Source] = sourceId;
            }

            customizer?.Invoke(outgoingEnvelope);

            outgoingEnvelope.SetHeader(MessagingHeaders.CorrelationId, Guid.NewGuid().ToString());

            return outgoingEnvelope;
        }

        private string GetSourceId()
        {
            var topicPrefix = _configuration.GetSection("Messaging")?["Source"];
            return topicPrefix ?? "";
        }
    }
}
