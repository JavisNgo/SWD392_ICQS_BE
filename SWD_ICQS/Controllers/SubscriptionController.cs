using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        //[Authorize(Policy = "RequireAdminOrContractorRole")]
        [HttpGet("/Subscriptions")]
        public async Task<IActionResult> getAllSubscriptions()
        {
            try
            {
                var subscriptionsList = _subscriptionService.getSubscriptions();
                return Ok(subscriptionsList);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }
        [AllowAnonymous]
        [HttpGet("/Subscriptions/{id}")]
        public IActionResult GetSubscriptionById(int id)
        {
            try
            {
                var subscription = _subscriptionService.GetSubscriptionById(id);

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
        [AllowAnonymous]
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

                var subscription = _subscriptionService.AddNewSubscription(subscriptionsView);

                if(subscription == null)
                {
                    return BadRequest("An error occurred while adding the subscription");
                }
                return Ok(subscription);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the subscription. Error message: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpPut("/Subscriptions/{id}")]
        public IActionResult UpdateSubscription(int id, [FromBody] SubscriptionsView subscriptionsView)
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

                var existingSubscription = _subscriptionService.UpdateSubscription(id, subscriptionsView);

                if (existingSubscription == null)
                {
                    return NotFound($"Subscription with ID {id} not found.");
                }

                // Map the properties from updatedCategoryView to existingCategory using AutoMapper
                

                return Ok(existingSubscription); // Return the updated Subscription
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the subscription. Error message: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpPut("/SubscriptionStatus/{id}")]
        public IActionResult ChangeStatusSubscriptions(int id)
        {
            try
            {
                var subscription = _subscriptionService.ChangeStatusSubscriptions(id);

                if (subscription == null)
                {
                    return NotFound($"Subscription with ID {id} not found.");
                }

                if(subscription.Status == false)
                {
                    return Ok("Set Status to false successfully.");
                } else
                {
                    return Ok("Set Status to true successfully.");
                }

                
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while changing status the Subscriptions. Error message: {ex.Message}");
            }
        }


        private bool IsValidName(string name)
        {
            // Use a regular expression to check if the name contains only letters and spaces
            return !string.IsNullOrWhiteSpace(name) && System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9\s]+$");
        }


    }
}
