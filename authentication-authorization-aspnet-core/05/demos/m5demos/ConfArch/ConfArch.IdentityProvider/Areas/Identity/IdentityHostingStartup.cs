using ConfArch.IdentityProvider.Areas.Identity;
using ConfArch.IdentityProvider.Areas.Identity.Data;
using ConfArch.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]
namespace ConfArch.IdentityProvider.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<ConfArchWebContext>(options =>
                    options.UseSqlServer(
                        context.Configuration
                            .GetConnectionString("IdentityDbContextConnection")));
                //options.SignIn.RequireConfirmedAccount = true)
                services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                        {
                            options.SignIn.RequireConfirmedAccount = true;
                        }
                   )
                    .AddEntityFrameworkStores<ConfArchWebContext>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();

                services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>,
                    ApplicationUserClaimsPrincipalFactory>();
                services.AddTransient<IEmailSender, AuthMessageSender>();

                services.AddTransient<ISmsSender, AuthMessageSender>();

                services.AddAuthentication()
                    .AddGoogle(o =>
                    {
                        o.ClientId = context.Configuration["Google:ClientId"];
                        o.ClientSecret = context.Configuration["Google:ClientSecret"];
                    });
            });
        }
    }
}