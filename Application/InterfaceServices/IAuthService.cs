using Core.Models;
using Application.InterfaceServices;
using Core.Enums;
using System.Threading.Tasks;

namespace Application.InterfaceServices
{

    public interface IAuthService
    {
        // register, login objects, verify password bool
        Task<User> RegisterAsync(string email, string password, string firstName, string lastName, Roles role = Roles.User);
        Task<User?> LoginAsync(string email, string password);
        Task<bool> VerifyPasswordAsync(string email, string password);

    }
}
