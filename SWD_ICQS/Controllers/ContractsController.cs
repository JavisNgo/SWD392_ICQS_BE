using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;

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
