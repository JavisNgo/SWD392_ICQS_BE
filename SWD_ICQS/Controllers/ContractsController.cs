using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using System.Diagnostics.Contracts;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public ContractsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/contracts")]
        public ActionResult<IEnumerable<ContractViewForGet>> GetContracts()
        {
            try
            {
                var contracts = unitOfWork.ContractRepository.Get().ToList();
                var contractsViews = new List<ContractViewForGet>();

                foreach (var contract in contracts)
                {
                    // Lấy thông tin Appointment
                    var appointment = unitOfWork.AppointmentRepository.GetByID(contract.AppointmentId);

                    if (appointment == null)
                    {
                        // Nếu không tìm thấy Appointment, bỏ qua contract này
                        continue;
                    }

                    // Lấy thông tin của Customer từ Appointment
                    var customer = unitOfWork.CustomerRepository.GetByID(appointment.CustomerId);

                    // Lấy thông tin của Contractor từ Appointment
                    var contractor = unitOfWork.ContractorRepository.GetByID(appointment.ContractorId);

                    if (customer == null || contractor == null)
                    {
                        // Nếu không tìm thấy thông tin của Customer hoặc Contractor, bỏ qua contract này
                        continue;
                    }

                    var contractView = _mapper.Map<ContractViewForGet>(contract);

                    // Gán tên của Customer và Contractor vào ContractView
                    contractView.CustomerName = customer.Name;
                    contractView.ContractorName = contractor.Name;

                    contractsViews.Add(contractView);
                }

                return Ok(contractsViews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("/api/v1/contracts/{contractId}")]
        public ActionResult<ContractViewForGet> GetContractById(int contractId)
        {
            try
            {
                // Lấy thông tin hợp đồng theo contractId
                var contract = unitOfWork.ContractRepository.GetByID(contractId);

                if (contract == null)
                {
                    return NotFound($"Contract with ID {contractId} not found.");
                }

                // Lấy thông tin Appointment của hợp đồng
                var appointment = unitOfWork.AppointmentRepository.GetByID(contract.AppointmentId);

                if (appointment == null)
                {
                    return NotFound($"Appointment related to Contract with ID {contractId} not found.");
                }

                // Lấy thông tin của Customer từ Appointment
                var customer = unitOfWork.CustomerRepository.GetByID(appointment.CustomerId);

                // Lấy thông tin của Contractor từ Appointment
                var contractor = unitOfWork.ContractorRepository.GetByID(appointment.ContractorId);

                var contractView = _mapper.Map<ContractViewForGet>(contract);

                // Gán tên của Customer và Contractor vào ContractViewForGet
                contractView.CustomerName = customer != null ? customer.Name : null;
                contractView.ContractorName = contractor != null ? contractor.Name : null;

                return Ok(contractView);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpGet("/api/v1/contracts/contractor/{contractorId}")]
        public ActionResult<IEnumerable<ContractViewForGet>> GetContractsByContractorId(int contractorId)
        {
            try
            {
                // Lấy tất cả các hợp đồng có liên quan đến nhà thầu có contractorId cung cấp
                var contracts = unitOfWork.ContractRepository.Get(filter: c => c.Appointment.ContractorId == contractorId).ToList();
                var contractsViews = new List<ContractViewForGet>();

                foreach (var contract in contracts)
                {
                    // Lấy thông tin Appointment
                    var appointment = unitOfWork.AppointmentRepository.GetByID(contract.AppointmentId);

                    if (appointment == null)
                    {
                        // Nếu không tìm thấy Appointment, bỏ qua contract này
                        continue;
                    }

                    // Lấy thông tin của Customer từ Appointment
                    var contractor = unitOfWork.ContractorRepository.GetByID(appointment.ContractorId);

                    if (contractor == null)
                    {
                        // Nếu không tìm thấy thông tin của Contractor, bỏ qua contract này
                        continue;
                    }

                    // Lấy thông tin của Customer từ Appointment
                    var customer = unitOfWork.CustomerRepository.GetByID(appointment.CustomerId);

                    var contractView = _mapper.Map<ContractViewForGet>(contract);

                    // Gán tên của Contractor và Customer vào ContractViewForGet
                    contractView.ContractorName = contractor.Name;
                    contractView.CustomerName = customer != null ? customer.Name : null;

                    contractsViews.Add(contractView);
                }

                return Ok(contractsViews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/contracts/customer/{customerId}")]
        public ActionResult<IEnumerable<ContractViewForGet>> GetContractsByCustomerId(int customerId)
        {
            try
            {
                // Lấy tất cả các hợp đồng có liên quan đến nhà thầu có CustomerId cung cấp
                var contracts = unitOfWork.ContractRepository.Get(filter: c => c.Appointment.CustomerId == customerId).ToList();
                var contractsViews = new List<ContractViewForGet>();

                foreach (var contract in contracts)
                {
                    // Lấy thông tin Appointment
                    var appointment = unitOfWork.AppointmentRepository.GetByID(contract.AppointmentId);

                    if (appointment == null)
                    {
                        // Nếu không tìm thấy Appointment, bỏ qua contract này
                        continue;
                    }

                    // Lấy thông tin của Customer từ Appointment
                    var customer = unitOfWork.CustomerRepository.GetByID(appointment.CustomerId);

                    if (customer == null)
                    {
                        // Nếu không tìm thấy thông tin của Contractor, bỏ qua contract này
                        continue;
                    }

                    // Lấy thông tin của Contractor từ Appointment
                    var contractor = unitOfWork.ContractorRepository.GetByID(appointment.ContractorId);

                    var contractView = _mapper.Map<ContractViewForGet>(contract);

                    // Gán tên của Customer và nhà thầu vào ContractViewForGet
                    contractView.CustomerName = customer.Name;
                    contractView.ContractorName = contractor != null ? contractor.Name : null;

                    contractsViews.Add(contractView);
                }

                return Ok(contractsViews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [AllowAnonymous]
        [HttpPost("/Contracts")]
        public IActionResult AddContract([FromBody] ContractsView contractView)
        {
            try
            {
                var checkingRequest = unitOfWork.RequestRepository.GetByID(contractView.RequestId);

                if (checkingRequest == null)
                {
                    return NotFound($"Request with ID {contractView.RequestId} not found.");
                }

                var checkingAppointment = unitOfWork.AppointmentRepository.GetByID(contractView.AppointmentId);

                if (checkingAppointment == null)
                {
                    return NotFound($"Appointment with ID {contractView.AppointmentId} not found.");
                }

                // Kiểm tra xem trạng thái của yêu cầu và cuộc hẹn có đúng không
                if (checkingRequest.Status == Requests.RequestsStatusEnum.COMPLETED &&
                    checkingAppointment.Status == Appointments.AppointmentsStatusEnum.COMPLETED)
                {
                    // Kiểm tra xem thời hạn Timeout của yêu cầu còn hay không
                    if (checkingRequest.TimeOut.HasValue && checkingRequest.TimeOut > DateTime.Now)
                    {
                        // Cập nhật trạng thái của yêu cầu thành SIGNED
                        checkingRequest.Status = Requests.RequestsStatusEnum.SIGNED;
                        unitOfWork.RequestRepository.Update(checkingRequest);

                        // Lấy và cập nhật appointment gần nhất thành SIGNED
                        var latestAppointment = unitOfWork.AppointmentRepository.Get(
                            filter: a => a.RequestId == checkingRequest.Id,
                            orderBy: q => q.OrderByDescending(a => a.MeetingDate),
                            includeProperties: "Request"
                        ).FirstOrDefault();

                        if (latestAppointment != null)
                        {
                            latestAppointment.Status = Appointments.AppointmentsStatusEnum.SIGNED;
                            unitOfWork.AppointmentRepository.Update(latestAppointment);
                        }

                        // Tạo một Contracts mới
                        var newContract = _mapper.Map<Contracts>(contractView);
                        newContract.UploadDate = DateTime.Now;

                        // Thêm bản hợp đồng vào repository
                        unitOfWork.ContractRepository.Insert(newContract);

                        // Lưu các thay đổi vào cơ sở dữ liệu
                        unitOfWork.Save();

                        return Ok(contractView);
                    }
                    else
                    {
                        return BadRequest("The request has expired.");
                    }
                }
                else
                {
                    return BadRequest("Both request and appointment must have status COMPLETED.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the contract. Error message: {ex.Message}");
            }
        }


        [HttpPut("{contractId}")]
        public IActionResult UploadContractProgress(int contractId, [FromBody] ContractsView contractView)
        {
            try
            {
                var existingContract = unitOfWork.ContractRepository.GetByID(contractId);

                if (existingContract == null)
                {
                    return NotFound($"Contract with ID {contractId} not found.");
                }

                // Update progress
                existingContract.Progress = contractView.Progress;

                unitOfWork.ContractRepository.Update(existingContract);
                unitOfWork.Save();

                return Ok($"Contract with ID {contractId} updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the contract progress. Error message: {ex.Message}");
            }
        }

        [HttpPut("/Contracts/{id}")]
        public IActionResult UpdateContract(int id, [FromBody] ContractsView contractsView)
        {
            try
            {
                var existingContract = unitOfWork.ContractRepository.GetByID(id);

                if (existingContract == null)
                {
                    return NotFound($"Contract with ID {id} not found.");
                }
                var checkingRequest = unitOfWork.RequestRepository.GetByID(contractsView.RequestId);

                if (checkingRequest == null)
                {
                    return NotFound($"Request with ID {contractsView.RequestId} not found.");
                }
                var checkingAppointmentId = unitOfWork.AppointmentRepository.GetByID(id);
                // Validate the name using a regular expression
                if (checkingAppointmentId == null)
                {
                    return NotFound("AppointmentId was not found");
                }

                var contract = _mapper.Map(contractsView, existingContract);
                contract.EditDate = DateTime.Now;
                // Mark the entity as modified
                unitOfWork.ContractRepository.Update(existingContract);
                unitOfWork.Save();

                return Ok(contractsView); 
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the contract. Error message: {ex.Message}");
            }
        }
    }
}
