
using Agreement.API.Models;
using Agreement.API.Models.Responses;
using Agreement.API.Services.Interfaces;
using CronScheduler.Extensions.Scheduler;

namespace Agreement.API.Services
{
    public class AgreementJob : IScheduledJob
    {
        private readonly IAgreementService _service;
        private readonly ILogger<AgreementJob> _logger;

        public AgreementJob(IAgreementService service, ILogger<AgreementJob> logger) {
          _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Name { get; } = nameof(AgreementJob);

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                _logger.LogInformation("Account access up to date");
                AccountAccessList list = await _service.GetAllAccessListAsync();
                foreach (var access in list.Items)
                {
                    DateTime accessDuration = access.CREATEDAT + TimeSpan.FromMinutes(access.DURATION);
                    if (DateTime.Now > accessDuration)
                    {
                        access.STATUS = ACCESS_STATUS.EXPIRED.ToString();
                        access.UPDATEDAT = DateTime.Now;
                        await _service.UpdateAccessAsync(access);
                        _logger.LogInformation("Account access up to date");
                    }
                }
            }
        }
    }
}
