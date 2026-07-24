# Loan Management Application

A comprehensive .NET-based console application for managing loan requests with role-based access control. This system allows clients to submit loan requests and administrators to review and manage them.

## Features

- **User Authentication**: Secure login and registration system for both clients and administrators
- **Role-Based Access Control**: Separate functionality for Admins and Clients
- **Loan Request Management**: 
  - Clients can submit loan requests with specified amounts and income information
  - Track loan status (Pending, Approved, Rejected)
  - View request history and details
- **Admin Panel**: 
  - Review pending loan requests
  - Approve or reject applications
  - Manage user accounts
  - View transaction history
- **Data Persistence**: File-based storage using JSON format
- **Console UI**: Interactive command-line interface with enhanced formatting using Spectre.Console

## Project Structure

```
Application/
├── Core/                          # Core business logic and models
│   ├── Constants/
│   │   └── SecurityConstants.cs   # Security-related constants
│   ├── Enums/
│   │   ├── LoanStatus.cs          # Loan status enumeration (Pending, Approved, Rejected)
│   │   └── Roles.cs               # User role enumeration (Admin, Client)
│   ├── Interfaces/
│   │   └── IFileManager.cs        # File management interface
│   └── Models/
│       ├── Account.cs             # Account model
│       ├── Admin.cs               # Admin user model
│       ├── Client.cs              # Client user model
│       ├── LoanRequest.cs         # Loan request model
│       ├── Transaction.cs         # Transaction model
│       └── User.cs                # Base user model (abstract)
├── Application/                   # Application services
│   ├── InterfaceServices/
│   │   └── IAuthService.cs        # Authentication service interface
│   └── Services/
│       └── AuthService.cs         # Authentication service implementation
├── Repository/                    # Data access layer
│   └── Data/
│       ├── FileRepository.cs      # File-based repository implementation
│       ├── Helpers/
│       │   ├── FileHelper.cs      # File I/O utilities
│       │   ├── LoanDeserializer.cs# Loan JSON deserialization
│       │   ├── RoleHelper.cs      # Role helper utilities
│       │   ├── RoleJsonConverter.cs# Role JSON conversion
│       │   └── UserDeserializer.cs# User JSON deserialization
│       ├── loans.json             # Loan data storage
│       └── users.json             # User data storage
└── UI/                            # User interface layer
	├── ConsoleUI.cs              # Main console UI controller
	├── AdminMenu.cs              # Admin menu interface
	├── UserMenu.cs               # Client menu interface
	├── GuestMenu.cs              # Guest menu interface
	├── Program.cs                # Application entry point
	├── Helpers/
	│   ├── ConsoleWrapper.cs     # Console abstraction wrapper
	│   └── InputHelpers.cs       # Input validation and helpers
	└── Interfaces/
		└── IConsole.cs           # Console interface contract
```

## Technology Stack

- **Framework**: .NET 10
- **Language**: C# 13
- **UI Library**: [Spectre.Console](https://spectreproject.dev/) - For enhanced console output
- **Architecture**: Layered Architecture
  - UI Layer (Console Interface)
  - Application Layer (Services)
  - Repository Layer (Data Access)
  - Core Layer (Models & Business Logic)

## Installation & Setup

### Prerequisites

- .NET 10 SDK or later
- Visual Studio 2026 (Community Edition or higher) or any compatible IDE

### Clone the Repository

```bash
git clone https://github.com/Mariamiddd/Application.git
cd Application
```

### Build the Project

```bash
dotnet build
```

### Run the Application

```bash
dotnet run --project UI/UI.csproj
```

## Usage

### User Roles

#### **Guest**
- View application information
- Register a new account as a Client
- Login with existing credentials

#### **Client**
- Submit loan requests with desired amount and income details
- View request history and current status
- Track approved/rejected applications
- View transaction history
- Manage account information

#### **Admin**
- View all pending loan requests
- Approve or reject loan applications
- Manage user accounts
- View all transactions
- System administration tasks

### Typical Workflow

1. **Start the Application**: Launch the console application
2. **Guest Options**:
   - Select "Register" to create a new client account
   - Or select "Login" if you already have credentials
3. **Client Workflow**:
   - Log in with client credentials
   - Submit a loan request with amount and income information
   - Monitor the status of your requests
   - View approved/rejected applications
4. **Admin Workflow**:
   - Log in with admin credentials
   - Review pending loan applications
   - Make approval/rejection decisions
   - Manage user accounts and view reports

## Data Storage

The application uses JSON files for data persistence:

- **users.json**: Stores user accounts (Clients, Admins, Guests)
- **loans.json**: Stores all loan requests and their statuses

These files are located in the `Repository/Data/` directory.

## Architecture Highlights

### Layered Architecture
- **Separation of Concerns**: Each layer has a specific responsibility
- **Dependency Injection**: Services are injected through constructors
- **Abstract Base Classes**: Polymorphism for different user types
- **Interface-Based Design**: Loose coupling between components

### Security Features
- Password-based authentication
- Role-based authorization
- Secure credential storage
- Input validation and sanitization

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is open source and available under the MIT License.

## Author

**Mariam** - [GitHub Profile](https://github.com/Mariamiddd)

## Support

For issues, feature requests, or questions, please open an issue on the [GitHub repository](https://github.com/Mariamiddd/Application/issues).

---

**Last Updated**: 2024
**Version**: 1.0.0
**Framework**: .NET 10
