using Core.Enums;

namespace Core.Models
{
    // user class with properties for user details for login and registration and user role (admin or client)
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Roles Role { get; set; }

        // method to displayed menu that can be overridden in derived classes (polymorphism) in diffeerent user roles (admin or client)
        public virtual void DisplayMenu()
        {
            // overridden in derived classes in Admin , Guest, Client classes
        }
    }
}