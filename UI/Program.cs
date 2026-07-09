using Application.Services;
using Core.Interfaces;
using Core.Models;
using Repository.Data;

namespace UI
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //Client myClient = new Client();
            //myClient.Username = "TestingUser";
            //myClient.Balance = 150;

            //myClient.DisplayMenu();

            //// 2. Create and test an Admin
            //Admin myAdmin = new Admin();
            //myAdmin.Username = "TestingAdmin";

            //myAdmin.DisplayMenu();

            IFileManager repository = new FileRepository();
            UserServices userServices = new UserServices(repository);

            userServices.RegisterUser("John doe", "password123");
        }
    }
}