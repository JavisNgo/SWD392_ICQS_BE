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
    public class SubscriptionController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public SubscriptionController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [Authorize(Policy = "RequireAdminOrContractorRole")]
        [HttpGet("/Subscriptions")]
        public async Task<IActionResult> getAllSubscriptions()
        {
            try
            {
                var subscriptionsList = unitOfWork.SubscriptionRepository.Get();
                return Ok(subscriptionsList);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }
        [Authorize]
        [HttpGet("/Subscriptions/{id}")]
        public IActionResult GetSubscriptionById(int id)
        {
            try
            {
                var subscription = unitOfWork.SubscriptionRepository.GetByID(id);

                if (subscription == null)
                {
                    return NotFound($"Subscription with ID {id} not found.");
                }

                return Ok(subscription);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting the subscription. Error message: {ex.Message}");
            }
        }
        [Authorize]
        [HttpPost("/Subscriptions")]
        public IActionResult AddSubscription([FromBody] SubscriptionsView subscriptionsView)
        {
            try
            {
                // Validate the name using a regular expression
                if (!IsValidName(subscriptionsView.Name))
                {
                    return BadRequest("Invalid name. It should only contain letters and spaces.");
                }

                // Validate the Price
                if (subscriptionsView.Price.HasValue && subscriptionsView.Price < 0)
                {
                    return BadRequest("Invalid Price. It should be a non-negative number.");
                }

                // Validate the Duration
                if (subscriptionsView.Duration.HasValue && subscriptionsView.Duration <= 0)
                {
                    return BadRequest("Invalid Duration. It should be a positive integer.");
                }

                var subscription = _mapper.Map<Subscriptions>(subscriptionsView);
                unitOfWork.SubscriptionRepository.Insert(subscription);
                unitOfWork.Save();

                return Ok(subscriptionsView);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the subscription. Error message: {ex.Message}");
            }
        }
        [Authorize]
        [HttpPut("/Subscriptions/{id}")]
        public IActionResult UpdateSubscription(int id, [FromBody] SubscriptionsView subscriptionsView)
        {
            try
            {
                var existingSubscription = unitOfWork.SubscriptionRepository.GetByID(id);

                if (existingSubscription == null)
                {
                    return NotFound($"Subscription with ID {id} not found.");
                }

                // Validate the name using a regular expression
                if (!IsValidName(subscriptionsView.Name))
                {
                    return BadRequest("Invalid name. It should only contain letters and spaces.");
                }

                // Validate the Price
                if (subscriptionsView.Price.HasValue && subscriptionsView.Price < 0)
                {
                    return BadRequest("Invalid Price. It should be a non-negative number.");
                }

                // Validate the Duration
                if (subscriptionsView.Duration.HasValue && subscriptionsView.Duration <= 0)
                {
                    return BadRequest("Invalid Duration. It should be a positive integer.");
                }

                // Map the properties from updatedCategoryView to existingCategory using AutoMapper
                _mapper.Map(subscriptionsView, existingSubscription);

                // Mark the entity as modified
                unitOfWork.SubscriptionRepository.Update(existingSubscription);
                unitOfWork.Save();

                return Ok(existingSubscription); // Return the updated Subscription
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the subscription. Error message: {ex.Message}");
            }
        }
        [Authorize]
        [HttpDelete("/Subscriptions/{id}")]
        public IActionResult DeleteSubscription(int id)
        {
            try
            {
                var subscriptions = unitOfWork.SubscriptionRepository.GetByID(id);

                if (subscriptions == null)
                {
                    return NotFound($"Subscription with ID {id} not found.");
                }

                unitOfWork.SubscriptionRepository.Delete(id);
                unitOfWork.Save();

                // You can return a custom response message 
                return Ok(new { Message = $"Subscription with ID {id} has been successfully deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the subscription. Error message: {ex.Message}");
            }
        }


        private bool IsValidName(string name)
        {
            // Use a regular expression to check if the name contains only letters and spaces
            return !string.IsNullOrWhiteSpace(name) && System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z\s]+$");
        }


    }
}
