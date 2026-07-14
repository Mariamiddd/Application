using Core.Models;
using System.Collections.Generic;

namespace Application.InterfaceServices
{
    public interface IUserService
    {
        List<User> GetAll();
        User? GetById(int id);
        User? GetByEmail(string email);
        void Update(User user);
        void Delete(int id);
        bool ExistsByEmail(string email);
    }
    // to ensure that the user update model is immutable, we can use a record type instead of a class. This will make it easier to work with and ensure that the data is not accidentally modified.
    public record UserUpdateModel(string? FirstName = null, string? LastName = null, bool? IsVerified = null);
    public record UserModel(int Id, string Email, string? FirstName, string? LastName, bool IsVerified);
}
