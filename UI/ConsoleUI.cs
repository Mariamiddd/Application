using System;
using Application.InterfaceServices;

namespace UI
{
    public class ConsoleUI
    {
        private readonly IAuthService _auth;
        private Core.Models.User? _currentUser; //storing current domain user in memory (polymorphic)
        private readonly GuestMenu _guestMenu;
        private readonly AdminMenu _adminMenu;
        private readonly UserMenu _userMenu;

        public ConsoleUI(IAuthService auth, Core.Interfaces.IFileManager fileManager, UI.Interfaces.IConsole? console = null) 
        {
            _auth = auth;
            var c = console ?? new UI.Helpers.ConsoleWrapper();
            _guestMenu = new GuestMenu(_auth, c);
            _adminMenu = new AdminMenu(c);
            _userMenu = new UserMenu(fileManager, c);
        }

        public void Run()
        {
            while (true)
            {
                if (_currentUser == null)
                {
                    var user = _guestMenu.Show();
                    if (user != null) _currentUser = user;
                }
                else
                {
                    // route based on role using polymorphism
                    if (_currentUser.Role == Core.Enums.Roles.Admin)
                    {
                        var logout = _adminMenu.Show(_currentUser);
                        if (logout) _currentUser = null;
                    }
                    else
                    {
                        var logout = _userMenu.Show(_currentUser);
                        if (logout) _currentUser = null;
                    }
                }
            }
        }


    }
}