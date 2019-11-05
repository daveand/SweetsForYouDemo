using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class EmailService : IEmailService
    {
        SmtpClient _client;
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmailService(SmtpClient client, IHostingEnvironment hostingEnvironment)
        {
            _client = client;
            _hostingEnvironment = hostingEnvironment;

            _client = new SmtpClient("smtp01.binero.se");
            _client.Port = 587;
            _client.EnableSsl = true;
            _client.UseDefaultCredentials = false;
            _client.Credentials = new NetworkCredential("noreply@sweetsforyou.se", "NoReply!1234");


        }

        private void SendEmail(string toEmail, string subject, string body, string userToken)
        {

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@sweetsforyou.se");
            mailMessage.To.Add(toEmail);
            mailMessage.Body = body;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            _client.Send(mailMessage);
        }

        public void SendUserConfirmation(string email, string subject, string confirmString)
        {
            string userToken = "UserConfirmation";
            string toEmail = email;
            string webRootPath = _hostingEnvironment.WebRootPath;
            string body = System.IO.File.ReadAllText(webRootPath + "/templates/NewCustomerMail.html");
            body = body.Replace("#Confirm#", confirmString);
            //body = body.Replace("#LastName#", order.LastName);
            //body = body.Replace("#OrderNumber#", order.Id.ToString());
            //body = body.Replace("#SumTotal#", order.OrderTotal.ToString());
            SendEmail(toEmail, subject, body, userToken);
        }

        public void SendResetPassword(string email, string subject, string body)
        {
            string userToken = "ResetPassword";
            string toEmail = email;
            SendEmail(toEmail, subject, body, userToken);
        }



        public async Task<string> SendOrderConfirmation(OrderModel order)
        {
            string userToken = "OrderConfirmation";
            string toEmail = order.User.Email;
            string subject = "Orderbekräftelse";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string body = await System.IO.File.ReadAllTextAsync(webRootPath + "/templates/OrderConfirmation.html");
            body = body.Replace("#FirstName#", order.FirstName);
            body = body.Replace("#LastName#", order.LastName);
            body = body.Replace("#OrderNumber#", order.Id.ToString());
            body = body.Replace("#SumTotal#", order.OrderTotal.ToString());
            SendEmail(toEmail, subject, body, userToken);
            return null;
        }

        public async Task<string> SendDeliverConfirmation(CustomerModel customer, int orderTotal)
        {
            string userToken = "DeliverConfirmation";
            string toEmail = customer.User.Email;
            string subject = "Leveransbekräftelse";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string body = await System.IO.File.ReadAllTextAsync(webRootPath + "/templates/DeliverConfirmation.html");
            body = body.Replace("#FirstName#", customer.FirstName);
            body = body.Replace("#LastName#", customer.LastName);
            body = body.Replace("#SumTotal#", orderTotal.ToString());
            SendEmail(toEmail, subject, body, userToken);
            return null;
        }

        public void SendNewCustomerConfirmation(CustomerModel customer)
        {
            string userToken = "NewCustomerConfirmation";
            string toEmail = "info@sweetsforyou.se";
            string subject = "Ny kund!";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string body = System.IO.File.ReadAllText(webRootPath + "/templates/NewCustomerConfirmation.html");
            body = body.Replace("#Company#", customer.Company);
            body = body.Replace("#Department#", customer.Department);
            body = body.Replace("#Participants#", customer.Participants.ToString());

            SendEmail(toEmail, subject, body, userToken);
        }

        public async Task<string> SendContactMessage(ContactFormModel contactForm)
        {
            string userToken = "ContactMessage";
            string toEmail = "info@sweetsforyou.se";
            string subject = "Nytt meddelande från " + contactForm.FirstName + " " + contactForm.LastName;
            string webRootPath = _hostingEnvironment.WebRootPath;
            string body = await System.IO.File.ReadAllTextAsync(webRootPath + "/templates/ContactForm.html");
            body = body.Replace("#FirstName#", contactForm.FirstName);
            body = body.Replace("#LastName#", contactForm.LastName);
            body = body.Replace("#Email#", contactForm.Email);
            body = body.Replace("#Message#", contactForm.Message);

            SendEmail(toEmail, subject, body, userToken);
            return null;
        }

    }
}
