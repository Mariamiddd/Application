using Core.Interfaces;
using Repository.Data;
using Application.Services;
using Application.InterfaceServices;
using System.Threading.Tasks;
using Spectre.Console;

namespace UI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Enable ANSI color support in console
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;

            // initialize the repository and auth service
            IFileManager repository = new FileRepository();
            IAuthService authService = new AuthService(repository);

            // Create and run the UI
            var ui = new ConsoleUI(authService, repository, new UI.Helpers.ConsoleWrapper());
            await ui.RunAsync();
        }
    }
}
