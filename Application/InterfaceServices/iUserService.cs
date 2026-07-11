using System.Collections.Generic;

namespace Application.InterfaceServices
{
    // IUserService: handles user data operations (CRUD). Authentication (login, tokens)
    // should be placed in a separate IAuthService.
    public interface IUserService
    {
        IReadOnlyList<UserModel> GetAll();
        UserModel? GetById(int id);
        UserModel? GetByEmail(string email);
        void Update(int id, UserUpdateModel dto);
        void Delete(int id);
        bool ExistsByEmail(string email);
    }
    // Models for IUserService. Keep lightweight and avoid exposing password hashes in public models.
    public record UserUpdateModel(string? FirstName = null, string? LastName = null, bool? IsVerified = null);
    public record UserModel(int Id, string Email, string? FirstName, string? LastName, bool IsVerified);
}
