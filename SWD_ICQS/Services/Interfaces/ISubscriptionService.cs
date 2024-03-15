using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface ISubscriptionService
    {
        IEnumerable<Subscriptions> getSubscriptions();
        Subscriptions GetSubscriptionById(int id);
        SubscriptionsView AddNewSubscription(SubscriptionsView subscriptionsView);
        SubscriptionsView UpdateSubscription(int id, SubscriptionsView subscriptionsView);

        Subscriptions ChangeStatusSubscriptions(int id);
    }
}
