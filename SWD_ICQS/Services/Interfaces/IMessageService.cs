using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Services.Interfaces
{
    public interface IMessageService
    {
        IEnumerable<Messages> getMesssageById(int CustomerId, int ContractorId);

        MessagesView AddMessage(MessagesView messagesView);
    }
}
