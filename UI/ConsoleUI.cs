using System;
using System.Threading.Tasks;
using Application.InterfaceServices;

namespace UI
{
    public class ConsoleUI
    {
        private readonly IAuthService _auth;
        // field to hold the current logged-in user, null if no user is logged in
        private Core.Models.User? _currentUser; 
        private readonly GuestMenu _guestMenu;
        private readonly AdminMenu _adminMenu;
        private readonly UserMenu _userMenu;

        // constructor with dependency injection for auth service, file manager, and optional console interface
        public ConsoleUI(IAuthService auth, Core.Interfaces.IFileManager fileManager, UI.Interfaces.IConsole? console = null) 
        {
            _auth = auth;  
            var c = console ?? new UI.Helpers.ConsoleWrapper();
            _guestMenu = new GuestMenu(_auth, c);
            _adminMenu = new AdminMenu(c, fileManager);
            _userMenu = new UserMenu(fileManager, c);
        }

        // main loop to run the console UI
        public async Task RunAsync()
        {
            while (true)
            {
                if (_currentUser == null)
                {
                    var user = await _guestMenu.ShowAsync();
                    if (user != null) _currentUser = user;
                }
                else
                {
                    // if current user is amin show admin menu, otherwise show user menu
                    if (_currentUser.Role == Core.Enums.Roles.Admin)
                    {
                        var logout = await _adminMenu.ShowAsync(_currentUser);
                        if (logout) _currentUser = null;
                    }
                    else
                    {
                        var logout = await _userMenu.ShowAsync(_currentUser);
                        if (logout) _currentUser = null;
                    }
                }
            }
        }


    }
}
