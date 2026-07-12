using Core.Models;
using Application.InterfaceServices;

namespace Application.InterfaceServices
{
    
    public interface IAuthService
    {
        User Register(string email, string password, string firstName, string lastName);
        User? Login(string email, string password);
        bool VerifyPassword(string email, string password);

    }
}
