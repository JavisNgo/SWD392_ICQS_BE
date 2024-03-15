using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Text;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {

        private readonly IOrdersService _orderService;

        public OrdersController(IOrdersService ordersService)
        {
            _orderService = ordersService;
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/orders/get")]
        public async Task<IActionResult> getAllOrders()
        {
            try
            {
                var orderList = _orderService.getOrders();

                if(!orderList.Any())
                {
                    return NotFound("No one ordered yet");
                }

                
                return Ok(orderList);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }
        //[AllowAnonymous]
        //[HttpGet("/api/v1/orders/contractor/{contractorId}")]
        //public ActionResult<IEnumerable<OrdersView>> GetOrdersByContractorId(int contractorId)
        //{
        //    try
        //    {
        //        var orders = unitOfWork.OrderRepository.Get(filter: o => o.ContractorId == contractorId).ToList();
        //        var ordersViews = new List<OrdersView>();

        //        foreach (var order in orders)
        //        {
        //            var orderView = new OrdersView
        //            {
        //                SubscriptionId = order.SubscriptionId,
        //                ContractorId = order.ContractorId,
        //                OrderPrice = order.OrderPrice,
        //                OrderDate = order.OrderDate,
        //                Status = order.Status,
        //                TransactionCode = order.TransactionCode
        //            };

        //            ordersViews.Add(orderView);
        //        }

        //        return Ok(ordersViews);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
        [AllowAnonymous]
        [HttpGet("/api/v1/orders/get/id={id}")]
        public IActionResult GetOrderById(int id)
        {
            try
            {
                var order = _orderService.GetOrderById(id);

                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the order. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/orders/get/contractorid={contractorid}")]
        public IActionResult GetOrderByContractorId(int contractorid)
        {
            try
            {
                var order = _orderService.GetOrdersByContractorId(contractorid);

                if (order == null)
                {
                    return NotFound($"Order with Contractor ID {contractorid} not found.");
                }

                

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the order. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/orders/post")]
        public IActionResult AddOrders([FromBody] OrdersView ordersView)
        {
            try
            {
                if (ordersView.OrderPrice < 0)
                {
                    return BadRequest("Price must be larger than 0");
                }
                var order = _orderService.AddOrder(ordersView);
                if(order != null)
                {
                    return Ok("Order successfully");
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the order. Error message: {ex.Message}");

            }
        }

        

        //[AllowAnonymous]
        //[HttpPut("/Orders/{id}")]
        //public IActionResult UpdateOrders(int id, [FromBody] OrdersView ordersView)
        //{
        //    try
        //    {
        //        var existingOrders = unitOfWork.OrderRepository.GetByID(id);

        //        if (existingOrders == null)
        //        {
        //            return NotFound($"Order with ID {id} not found.");
        //        }
        //        var checkContractorId = unitOfWork.ContractorRepository.GetByID(ordersView.ContractorId);
        //        var checkSubscriptionId = unitOfWork.SubscriptionRepository.GetByID(ordersView.SubscriptionId);

        //        if (checkContractorId == null || checkSubscriptionId == null)
        //        {
        //            return NotFound("ContractorId or SubscriptionId not found");
        //        }
        //        if (ordersView.OrderPrice < 0)
        //        {
        //            return BadRequest("Price must be larger than 0");
        //        }

        //        _mapper.Map(ordersView, existingOrders);

        //        // Mark the entity as modified
        //        existingOrders.OrderDate = DateTime.Now;
        //        unitOfWork.OrderRepository.Update(existingOrders);
        //        unitOfWork.Save();

        //        return Ok(ordersView); // Return 
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"An error occurred while updating the order. Error message: {ex.Message}");
        //    }
        //}


        //[AllowAnonymous]
        //[HttpPut("/OrderStatus/{id}")]
        //public IActionResult ChangeStatusOrder(int id)
        //{
        //    try
        //    {
        //        var order = unitOfWork.OrderRepository.GetByID(id);

        //        if (order == null)
        //        {
        //            return NotFound($"Order with ID {id} not found.");
        //        }

        //        // Chỉ đặt thuộc tính Status là false thay vì xóa hoàn toàn
        //        order.Status = false;

        //        unitOfWork.OrderRepository.Update(order);
        //        unitOfWork.Save();

        //        return Ok("Set status to false successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"An error occurred while changing status the order. Error message: {ex.Message}");
        //    }
        //}

    }
}
