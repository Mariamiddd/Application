using System;
using Core.Interfaces;
using Repository.Data;
using Application.Services;
using Application.InterfaceServices;

namespace UI
{
    class Program
    {
        static void Main(string[] args)
        {
            IFileManager repository = new FileRepository();
            IAuthService authService = new AuthService(repository);

            var ui = new ConsoleUI(authService, repository, new UI.Helpers.ConsoleWrapper());
            var app = new App(ui);
            app.Run();
        }
    }
}


