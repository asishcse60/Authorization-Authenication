using System.Threading.Tasks;

namespace ConfArch.IdentityProvider
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
