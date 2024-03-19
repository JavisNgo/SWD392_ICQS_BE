using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Services.Implements;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositOrdersController : ControllerBase
    {
        private readonly IDepositOrdersService _depositOrdersService;

        public DepositOrdersController(IDepositOrdersService depositOrdersService)
        {
            _depositOrdersService = depositOrdersService;
        }

        [HttpGet("/api/v1/depositOrders/get")]
        public async Task<IActionResult> GetAllDepositOrders()
        {
            try
            {

                var depositOrders = _depositOrdersService.GetDepositOrders();
                if(depositOrders == null)
                {
                    return BadRequest("Cannot get orders");
                }
                if(!depositOrders.Any())
                {
                    return NotFound("No order found in platform");
                }

                return Ok(depositOrders);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [HttpGet("/api/v1/depositOrders/get/customerid={customerid}")]
        public async Task<IActionResult> GetAllDepositOrdersByCustomerId(int customerid)
        {
            try
            {

                var depositOrders = _depositOrdersService.GetDepositOrdersByCustomerId(customerid);
                if (depositOrders == null)
                {
                    return BadRequest("Cannot get orders");
                }
                if (!depositOrders.Any())
                {
                    return NotFound($"No order of customer with id {customerid} found in platform");
                }

                return Ok(depositOrders);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [HttpGet("/api/v1/depositOrders/get/requestid={requestid}")]
        public async Task<IActionResult> GetDepositOrderByRequestId(int requestid)
        {
            try
            {

                var depositOrder = _depositOrdersService.GetDepositOrdersByRequestId(requestid);
                if (depositOrder == null)
                {
                    return BadRequest("This request don't have any order");
                }

                return Ok(depositOrder);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [HttpGet("/api/v1/depositOrders/get/id={id}")]
        public async Task<IActionResult> GetDepositOrderById(int id)
        {
            try
            {

                var depositOrder = _depositOrdersService.GetDepositOrdersById(id);
                if (depositOrder == null)
                {
                    return BadRequest($"This order with id {id} doesn't existed");
                }

                return Ok(depositOrder);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [HttpPut("/api/v1/depositOrders/update/{id}")]
        public async Task<IActionResult> UpdateTransactionCode(int id, string transactionCode)
        {
            try
            {
                var depositOrder = _depositOrdersService.UpdateTransactionCode(id, transactionCode);
                if(depositOrder == true)
                {
                    return Ok();
                }
                return BadRequest("Error, contract not uploaded and signed or contract not found or request not found!");
            }catch(Exception ex)
            {
                throw new Exception($"An error occurred while update.ErrorMessage:{ex}");
            }
        }

        [HttpPut("/api/v1/depositOrders/updateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            try
            {
                var depositCheck = _depositOrdersService.UpdateStatus(id);
                if(depositCheck == true)
                {
                    return Ok();
                }
                return BadRequest("Error!");
            }catch(Exception ex)
            {
                throw new Exception($"An error occurred while update.ErrorMessage:{ex}");
            }
        }
        
    }
}
