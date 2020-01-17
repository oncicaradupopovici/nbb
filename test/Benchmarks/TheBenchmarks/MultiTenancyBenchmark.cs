using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBB.Contracts.Application.CommandHandlers;
using NBB.Contracts.Application.Commands;
using NBB.Contracts.Domain.ServicesContracts;
using NBB.Messaging.Abstractions;
using NBB.Messaging.DataContracts;
using NBB.Messaging.Nats;
using NBB.Tenancy.Abstractions;
using NBB.Tenancy.Impl;
using Serilog;
using Serilog.Events;
using STAN.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NBB.Messaging.Nats.Internal;

namespace TheBenchmarks
{

    class ApplicationLifetime : IApplicationLifetime
    {
        public CancellationToken ApplicationStarted => throw new NotImplementedException();

        public CancellationToken ApplicationStopping => throw new NotImplementedException();

        public CancellationToken ApplicationStopped => throw new NotImplementedException();

        public void StopApplication()
        {
            throw new NotImplementedException();
        }
    }

    [SimpleJob(launchCount: 1, warmupCount: 0, targetCount: 1)]
    //[HtmlExporter]
    //[RPlotExporter, RankColumn]
    public class MultiTenancyBenchmark
    {
        private IServiceProvider _container;
        private StanConnectionProvider _stanConnectionManager;
        private readonly int _msgsCnt = 2000;

        // [GlobalSetup(Target = nameof(NatsSubscribeTest))]
        [GlobalSetup]
        public void NatsGlobalSetup()
        {
            var services = GetServices();
            _container = services.BuildServiceProvider();
            //   SeedTopic();
            _stanConnectionManager = _container.GetService<StanConnectionProvider>();
        }

        public static IServiceCollection GetServices()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.Equals(environment, "development", StringComparison.OrdinalIgnoreCase);

            if (isDevelopment)
            {
                configurationBuilder.AddUserSecrets(Assembly.GetExecutingAssembly());
            }

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();
            services.AddSingleton<IApplicationLifetime, ApplicationLifetime>();
            services.AddSingleton<ITenantConfig, TenantConfig>();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddNatsMessaging();
            services.AddTenancy();
            services.AddSingleton<ITenantService, StubTenantService>();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
                loggingBuilder.AddConsole();
            });

            return services;
        }

        [Benchmark]
        [Arguments(1000)]
        [Arguments(10000)]
        [Arguments(500000)]
        public void SubscribeToTopicAsyncBecnh(int numberOfSubscriptions)
        {
            var list = new List<Task>();
            for (int i = 0; i < numberOfSubscriptions; i++)
                list.Add(SubscribeToTopicAsync($"Tenant_{i}", msg => Task.CompletedTask));

            Task.WhenAll(list).GetAwaiter().GetResult();
        }

        public Task SubscribeToTopicAsync(string subject, Func<string, Task> handler, CancellationToken cancellationToken = default, MessagingSubscriberOptions options = null)
        {
            var opts = StanSubscriptionOptions.GetDefaultOptions();
            opts.DurableName = "durable";
            var qGroup = "NBB.Benchmark";
            var subscriberOptions = options ?? new MessagingSubscriberOptions();
            opts.ManualAcks = subscriberOptions.AcknowledgeStrategy != MessagingAcknowledgeStrategy.Auto;
            opts.MaxInflight = 1;
            opts.AckWait = 50000;

            void StanMsgHandler(object obj, StanMsgHandlerArgs args) { }

            _stanConnectionManager.Execute(stanConnection =>
            {
                var _ = subscriberOptions.ConsumerType == MessagingConsumerType.CollaborativeConsumer
                    ? stanConnection.Subscribe(subject, opts, StanMsgHandler)
                    : stanConnection.Subscribe(subject, qGroup, opts, StanMsgHandler);
            });

            return Task.CompletedTask;
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            (_container as IDisposable).Dispose();
        }
    }

    class StubTenantService : ITenantService
    {
        public string GetTenantId() => "Tenant_6";
    }
}