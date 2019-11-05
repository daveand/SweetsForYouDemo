using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;


        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _emailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        [BindProperty]
        public CustomerModel Customer { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Du måste ange ett användarnamn")]
            [Display(Name = "Användarnamn")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Du måste ange en epostaddress")]
            [EmailAddress(ErrorMessage = "Du måste ange en korrekt epostaddress")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Ange ett lösenord")]
            [StringLength(100, ErrorMessage = "Lösenordet måste vara minst {2} och max {1} tecken långt.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Lösenord")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Bekräfta lösenord")]
            [Compare("Password", ErrorMessage = "Lösenorden stämmer inte överens.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(CustomerModel customerModel, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.UserName, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);


                    customerModel.UserId = user.Id;
                    customerModel.FirstName = Customer.FirstName;
                    customerModel.LastName = Customer.LastName;
                    customerModel.Telephone = Customer.Telephone;
                    customerModel.Company = Customer.Company;
                    customerModel.Department = Customer.Department;
                    customerModel.Street = Customer.Street;
                    customerModel.PostalCode = Customer.PostalCode;
                    customerModel.City = Customer.City;
                    customerModel.PersonalNumber = Customer.PersonalNumber;
                    customerModel.Participants = Customer.Participants;
                    customerModel.Receiver = Customer.Receiver;
                    customerModel.PayMethod = Customer.PayMethod;
                    customerModel.PayNumber = Customer.PayNumber;


                    _context.Add(customerModel);
                    await _context.SaveChangesAsync();

                    await _userManager.AddToRoleAsync(user, "User");

                    _emailService.SendUserConfirmation(Input.Email, "Sweets For You - Bekräfta din email-adress", 
                        $"Klicka <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>här</a> för att bekräfta din email-adress.");
                    _emailService.SendNewCustomerConfirmation(customerModel);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    //return LocalRedirect(returnUrl);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
