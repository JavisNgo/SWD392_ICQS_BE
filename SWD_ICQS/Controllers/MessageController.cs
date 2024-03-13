using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using System.Diagnostics.Contracts;
using System.Text;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _imagesDirectory;

        public MessageController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "messageImage");
        }

        //[HttpGet("/messages")]
        //public async Task<IActionResult> getAllMesssage()
        //{
        //    try
        //    {
        //        var messages = unitOfWork.MessageRepository.Get();
        //        return Ok(messages);
        //    }catch (Exception ex)
        //    {
        //        throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
        //    }
        //}

        [HttpGet("/api/v1/messages/CustomerId={CustomerId}&ContractorId={ContractorId}")]
        public IActionResult getMesssageById(int CustomerId, int ContractorId)
        {
            try
            {
                var messages = unitOfWork.MessageRepository.Find(m => m.CustomerId == CustomerId && m.ContractorId == ContractorId);
                if(messages == null)
                {
                    return NotFound($"Message with id {CustomerId} not found");
                }
                
                foreach(var message in messages)
                {
                    if (!String.IsNullOrEmpty(message.ImageUrl))
                    {
                        message.ImageUrl = $"https://localhost:7233/img/messageImage/{message.ImageUrl}";
                    }
                }
                
                return Ok(messages);
                
            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the message. Error message: {ex.Message}");
            }
        }

        [HttpPost("/api/v1/messages/send")]
        public IActionResult AddMessage([FromBody] MessagesView messagesView)
        {
            try
            {
                var checkContractorID = unitOfWork.ContractorRepository.GetByID(messagesView.ContractorId);
                var checkCustomerID = unitOfWork.CustomerRepository.GetByID(messagesView.CustomerId);
                if(checkContractorID == null)
                {
                    return NotFound("Contractor not found");
                }
                if(checkCustomerID == null)
                {
                    return NotFound("Customer not found");
                }
                Messages message = _mapper.Map<Messages>(messagesView);
                
                message.SendAt = DateTime.Now;
                message.Status = true;
                string filename = null;
                string? tempString = message.ImageUrl;
                if (!String.IsNullOrEmpty(message.ImageUrl))
                {
                    string randomString = GenerateRandomString(15);
                    byte[] imageBytes = Convert.FromBase64String(message.ImageUrl);
                    filename = $"MessageImage_{message.CustomerId}_{message.CustomerId}_{randomString}.png";
                    string imagePath = Path.Combine(_imagesDirectory, filename);
                    System.IO.File.WriteAllBytes(imagePath, imageBytes);
                }
                message.ImageUrl = filename;

                unitOfWork.MessageRepository.Insert(message);
                
                unitOfWork.Save();
                return Ok(messagesView);
            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the message. Error message: {ex.Message}");
            }
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }

        //[HttpPut("/messageStatus/{id}")]        
        //public IActionResult ChangingStatusMessage(int id)
        //{
        //    try
        //    {
        //        var message = unitOfWork.MessageRepository.GetByID(id);
        //        if(message == null)
        //        {
        //            return NotFound($"Message with id {id} not found");
        //        }
        //            message.Status = false;
        //            unitOfWork.MessageRepository.Update(message);
        //            unitOfWork.Save();
        //            return Ok(new { Message = $"Message with ID {id} set status to false successfully." });
                
        //    }catch (Exception ex)
        //    {
        //        return BadRequest($"An error occurred while changing status the message. Error message: {ex.Message}");
        //    }
        //}

        //[HttpPut("/Message/{id}")]
        //public IActionResult AddImageForMessage(int id, IFormFile formFile)
        //{
        //    try
        //    {
        //        var existingMessage = unitOfWork.MessageRepository.GetByID(id);
        //        if(existingMessage == null)
        //        {
        //            return NotFound($"Message with ID {id} not found.");
        //        }
                
        //        // Check if a file is uploaded
        //        if (formFile != null && formFile.Length > 0)
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                formFile.CopyTo(memoryStream);
        //                existingMessage.ImageBin = memoryStream.ToArray();
        //            }
        //        }
                

        //        // Insert the new product into the database
        //        unitOfWork.MessageRepository.Update(existingMessage);
        //        unitOfWork.Save();
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"An error occurred while save the message. Error message: {ex.Message}");
        //    }
        //}



    }
}
