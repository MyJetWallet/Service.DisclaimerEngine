using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;

namespace Service.DisclaimerEngine
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyNoSqlClientLifeTime _noSqlClientLifeTime;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, ILogger<ApplicationLifetimeManager> logger, MyNoSqlClientLifeTime noSqlClientLifeTime)
            : base(appLifetime)
        {
            _logger = logger;
            _noSqlClientLifeTime = noSqlClientLifeTime;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _noSqlClientLifeTime.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _noSqlClientLifeTime.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
