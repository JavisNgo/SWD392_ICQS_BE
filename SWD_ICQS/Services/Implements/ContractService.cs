using AutoMapper;
using Org.BouncyCastle.Asn1.Ocsp;
using SimpleEmailApp.Models;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Diagnostics.Contracts;
using System.Net.Mail;
using System.Net;
using System.Security.Policy;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SWD_ICQS.Services.Implements
{
    public class ContractService : IContractService
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _PDFFileDirectory;
        private readonly IConfiguration _config;

        public ContractService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _PDFFileDirectory = Path.Combine(env.ContentRootPath, "pdf", "contracts");
            _config = config;
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
                checkingDeposit.Status == DepositOrders.DepositOrderStatusEnum.PENDING)
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
        public IEnumerable<ContractViewForGet>? GetAllContract()
        {
            try
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

                    string url = $"https://localhost:7233/pdf/contracts/{contract.ContractUrl}";
                    contractView.ContractUrl = url;


                    contractsViews.Add(contractView);
                }
                return contractsViews;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ContractViewForGet? GetContractById(int contractId)
        {
            try
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
            } catch (Exception ex )
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ContractViewForGet>? GetContractsByContractorId(int contractorId)
        {
            try
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
                    string url = $"https://localhost:7233/pdf/contracts/{contract.ContractUrl}";
                    contractView.ContractUrl = url;

                    contractsViews.Add(contractView);
                }

                return contractsViews;
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ContractViewForGet>? GetContractsByCustomerId(int customerId)
        {
            try
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
                    string url = $"https://localhost:7233/pdf/contracts/{contract.ContractUrl}";
                    contractView.ContractUrl = url;

                    contractsViews.Add(contractView);
                }
                return contractsViews;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

                var existingContractor = unitOfWork.ContractorRepository.GetByID(checkingRequest.ContractorId);

                if (existingContractor != null)
                {

                    EmailDto email = new EmailDto()
                    {

                        To = existingContractor.Email,
                        Subject = "New request from customer",
                        Body = emailBodyForCustomerFirst(existingContractor, checkingRequest)

                    };

                    SendMail(email);

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public string emailBodyForCustomerFirst(Contractors contractor, Requests request)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Notification</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        h1 {{
            color: #333;
        }}
        p {{
            color: #666;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>New contract from customer</h1>
        <p>Dear {contractor.Name},</p>
        <p>You have a new contract from customer</p>        
        

        <p>Please check contract.</p></br></br>

        <p>Best regards,<br/>[Admin from ICQS]</p>
    </div>
</body>
</html>
";
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

                    var existingCustomer = unitOfWork.CustomerRepository.GetByID(checkingRequest.CustomerId);

                    if (existingCustomer != null)
                    {

                        EmailDto email = new EmailDto()
                        {

                            To = existingCustomer.Email,
                            Subject = "New request from customer",
                            Body = emailBodyForContractorSecond(existingCustomer, checkingRequest)

                        };

                        SendMail(email);

                    }
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

        public string emailBodyForContractorSecond(Customers customer, Requests request)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Notification</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        h1 {{
            color: #333;
        }}
        p {{
            color: #666;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Contractor checked your contract.</h1>
        <p>Dear {customer.Name},</p>
        <p>Contractor accept your contract</p>        
        

        <p>Please check contract and pay deposit order before expired date.</p></br></br>
        <p>Expired date: {request.TimeOut}</p>  

        <p>Best regards,<br/>[Admin from ICQS]</p>
    </div>
</body>
</html>
";
        }

        public void SendMail(EmailDto request)
        {
            try
            {
                string fromMail = _config.GetSection("EmailUsername").Value;
                string fromPassword = _config.GetSection("EmailPassword").Value;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = request.Subject;
                message.To.Add(new MailAddress(request.To));
                message.Body = request.Body;
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient(_config.GetSection("EmailHost").Value)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };

                smtpClient.Send(message);
            }
            catch

            {
                throw new Exception("Error while send mail");
            }

        }

        public bool UploadContractProgress(int id, ContractsView contractView)
        {
            var existingContract = unitOfWork.ContractRepository.GetByID(id);

            if (existingContract == null)
            {
                return false; 
            }
            try
            {
                if(existingContract.Status == 1)
                {
                    existingContract.Progress = contractView.Progress;
                    existingContract.EditDate = DateTime.Now;
                    unitOfWork.ContractRepository.Update(existingContract);
                    unitOfWork.Save();
                    return true;
                } else
                {
                    throw new Exception("Contract has not been signed yet");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating progress. Error message: {ex.Message}");

            }
        }

        public ContractViewForGet? GetContractByRequestId(int requestId)
        {
            try
            {
                var contract = unitOfWork.ContractRepository.Find(c => c.RequestId == requestId).FirstOrDefault();
                var request = unitOfWork.RequestRepository.GetByID(requestId);

                var customer = unitOfWork.CustomerRepository.GetByID(request.CustomerId);

                var contractor = unitOfWork.ContractorRepository.GetByID(request.ContractorId);

                var contractView = _mapper.Map<ContractViewForGet>(contract);

                contractView.CustomerName = customer.Name;
                contractView.ContractorName = contractor != null ? contractor.Name : null;
                string url = $"https://localhost:7233/pdf/contracts/{contract.ContractUrl}";
                contractView.ContractUrl = url;

                return contractView;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
