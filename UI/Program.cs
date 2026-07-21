using Core.Interfaces;
using Repository.Data;
using Application.Services;
using Application.InterfaceServices;
using System.Threading.Tasks;

namespace UI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // initialize the repository and auth service
            IFileManager repository = new FileRepository();
            IAuthService authService = new AuthService(repository);

            // Create and run the UI
            var ui = new ConsoleUI(authService, repository, new UI.Helpers.ConsoleWrapper());
            await ui.RunAsync();
        }
    }
}
