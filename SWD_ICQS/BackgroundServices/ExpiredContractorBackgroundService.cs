using AutoMapper;
using SWD_ICQS.Repository;
using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.BackgroundServices
{
    public class ExpiredContractorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ExpiredContractorBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // Get all contractors whose expiration date has passed
                    var expiredContractors = unitOfWork.ContractorRepository.Get(
                        filter: c => c.ExpiredDate != null && c.ExpiredDate <= DateTime.Now
                    );

                    /// Update each expired contractor
                    foreach (var contractor in expiredContractors)
                    {
                        contractor.SubscriptionId = 1; // Set default subscription ID
                        contractor.ExpiredDate = new DateTime(2099, 12, 31, 23, 59, 0); // Set default expiration date
                        unitOfWork.ContractorRepository.Update(contractor);
                    }

                    // Save changes to the database
                    unitOfWork.Save();
                }

                // Sleep for a day before checking again
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}
