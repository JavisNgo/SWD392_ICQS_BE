using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Diagnostics.Contracts;
using System.Text;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
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
                var messages = _messageService.getMesssageById(CustomerId, ContractorId);
                if(messages == null)
                {
                    return NotFound($"Message with id {CustomerId} not found");
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
                var message = _messageService.AddMessage(messagesView);
                
                return Ok(message);
            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the message. Error message: {ex.Message}");
            }
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
