using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private IEmailService _emailService;

        public HomeController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Information()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        public async Task<IActionResult> ContactMessage([Bind("FirstName,LastName,Email,Message")] ContactFormModel contactForm)
        {
            if (ModelState.IsValid)
            {
                await _emailService.SendContactMessage(contactForm);
                return RedirectToAction("ContactMessageConfirmation");
            } else
            {
                return View();
            }

            
        }
        public IActionResult ContactMessageConfirmation()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
