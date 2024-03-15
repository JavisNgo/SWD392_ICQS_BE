using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Interfaces;
using System.Text;

namespace SWD_ICQS.Services.Implements
{
    public class MessageService : IMessageService
    {
        private IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _imagesDirectory;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "messageImage");
        }
        public MessagesView AddMessage(MessagesView messagesView)
        {
            try
            {
                var checkContractorID = unitOfWork.ContractorRepository.GetByID(messagesView.ContractorId);
                var checkCustomerID = unitOfWork.CustomerRepository.GetByID(messagesView.CustomerId);
                if (checkContractorID == null)
                {
                    throw new Exception("Contractor not found");
                }
                if (checkCustomerID == null)
                {
                    throw new Exception("Customer not found");
                }
                Messages message = _mapper.Map<Messages>(messagesView);

                message.SendAt = DateTime.Now;
                message.Status = true;
                string filename = null;
                string? tempString = message.ImageUrl;
                if (!String.IsNullOrEmpty(message.ImageUrl))
                {
                    string randomString = GenerateRandomString(15);
                    byte[] imageBytes = Convert.FromBase64String(message.ImageUrl);
                    filename = $"MessageImage_{message.CustomerId}_{message.CustomerId}_{randomString}.png";
                    string imagePath = Path.Combine(_imagesDirectory, filename);
                    System.IO.File.WriteAllBytes(imagePath, imageBytes);
                }
                message.ImageUrl = filename;

                unitOfWork.MessageRepository.Insert(message);

                unitOfWork.Save();
                return messagesView;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the message. Error message: {ex.Message}");
            }
        }
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }

        public IEnumerable<Messages> getMesssageById(int CustomerId, int ContractorId)
        {
            try
            {
                var messages = unitOfWork.MessageRepository.Find(m => m.CustomerId == CustomerId && m.ContractorId == ContractorId);
                if (messages == null)
                {
                    return null;
                }

                foreach (var message in messages)
                {
                    if (!String.IsNullOrEmpty(message.ImageUrl))
                    {
                        message.ImageUrl = $"https://localhost:7233/img/messageImage/{message.ImageUrl}";
                    }
                }
                return messages;

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the message. Error message: {ex.Message}");
            }
        }
    }
}
