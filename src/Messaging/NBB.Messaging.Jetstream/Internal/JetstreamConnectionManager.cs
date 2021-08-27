using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.Messaging.Nats.Internal
{
    public class JetstreamConnectionManager : IDisposable
    {
        private readonly IOptions<NatsOptions> _natsOptions;
        private readonly ILogger<JetstreamConnectionManager> _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private IConnection _connection;
        private Exception _unrecoverableException;
        private readonly Lazy<IConnection> _lazyConnection;

        public JetstreamConnectionManager(IOptions<NatsOptions> natsOptions, ILogger<JetstreamConnectionManager> logger,
            IHostApplicationLifetime applicationLifetime)
        {
            _natsOptions = natsOptions;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _lazyConnection = new Lazy<IConnection>(GetConnection);
        }

        public async Task ExecuteAsync(Func<IConnection, Task> action)
        {
            var connection = GetAndCheckConnection();
            try
            {
                await action(connection);
            }
            catch (Exception ex)
                when (IsUnrecoverableException(ex))
            {
                SetUnrecoverableState(ex);
                throw;
            }
        }

        public void Execute(Action<IConnection> action)
        {
            var connection = GetAndCheckConnection();
            try
            {
                action(connection);
            }
            catch (Exception ex)
                when (IsUnrecoverableException(ex))
            {
                SetUnrecoverableState(ex);
                throw;
            }
        }

        private IConnection GetAndCheckConnection()
        {
            ThrowIfUnrecoverableState();
            try
            {
                // Exception from the lazy factory method is cached
                return _lazyConnection.Value;
            }
            catch (Exception ex)
            {
                SetUnrecoverableState(ex);
                throw;
            }
        }

        private IConnection GetConnection()
        {
            var clientId = _natsOptions.Value.ClientId?.Replace(".", "_");
            var options = ConnectionFactory.GetDefaultOptions();
            options.Url = _natsOptions.Value.NatsUrl;
            options.Password = _natsOptions.Value.Password;
            options.User = _natsOptions.Value.User;
            //options.SetNkey("UC2E5KVSC44GNNVGWZTDKOSJYX2FYREACOEUXW4QVR46MJBQEDSZN5BM", "./user.nk");
            options.ClosedEventHandler = (_, args) =>
            {
                SetUnrecoverableState(args.Error ?? new Exception("NATS connection was lost"));
            };

       //     options.Timeout = Defaults.Timeout;

            var cf = new ConnectionFactory();
            _connection = cf.CreateConnection(options);

            return _connection;
        }

        private void SetUnrecoverableState(Exception exception)
        {
            // Set the field to the current exception if not already set
            var existingException =
                Interlocked.CompareExchange(ref _unrecoverableException, exception, null);

            // Send the application stop signal only once
            if (existingException != null)
                return;

            _logger.LogCritical(exception, "NATS connection unrecoverable");
            _applicationLifetime.StopApplication();

            Dispose();
        }

        private void ThrowIfUnrecoverableState()
        {
            // For consistency, read the field using the same primitive used for writing instead of using Thread.VolatileRead
            var exception = Interlocked.CompareExchange(ref _unrecoverableException, null, null);
            if (exception != null)
            {
                throw new Exception("NATS connection encountered an unrecoverable exception", exception);
            }
        }

        private static bool IsUnrecoverableException(Exception ex) =>
            ex is NATSConnectionClosedException ||
            ex is NATSConnectionException ||
            ex is NATSBadSubscriptionException ||
            ex is NATSTimeoutException ||
            ex is NATSStaleConnectionException ||
            ex is NATSNoServersException;

        public void Dispose() => _connection?.Dispose();
    }
}