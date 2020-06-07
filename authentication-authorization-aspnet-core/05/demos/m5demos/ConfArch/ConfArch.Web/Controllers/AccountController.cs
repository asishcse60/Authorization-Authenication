using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ConfArch.Web.Controllers
{
    public class AccountController: Controller
    {
        public IActionResult Login() => Challenge(new AuthenticationProperties { RedirectUri = "/" });
        public IActionResult AccessDenied() => View();
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}
