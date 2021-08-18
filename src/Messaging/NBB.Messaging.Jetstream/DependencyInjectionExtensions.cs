using Microsoft.Extensions.Configuration;
using NBB.Messaging.Abstractions;
using NBB.Messaging.Nats;
using NBB.Messaging.Nats.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddJetstreamTransport(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NatsOptions>(configuration.GetSection("Messaging").GetSection("Nats"));
            services.AddSingleton<JetstreamConnectionManager>();
            services.AddSingleton<IMessagingTransport, JetstreamMessagingTransport>();

            return services;
        }
    }
}
