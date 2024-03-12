using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public OrdersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("/Orders")]
        public async Task<IActionResult> getAllOrders()
        {
            try
            {
                var orderList = unitOfWork.OrderRepository.Get();
                return Ok(orderList);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }


        [AllowAnonymous]
        [HttpGet("/Orders/{id}")]
        public IActionResult GetOrderById(int id)
        {
            try
            {
                var order = unitOfWork. OrderRepository.GetByID(id);

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
        [HttpPost("/Orders")]
        public IActionResult AddOrders([FromBody] OrdersView ordersView)
        {
            try
            {
                var checkContractorId = unitOfWork.OrderRepository.GetByID(ordersView.ContractorId);
                var checkSubscriptionId = unitOfWork.OrderRepository.GetByID(ordersView.SubscriptionId);

                if (checkContractorId == null || checkSubscriptionId == null)
                {
                    return NotFound("ContractorId or SubscriptionId not found");
                }

                if (ordersView.OrderPrice < 0)
                {
                    return BadRequest("Price must be larger than 0");
                }
                var order = _mapper.Map<Orders>(ordersView);
                order.OrderDate = DateTime.Now;
                unitOfWork.OrderRepository.Insert(order);
                unitOfWork.Save();
                return Ok(ordersView);
            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the order. Error message: {ex.Message}");

            }
        }

        [AllowAnonymous]
        [HttpPut("/Orders/{id}")]
        public IActionResult UpdateOrders(int id, [FromBody] OrdersView ordersView)
        {
            try
            {
                var existingOrders = unitOfWork.OrderRepository.GetByID(id);

                if (existingOrders == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }
                var checkContractorId = unitOfWork.OrderRepository.GetByID(ordersView.ContractorId);
                var checkSubscriptionId = unitOfWork.OrderRepository.GetByID(ordersView.SubscriptionId);

                if (checkContractorId == null || checkSubscriptionId == null)
                {
                    return NotFound("ContractorId or SubscriptionId not found");
                }
                if (ordersView.OrderPrice < 0)
                {
                    return BadRequest("Price must be larger than 0");
                }

                _mapper.Map(ordersView, existingOrders);

                // Mark the entity as modified
                existingOrders.OrderDate = DateTime.Now;
                unitOfWork.OrderRepository.Update(existingOrders);
                unitOfWork.Save();

                return Ok(existingOrders); // Return 
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the order. Error message: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpPut("/OrderStatus/{id}")]
        public IActionResult ChangeStatusOrder(int id)
        {
            try
            {
                var order = unitOfWork.OrderRepository.GetByID(id);

                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                // Chỉ đặt thuộc tính Status là false thay vì xóa hoàn toàn
                order.Status = false;

                unitOfWork.OrderRepository.Update(order);
                unitOfWork.Save();

                return Ok("Set status to false successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while changing status the order. Error message: {ex.Message}");
            }
        }

    }
}
