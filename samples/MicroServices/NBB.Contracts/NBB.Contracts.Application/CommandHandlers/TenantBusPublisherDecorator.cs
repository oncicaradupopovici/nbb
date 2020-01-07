using NBB.Contracts.Domain.ServicesContracts;
using NBB.Messaging.Abstractions;
using NBB.Messaging.DataContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.Contracts.Application.CommandHandlers
{
    public class TenantBusPublisherDecorator : IMessageBusPublisher
    {
        private readonly IMessageBusPublisher _inner;
        private readonly ITenantService _tenantService;

        public TenantBusPublisherDecorator(IMessageBusPublisher inner, ITenantService tenantService)
        {
            _inner = inner;
            _tenantService = tenantService;
        }

        public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default, Action<MessagingEnvelope> envelopeCustomizer = null, string topicName = null)
        {
            void NewCustomizer(MessagingEnvelope outgoingEnvelope)
            {
                outgoingEnvelope.SetHeader("TenantId", _tenantService.GetTenantId());
                envelopeCustomizer?.Invoke(outgoingEnvelope);
            }

            return _inner.PublishAsync(message, cancellationToken, NewCustomizer);
        }
    }
}