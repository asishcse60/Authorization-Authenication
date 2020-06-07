using ConfArch.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ConfArch.Web.Areas.Identity.IdentityHostingStartup))]
namespace ConfArch.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration
                            .GetConnectionString("IdentityDbContextConnection")));

                services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireNonAlphanumeric = false;
                    options.SignIn.RequireConfirmedAccount = true;
                }).AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();

                services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>,
                    ApplicationUserClaimsPrincipalFactory>();
                services.AddTransient<IEmailSender, AuthMessageSender>();
                services.AddTransient<ISmsSender, AuthMessageSender>();

                services.AddAuthentication()
                    .AddGoogle(o =>
                    {
                        o.ClientId =context.Configuration["Google:ClientId"];
                        o.ClientSecret = context.Configuration["Google:ClientSecret"];
                    });
            });
        }
    }
}