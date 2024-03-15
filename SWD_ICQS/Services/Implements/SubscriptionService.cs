using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Services.Implements
{
    public class SubscriptionService : ISubscriptionService
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public SubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public SubscriptionsView AddNewSubscription(SubscriptionsView subscriptionsView)
        {
            try
            {


                var subscription = _mapper.Map<Subscriptions>(subscriptionsView);
                if(subscription != null)
                {
                    unitOfWork.SubscriptionRepository.Insert(subscription);
                    unitOfWork.Save();
                    return subscriptionsView;
                }
                else
                {
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the subscription. Error message: {ex.Message}");
            }
        }

        public Subscriptions GetSubscriptionById(int id)
        {
            try
            {
                var subscription = unitOfWork.SubscriptionRepository.GetByID(id);

                if (subscription == null)
                {
                    return null;
                }

                return subscription;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the subscription. Error message: {ex.Message}");
            }
        }

        public IEnumerable<Subscriptions> getSubscriptions()
        {
            try
            {
                var subscriptionsList = unitOfWork.SubscriptionRepository.Get().ToList();
                return subscriptionsList;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
            }
        }

        public SubscriptionsView UpdateSubscription(int id, SubscriptionsView subscriptionsView)
        {
            try
            {
                var existingSubscription = unitOfWork.SubscriptionRepository.GetByID(id);

                if (existingSubscription != null)
                {
                    // Map the properties from updatedCategoryView to existingCategory using AutoMapper
                    _mapper.Map(subscriptionsView, existingSubscription);

                    // Mark the entity as modified
                    unitOfWork.SubscriptionRepository.Update(existingSubscription);
                    unitOfWork.Save();

                    return subscriptionsView; // Return the updated Subscription
                }
                else
                {
                    return null;
                }

                
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the subscription. Error message: {ex.Message}");
            }
        }

        public Subscriptions ChangeStatusSubscriptions(int id)
        {
            try
            {
                var subscription = unitOfWork.SubscriptionRepository.GetByID(id);

                if (subscription == null)
                {
                    return null;
                }

                if (subscription.Status == true)
                {
                    // Chỉ đặt thuộc tính Status là false thay vì xóa hoàn toàn
                    subscription.Status = false;
                    unitOfWork.SubscriptionRepository.Update(subscription);
                    unitOfWork.Save();
                    return subscription;
                }
                else
                {
                    // Chỉ đặt thuộc tính Status là false thay vì xóa hoàn toàn
                    subscription.Status = true;
                    unitOfWork.SubscriptionRepository.Update(subscription);
                    unitOfWork.Save();
                    return subscription;
                }


            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while changing status the Subscriptions. Error message: {ex.Message}");
            }
        }
    }
}
