using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Services.Implements
{
    public class AppointmentService : IAppointmentService
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<AppointmentView> GetAllAppointment()
        {
            var appointments = unitOfWork.AppointmentRepository.Get().ToList();
            var appointmentViews = new List<AppointmentView>();

            foreach (var appointment in appointments)
            {
                var customer = unitOfWork.CustomerRepository.GetByID(appointment.CustomerId);
                var contractor = unitOfWork.ContractorRepository.GetByID(appointment.ContractorId);

                if (customer != null && contractor != null)
                {
                    var appointmentView = new AppointmentView
                    {
                        Id = appointment.Id,
                        CustomerId = appointment.CustomerId,
                        ContractorId = appointment.ContractorId,
                        CustomerName = customer.Name,
                        ContractorName = contractor.Name,
                        RequestId = appointment.RequestId,
                        MeetingDate = appointment.MeetingDate,
                        Status = appointment.Status
                    };

                    appointmentViews.Add(appointmentView);
                }
            }

            return appointmentViews;
        }

        public IEnumerable<AppointmentView> GetAppointmentByContractorId(int contractorId)
        {
            var appointments = unitOfWork.AppointmentRepository.Get(filter: c => c.ContractorId == contractorId).ToList();
            var appointmentViews = new List<AppointmentView>();

            foreach (var appointment in appointments)
            {
                var contractor = unitOfWork.ContractorRepository.GetByID(appointment.ContractorId);
                if (contractor == null)
                {
                    continue;
                }

                var customer = unitOfWork.CustomerRepository.GetByID(appointment.CustomerId);

                var appointmentView = _mapper.Map<AppointmentView>(appointment);
                appointmentView.ContractorName = contractor.Name;
                appointmentView.CustomerName = customer != null ? customer.Name : null;

                appointmentViews.Add(appointmentView);
            }

            return appointmentViews;
        }

        public IEnumerable<AppointmentView> GetAppointmentByCustomerId(int customerId)
        {
            var appointments = unitOfWork.AppointmentRepository.Get(filter: c => c.CustomerId == customerId).ToList();
            var appointmentViews = new List<AppointmentView>();
            foreach (var appointment in appointments)
            {
                var customer = unitOfWork.CustomerRepository.GetByID(appointment.CustomerId);
                if (customer == null)
                {
                    continue;
                }
                var contractor = unitOfWork.ContractorRepository.GetByID(appointment.ContractorId);

                var appointmentView = _mapper.Map<AppointmentView>(appointment);
                appointmentView.CustomerName = customer.Name;
                appointmentView.ContractorName = contractor != null ? contractor.Name : null;

                appointmentViews.Add(appointmentView);
            }

            return appointmentViews;
        }
    }
}
