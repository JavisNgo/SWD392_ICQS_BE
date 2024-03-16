using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.BackgroundServices
{
    public class ExpiredRequestTimeoutChangeStatusToRejectedBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ExpiredRequestTimeoutChangeStatusToRejectedBackgroundService(IServiceProvider serviceProvider)
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
                    var expiredRequest = unitOfWork.RequestRepository.Get(
                        filter: c => c.TimeOut != null && c.TimeOut <= DateTime.Now
                    );

                    /// Update each expired contractor
                    foreach (var request in expiredRequest)
                    {
                        request.Status = Entities.Requests.RequestsStatusEnum.REJECTED; // Set default subscription ID

                        unitOfWork.RequestRepository.Update(request);

                        var deposit = unitOfWork.DepositOrdersRepository.Find(d => d.RequestId == request.Id).FirstOrDefault();
                        if (deposit != null)
                        {
                            if(deposit.Status.Equals(Entities.DepositOrders.DepositOrderStatusEnum.PENDING))
                            {
                                deposit.Status = Entities.DepositOrders.DepositOrderStatusEnum.REJECTED;
                                unitOfWork.DepositOrdersRepository.Update(deposit);
                            }
                            
                        }
                    }

                    // Save changes to the database
                    unitOfWork.Save();
                }

                // Sleep for a day before checking again
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
