﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public MessageController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("/messages")]
        public async Task<IActionResult> getAllMesssage()
        {
            try
            {
                var messages = unitOfWork.MessageRepository.Get();
                return Ok(messages);
            }catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }
        [HttpGet("/message/{id}")]
        public IActionResult getMesssageById(int id)
        {
            try
            {
                var message = unitOfWork.MessageRepository.GetByID(id);
                if(message == null)
                {
                    return NotFound($"Message with id {id} not found");
                }
                
                    return Ok(message);
                
            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the message. Error message: {ex.Message}");
            }
        }

        [HttpPost("/messages")]
        public IActionResult AddMessage([FromBody] MessagesView messagesView)
        {
            try
            {
                Messages message = _mapper.Map<Messages>(messagesView);
                
                message.SendAt = DateTime.Now;
                message.Status = true;
                unitOfWork.MessageRepository.Insert(message);
                
                unitOfWork.Save();
                return Ok(messagesView);
            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the message. Error message: {ex.Message}");
            }
        }
        [HttpDelete("/messages/{id}")]        
        public IActionResult DeleteMessage(int id)
        {
            try
            {
                var message = unitOfWork.MessageRepository.GetByID(id);
                if(message == null)
                {
                    return NotFound($"Message with id {id} not found");
                }
                
                    unitOfWork.MessageRepository.Delete(id);
                    unitOfWork.Save();
                    return Ok(new { Message = $"Message with ID {id} has been successfully deleted." });
                
            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the message. Error message: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult AddImageForMessage(int id, IFormFile formFile)
        {
            try
            {
                var existingMessage = unitOfWork.MessageRepository.GetByID(id);
                if(existingMessage == null)
                {
                    return NotFound($"Message with ID {id} not found.");
                }
                
                // Check if a file is uploaded
                if (formFile != null && formFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        formFile.CopyTo(memoryStream);
                        existingMessage.ImageBin = memoryStream.ToArray();
                    }
                }
                

                // Insert the new product into the database
                unitOfWork.MessageRepository.Update(existingMessage);
                unitOfWork.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while save the message. Error message: {ex.Message}");
            }
        }



    }
}
