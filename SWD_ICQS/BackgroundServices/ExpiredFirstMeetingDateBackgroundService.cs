using SWD_ICQS.Entities;
using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.BackgroundServices
{
    public class ExpiredFirstMeetingDateBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ExpiredFirstMeetingDateBackgroundService(IServiceProvider serviceProvider)
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
                    var expiredAppointment = unitOfWork.AppointmentRepository.Get(
                        filter: c => c.Status == Appointments.AppointmentsStatusEnum.PENDING && c.MeetingDate <= DateTime.Now
                    );

                    /// Update each expired contractor
                    foreach (var appointment in expiredAppointment)
                    {
                        appointment.Status = Appointments.AppointmentsStatusEnum.CANCELLED; // Set default subscription ID

                        unitOfWork.AppointmentRepository.Update(appointment);
                        unitOfWork.Save();
                        var countAppointment = unitOfWork.AppointmentRepository.Find(a => a.RequestId == appointment.RequestId);
                        
                        var existingRequest = unitOfWork.RequestRepository.GetByID(appointment.RequestId);
                        if (countAppointment.Count().Equals(2))
                        {
                            if (existingRequest != null)
                            {
                                existingRequest.Status = Requests.RequestsStatusEnum.REJECTED;
                                unitOfWork.RequestRepository.Update(existingRequest);
                                unitOfWork.Save();
                            }
                        }
                        if (countAppointment.Count().Equals(1))
                        {
                            if (existingRequest != null)
                            {
                                var newAppointment = new Appointments
                                {
                                    ContractorId = appointment.ContractorId,
                                    CustomerId = appointment.CustomerId,
                                    RequestId = appointment.RequestId,
                                    MeetingDate = existingRequest.TimeOut,
                                    Status = 0
                                };
                                unitOfWork.AppointmentRepository.Insert(newAppointment);
                                unitOfWork.Save();
                            }
                        }
                        
                    }

                    
                    
                    
                }

                // Sleep for a day before checking again
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}
