using Application.InterfaceServices;

namespace Application.InterfaceServices
{
    // Authentication related operations separated from IUserService
    public interface IAuthService
    {
        UserModel Register(RegistrationModel registrationModel);
        bool VerifyPassword(string email, string password);
        UserModel? Login(string email, string password);
    }

    public record RegistrationModel(string Email, string Password, string? FirstName = null, string? LastName = null);
}
