using System.Threading.Tasks;

namespace ConfArch.IdentityProvider.Areas.Identity
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
