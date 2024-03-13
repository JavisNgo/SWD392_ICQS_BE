using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.BackgroundServices
{
    public class DeleteStatusFalseMessageBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DeleteStatusFalseMessageBackgroundService(IServiceProvider serviceProvider)
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

                    
                    var deleteMessage = unitOfWork.MessageRepository.Get(
                        filter:c => c.Status == false
                    );

                    
                    foreach (var message in deleteMessage)
                    {
                        

                        unitOfWork.MessageRepository.Delete(message);
                    }

                    
                    unitOfWork.Save();
                }

                
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}
