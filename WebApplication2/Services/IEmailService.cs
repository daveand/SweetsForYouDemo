using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IEmailService
    {
        Task<string> SendOrderConfirmation(OrderModel order);
        Task<string> SendDeliverConfirmation(CustomerModel customer, int orderTotal);
        void SendNewCustomerConfirmation(CustomerModel customer);
        Task<string> SendContactMessage(ContactFormModel contactForm);
        void SendUserConfirmation(string email, string subject, string confirmString);
        void SendResetPassword(string email, string subject, string body);

    }
}