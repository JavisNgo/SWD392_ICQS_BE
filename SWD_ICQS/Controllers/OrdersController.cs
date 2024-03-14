using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using System.Text;

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
        [HttpGet("/api/v1/orders/get")]
        public async Task<IActionResult> getAllOrders()
        {
            try
            {
                var orderList = unitOfWork.OrderRepository.Get();

                if(!orderList.Any())
                {
                    return NotFound("No one ordered yet");
                }

                List<OrdersView> ordersViews = new List<OrdersView>();

                foreach (var item in orderList)
                {
                    ordersViews.Add(_mapper.Map<OrdersView>(item));
                }
                return Ok(ordersViews);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/orders/get/id={id}")]
        public IActionResult GetOrderById(int id)
        {
            try
            {
                var order = unitOfWork. OrderRepository.GetByID(id);

                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                var orderView = _mapper.Map<OrdersView>(order);

                return Ok(orderView);
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
                var order = unitOfWork.OrderRepository.Find(o => o.ContractorId == contractorid).ToList();

                if (order == null)
                {
                    return NotFound($"Order with Contractor ID {contractorid} not found.");
                }

                List<OrdersView> ordersViews = new List<OrdersView>();

                foreach (var item in order)
                {
                    ordersViews.Add(_mapper.Map<OrdersView>(item));
                }

                return Ok(ordersViews);
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
                var checkContractorId = unitOfWork.ContractorRepository.GetByID(ordersView.ContractorId);
                var checkSubscriptionId = unitOfWork.SubscriptionRepository.GetByID(ordersView.SubscriptionId);

                if (checkContractorId == null || checkSubscriptionId == null)
                {
                    return NotFound("ContractorId or SubscriptionId not found");
                }

                if (ordersView.OrderPrice < 0)
                {
                    return BadRequest("Price must be larger than 0");
                }

                var order = new Orders
                {
                    SubscriptionId = ordersView.SubscriptionId,
                    ContractorId = ordersView.ContractorId,
                    OrderPrice = checkSubscriptionId.Price,
                    OrderDate = DateTime.Now,
                    Status = true,
                    TransactionCode = GenerateRandomCode(10)
                };

                unitOfWork.OrderRepository.Insert(order);
                unitOfWork.Save();

                checkContractorId.SubscriptionId = ordersView.SubscriptionId;
                checkContractorId.ExpiredDate = DateTime.Now.AddDays((double) checkSubscriptionId.Duration);
                unitOfWork.ContractorRepository.Update(checkContractorId); 
                unitOfWork.Save();

                return Ok("Order successfully");
            }catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the order. Error message: {ex.Message}");

            }
        }

        public static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
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
