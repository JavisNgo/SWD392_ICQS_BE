using AutoMapper;
using Org.BouncyCastle.Asn1.Ocsp;
using SimpleEmailApp.Models;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Transactions;

namespace SWD_ICQS.Services.Implements
{
    public class DepositOrdersService : IDepositOrdersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public DepositOrdersService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
        }

        public IEnumerable<DepositOrders>? GetDepositOrders()
        {
            try
            {
                var DepositOrders = _unitOfWork.DepositOrdersRepository.Get();

                return DepositOrders;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<DepositOrders>? GetDepositOrdersByCustomerId(int CustomerId)
        {
            try
            {
                List<DepositOrders> depositOrders = new List<DepositOrders>();
                var Requests = _unitOfWork.RequestRepository.Find(r => r.CustomerId == CustomerId).ToList();
                if (Requests != null)
                {
                    if (Requests.Any())
                    {
                        foreach(var Request in Requests)
                        {
                            var depositOrder = _unitOfWork.DepositOrdersRepository.Find(d => d.RequestId == Request.Id).FirstOrDefault();
                            if(depositOrder != null)
                            {
                                depositOrders.Add(depositOrder);
                            }
                        }
                    }
                }
                return depositOrders;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DepositOrders? GetDepositOrdersById(int id)
        {
            try
            {
                var depositOrder = _unitOfWork.DepositOrdersRepository.GetByID(id);

                return depositOrder;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DepositOrders? GetDepositOrdersByRequestId(int RequestsId)
        {
            try
            {
                var depositOrder = _unitOfWork.DepositOrdersRepository.Find(d => d.RequestId == RequestsId).FirstOrDefault();
                return depositOrder;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateStatus(int id)
        {
            try
            {
                var depositOrder = _unitOfWork.DepositOrdersRepository.GetByID(id);
                if (depositOrder != null)
                {
                    if(depositOrder.TransactionCode != null)
                    {
                        depositOrder.Status = DepositOrders.DepositOrderStatusEnum.COMPLETED;
                        _unitOfWork.DepositOrdersRepository.Update(depositOrder);
                        _unitOfWork.Save();
                        var checkRequest = _unitOfWork.RequestRepository.Find(r => r.Id == depositOrder.RequestId).FirstOrDefault();
                        if (checkRequest == null)
                        {
                            return false;
                        }

                        var existingCustomer = _unitOfWork.CustomerRepository.GetByID(checkRequest.CustomerId);

                        if (existingCustomer != null)
                        {

                            EmailDto email = new EmailDto()
                            {

                                To = existingCustomer.Email,
                                Subject = "New request from customer",
                                Body = emailBodyForChecked(existingCustomer, depositOrder)

                            };

                            SendMail(email);

                        }
                        return true;
                    }
                    
                    
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateTransactionCode(int id, string transactionCode)
        {
            try
            {
                var depositOrder = _unitOfWork.DepositOrdersRepository.GetByID(id);
                if(depositOrder != null)
                {
                    var checkContract = _unitOfWork.ContractRepository.Find(c => c.RequestId == depositOrder.RequestId).FirstOrDefault();
                    if(checkContract == null) {
                        return false;
                    }
                    var checkRequest = _unitOfWork.RequestRepository.Find(r => r.Id == depositOrder.RequestId).FirstOrDefault();
                    if(checkRequest == null)
                    {
                        return false;
                    }
                    if(depositOrder.TransactionCode == null && checkContract.Status == 1 && checkRequest.Status == Requests.RequestsStatusEnum.SIGNED)
                    {
                        checkRequest.Status = Requests.RequestsStatusEnum.DEPOSITED;
                        _unitOfWork.RequestRepository.Update(checkRequest);
                        depositOrder.DepositDate = DateTime.Now;
                        depositOrder.TransactionCode = transactionCode;
                        depositOrder.Status = DepositOrders.DepositOrderStatusEnum.PROCESSING;
                        _unitOfWork.DepositOrdersRepository.Update(depositOrder);
                        _unitOfWork.Save();
                        var existingContractor = _unitOfWork.ContractorRepository.GetByID(checkRequest.ContractorId);

                        if (existingContractor != null)
                        {

                            EmailDto email = new EmailDto()
                            {

                                To = existingContractor.Email,
                                Subject = "New request from customer",
                                Body = emailBodyForPaying(existingContractor, depositOrder)

                            };

                            SendMail(email);

                        }
                        return true;
                    }
                    
                }
                
                return false;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string emailBodyForChecked(Customers customer, DepositOrders deposit)
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
        <h1>Contractor checked your deposit</h1>
        <p>Dear {customer.Name},</p>
        <p>Your deposit order checked successfully</p>        
        <p>Deposit price: {deposit.DepositPrice} VNĐ</p>        
        <p>Deposit date: {deposit.DepositDate}</p>
        <p>Transaction Code: {deposit.TransactionCode}</p>

        

        

        <p>Best regards,<br/>[Admin from ICQS]</p>
    </div>
</body>
</html>
";
        }
        public string emailBodyForPaying(Contractors contractor, DepositOrders deposit)
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
        <h1>New deposit order from customer</h1>
        <p>Dear {contractor.Name},</p>
        <p>You have a new deposit order from customer</p>        
        <p>Deposit price: {deposit.DepositPrice}</p>        
        <p>Deposit date: {deposit.DepositDate}</p>
        <p>Transaction Code: {deposit.TransactionCode}</p>

        

        <p>Please check deposit order.</p></br></br>

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
            }catch
            
            {
                throw new Exception("Error while send mail");
            }
            
        }

        public IEnumerable<DepositOrders>? GetDepositOrdersByContractorId(int ContractorId)
        {
            try
            {
                List<DepositOrders> depositOrders = new List<DepositOrders>();
                var Requests = _unitOfWork.RequestRepository.Find(r => r.ContractorId == ContractorId).ToList();
                if (Requests != null)
                {
                    if (Requests.Any())
                    {
                        foreach (var Request in Requests)
                        {
                            var depositOrder = _unitOfWork.DepositOrdersRepository.Find(d => d.RequestId == Request.Id).FirstOrDefault();
                            if (depositOrder != null)
                            {
                                depositOrders.Add(depositOrder);
                            }
                        }
                    }
                }
                return depositOrders;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
