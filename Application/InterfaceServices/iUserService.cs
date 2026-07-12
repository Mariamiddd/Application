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
}
