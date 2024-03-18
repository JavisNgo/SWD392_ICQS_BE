using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Diagnostics.Contracts;

namespace SWD_ICQS.Services.Implements
{
    public class ContractService : IContractService
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _PDFFileDirectory;

        public ContractService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _PDFFileDirectory = Path.Combine(env.ContentRootPath, "pdf", "contracts");
        }
        public ContractsView AddContract(ContractsView contractView)
        {
            var checkingRequest = unitOfWork.RequestRepository.GetByID(contractView.RequestId);

            if (checkingRequest == null)
            {
                throw new Exception($"Request with ID {contractView.RequestId} not found.");
            }

            var appointment = new Appointments();
            var checkingAppointment = unitOfWork.AppointmentRepository.Find(a => a.RequestId == contractView.RequestId).ToList();
            if(checkingAppointment.Count == 1)
            {
                 appointment = checkingAppointment[0];
            } else if(checkingAppointment.Count == 2)
            {
                appointment = checkingAppointment[1];
            }
            var checkingDeposit = unitOfWork.DepositOrdersRepository.Find(d => d.RequestId == contractView.RequestId).FirstOrDefault();
            if(checkingDeposit == null)
            {
                throw new Exception($"Deposit  not found.");
            }    

            // Kiểm tra xem trạng thái của yêu cầu và cuộc hẹn có đúng không
            if (checkingRequest.Status == Requests.RequestsStatusEnum.COMPLETED &&
                appointment.Status == Appointments.AppointmentsStatusEnum.COMPLETED && 
                checkingDeposit.Status == DepositOrders.DepositOrderStatusEnum.COMPLETED)
            {
                // Kiểm tra xem thời hạn Timeout của yêu cầu còn hay không
                if (checkingRequest.TimeOut.HasValue && checkingRequest.TimeOut > DateTime.Now)
                {
                    // Cập nhật trạng thái của yêu cầu thành SIGNED
                    //checkingRequest.Status = Requests.RequestsStatusEnum.SIGNED;
                    //unitOfWork.RequestRepository.Update(checkingRequest);

                    // Lấy và cập nhật appointment gần nhất thành SIGNED
                    //var latestAppointment = unitOfWork.AppointmentRepository.Get(
                    //    filter: a => a.RequestId == checkingRequest.Id,
                    //    orderBy: q => q.OrderByDescending(a => a.MeetingDate),
                    //    includeProperties: "Request"
                    //).FirstOrDefault();

                    //if (latestAppointment != null)
                    //{
                    //    latestAppointment.Status = Appointments.AppointmentsStatusEnum.SIGNED;
                    //    unitOfWork.AppointmentRepository.Update(latestAppointment);
                    //}

                    // Tạo một Contracts mới
                    var newContract = _mapper.Map<Contracts>(contractView);
                    newContract.ContractUrl = null;
                    newContract.Status = 0;
                    newContract.Progress = null;

                    // Thêm bản hợp đồng vào repository
                    unitOfWork.ContractRepository.Insert(newContract);

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    unitOfWork.Save();

                    
                }
            }
            return contractView;
        }
            public IEnumerable<ContractViewForGet> GetAllContract()
        {
            var contracts = unitOfWork.ContractRepository.Get().ToList();
            var contractsViews = new List<ContractViewForGet>();

            foreach (var contract in contracts)
            {
                // Lấy thông tin Request
                var request = unitOfWork.RequestRepository.GetByID(contract.RequestId);

                if (request == null)
                {
                    // Nếu không thấy cút đi tiếp
                    continue;
                }

                // Lấy thông tin của Customer từ Request
                var customer = unitOfWork.CustomerRepository.GetByID(request.CustomerId);

                // Lấy thông tin của Contractor từ Request
                var contractor = unitOfWork.ContractorRepository.GetByID(request.ContractorId);

                if (customer == null || contractor == null)
                {
                    //Nếu k thấy đi tiếp
                    continue;
                }

                var contractView = _mapper.Map<ContractViewForGet>(contract);

                // Gán tên của Customer và Contractor vào ContractView
                contractView.CustomerName = customer.Name;
                contractView.ContractorName = contractor.Name;

                contractsViews.Add(contractView);
            }
            return contractsViews;
        }
            public ContractViewForGet GetContractById(int contractId)
            {
            // Lấy thông tin hợp đồng theo contractId
            var contract = unitOfWork.ContractRepository.GetByID(contractId);

            if (contract == null)
            {
                throw new Exception($"Contract with ID {contractId} not found.");
            }

            // Lấy thông tin request của hợp đồng
            var request = unitOfWork.RequestRepository.GetByID(contract.RequestId);

            if (request == null)
            {
                throw new Exception($"Request related to Contract with ID {contractId} not found.");
            }

            // Lấy thông tin của Customer từ request
            var customer = unitOfWork.CustomerRepository.GetByID(request.CustomerId);

            // Lấy thông tin của Contractor từ request
            var contractor = unitOfWork.ContractorRepository.GetByID(request.ContractorId);

            var contractView = _mapper.Map<ContractViewForGet>(contract);

            // Gán tên của Customer và Contractor vào ContractViewForGet
            contractView.CustomerName = customer != null ? customer.Name : null;
            contractView.ContractorName = contractor != null ? contractor.Name : null;

            return contractView;
            }

        public IEnumerable<ContractViewForGet>? GetContractsByContractorId(int contractorId)
        {
            // Lấy tất cả các hợp đồng có liên quan đến nhà thầu có contractorId cung cấp
            var contracts = unitOfWork.ContractRepository.Get(filter: c => c.Request.ContractorId == contractorId).ToList();
            var contractsViews = new List<ContractViewForGet>();

            foreach (var contract in contracts)
            {
                var request = unitOfWork.RequestRepository.GetByID(contract.RequestId);

                if (request == null)
                {
                    continue;
                }

                var contractor = unitOfWork.ContractorRepository.GetByID(request.ContractorId);

                if (contractor == null)
                {
                    continue;
                }

                var customer = unitOfWork.CustomerRepository.GetByID(request.CustomerId);

                var contractView = _mapper.Map<ContractViewForGet>(contract);

                contractView.ContractorName = contractor.Name;
                contractView.CustomerName = customer != null ? customer.Name : null;

                contractsViews.Add(contractView);
            }

            return contractsViews;
        }

        public IEnumerable<ContractViewForGet>? GetContractsByCustomerId(int customerId)
        {
            var contracts = unitOfWork.ContractRepository.Get(filter: c => c.Request.CustomerId == customerId).ToList();
            var contractsViews = new List<ContractViewForGet>();

            foreach (var contract in contracts)
            {
                var request = unitOfWork.RequestRepository.GetByID(contract.RequestId);

                if (request == null)
                {
                    continue;
                }

                var customer = unitOfWork.CustomerRepository.GetByID(request.CustomerId);

                if (customer == null)
                {
                    continue;
                }

                var contractor = unitOfWork.ContractorRepository.GetByID(request.ContractorId);

                var contractView = _mapper.Map<ContractViewForGet>(contract);

                contractView.CustomerName = customer.Name;
                contractView.ContractorName = contractor != null ? contractor.Name : null;

                contractsViews.Add(contractView);
            }
            return contractsViews;
        }

        public void UpdateContractCustomerFirst(int id, ContractsView contractsView)
        {
            try
            {
                
                var existingContract = unitOfWork.ContractRepository.GetByID(id);

                if (existingContract == null)
                {
                    throw new Exception($"Contract with ID {id} not found.");
                }
                var checkingRequest = unitOfWork.RequestRepository.GetByID(contractsView.RequestId);

                if (checkingRequest == null)
                {
                    throw new Exception($"Request with ID {contractsView.RequestId} not found.");
                }
                var checkingAppointment = unitOfWork.AppointmentRepository.Find(c => c.RequestId == contractsView.RequestId).ToList();

                if (checkingAppointment == null)
                {
                    throw new Exception("Appointment was not found");
                }
                
                var contract = _mapper.Map(contractsView, existingContract);
                contract.UploadDate = DateTime.Now;
                byte[] pdfBytes = Convert.FromBase64String(contractsView.ContractUrl);
                string filename = $"Contract_{existingContract.Id}.pdf";
                contract.ContractUrl = filename;
                string pdfPath = Path.Combine(_PDFFileDirectory, filename);
                System.IO.File.WriteAllBytes(pdfPath, pdfBytes);
                contract.Progress = "Customer signed contract";

                unitOfWork.ContractRepository.Update(existingContract);
                unitOfWork.Save();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public bool UpdateContractContractorSecond(int id, ContractsView contractsView)
        {
            try
            {
                bool status = false;
                var existingContract = unitOfWork.ContractRepository.GetByID(id);

                if (existingContract == null)
                {
                    throw new Exception($"Contract with ID {id} not found.");
                }
                var checkingRequest = unitOfWork.RequestRepository.GetByID(contractsView.RequestId);

                if (checkingRequest == null)
                {
                    throw new Exception($"Request with ID {contractsView.RequestId} not found.");
                }
                var checkingAppointment = unitOfWork.AppointmentRepository.Find(c => c.RequestId == contractsView.RequestId).ToList();

                if (checkingAppointment == null)
                {
                    throw new Exception("Appointment was not found");
                }

                var appointment = new Appointments();
                if (checkingAppointment.Count == 1)
                {
                    appointment = checkingAppointment[0];
                }
                else if (checkingAppointment.Count == 2)
                {
                    appointment = checkingAppointment[1];
                }

                var existingFilename = $"Contract_{existingContract.Id}.pdf";

                if (existingContract.ContractUrl.Equals(existingFilename))
                {
                    var contract = _mapper.Map(contractsView, existingContract);
                    contract.EditDate = DateTime.Now;

                    byte[] pdfBytes = Convert.FromBase64String(contractsView.ContractUrl);
                    string filename = $"Contract_{existingContract.Id}_Signed.pdf";
                    contract.ContractUrl = filename;
                    string pdfPath = Path.Combine(_PDFFileDirectory, filename);
                    System.IO.File.WriteAllBytes(pdfPath, pdfBytes);


                    contract.Progress = "Contractor signed contract";
                    contract.Status = 1;
                    unitOfWork.ContractRepository.Update(existingContract);
                    checkingRequest.Status = Requests.RequestsStatusEnum.SIGNED;
                    unitOfWork.RequestRepository.Update(checkingRequest);
                    appointment.Status = Appointments.AppointmentsStatusEnum.SIGNED;
                    unitOfWork.AppointmentRepository.Update(appointment);
                    
                    unitOfWork.Save();
                    status = true;
                }
                else
                {
                    throw new Exception("Customer have not signed contract yet");
                }

                return status;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public bool UploadContractProgress(int id, ContractsView contractView)
        {
            var existingContract = unitOfWork.ContractRepository.GetByID(id);

            if (existingContract == null)
            {
                return false; 
            }

            existingContract.Progress = contractView.Progress;

            try
            {
                unitOfWork.ContractRepository.Update(existingContract);
                unitOfWork.Save();
                return true; 
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating progress. Error message: {ex.Message}");

            }
        }

    }
}
