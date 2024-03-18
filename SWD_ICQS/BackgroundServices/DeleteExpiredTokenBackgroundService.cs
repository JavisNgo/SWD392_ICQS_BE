using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.BackgroundServices
{
    public class DeleteExpiredTokenBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public DeleteExpiredTokenBackgroundService(IServiceProvider serviceProvider)
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


                    var deleteExpiredToken = unitOfWork.TokenRepository.Get(filter: t => t.ExpiredDate < DateTime.Now);


                    foreach (var token in deleteExpiredToken)
                    {


                        unitOfWork.TokenRepository.Delete(token);
                    }


                    unitOfWork.Save();
                }


                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
