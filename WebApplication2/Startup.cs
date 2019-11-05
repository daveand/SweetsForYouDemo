using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication2.Models;
using WebApplication2.Interfaces;
using WebApplication2.Repositories;
using WebApplication2.Services;
using System.Net.Mail;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using WebApplication2.Helpers;
using WebApplication2.Services.PDFService;
using System.IO;
using System.Reflection;
using RazorLight;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WebApplication2
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AzureConnection")));
            //services.AddDefaultIdentity<IdentityUser> ()
            //    .AddDefaultUI(UIFramework.Bootstrap4)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddIdentity<IdentityUser, IdentityRole>()
                 .AddDefaultUI(UIFramework.Bootstrap4)
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<UserDisplayService>();
            services.AddScoped(sp => ShoppingCart.GetCart(sp));
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<SmtpClient>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMemoryCache();
            services.AddSession();

            services.AddScoped<IRazorLightEngine>(sp =>
            {
                var engine = new RazorLightEngineBuilder()
                    .UseFilesystemProject(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                    .UseMemoryCachingProvider()
                    .Build();
                return engine;
            });

            var processSufix = "32bit";
            if (Environment.Is64BitProcess && IntPtr.Size == 8)
            {
                processSufix = "64bit";
            }
            var assemblyLoadContext = new CustomAssemblyLoadContext();
            var contentRoot = _env.ContentRootPath;
            assemblyLoadContext.LoadUnmanagedLibrary(contentRoot + $"\\wwwroot\\PDFNative\\{processSufix}\\libwkhtmltox.dll");

            services.AddScoped<IPDFService, PDFService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            var cultureInfo = new CultureInfo("sv-SE");
            cultureInfo.NumberFormat.CurrencySymbol = "kr";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //Run this method to create roles and assign users to role.
            //CreateUserRoles(services).Wait();
        }

        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            IdentityResult roleResult;
            //Adding Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("User");
            if (!roleCheck)
            {
                //create the roles and seed them to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("User"));
            }
            //Assign Admin role to the main User here we have given our newly registered 
            //login id for Admin management
            IdentityUser user = await UserManager.FindByEmailAsync("user@user.com");
            var User = new IdentityUser();
            await UserManager.AddToRoleAsync(user, "User");
        }
    }
}
