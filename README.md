# Event Management System

Welcome to the Event Management System, a comprehensive solution for managing events, participants, and categories. This project is built using ASP.NET Core Razor Pages and Entity Framework Core.

This project being developed for an assignment from instructor HoanNN in FPT University, Summer 2024 Block3W. You can get the requirements [here](https://github.com/user-attachments/files/16648119/Assignment_03_Hoan.docx).

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Features

- **User Roles**: Admin and regular users with different access levels.
- **Event Management**: Create, update, delete, and view events.
- **Search and Filter**: Search events by title, category, and date range.
- **Real-time Updates**: Real-time event updates using SignalR.
- **Validation**: Client-side and server-side validation for event forms.

## Technologies Used

- **ASP.NET Core**: Web framework for building modern web applications.
- **Entity Framework Core**: ORM for database operations.
- **Razor Pages**: Simplified web page creation.
- **SignalR**: Real-time web functionality.
- **Bootstrap**: Responsive design and styling.
- **jQuery**: Simplified JavaScript operations. ~why have this?~

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Installation

1. **Clone the repository**:
```
    git clone https://github.com/gnaohuv22/EventManagement.git
    cd EventManagement
```

2. **Set up the database**:
    - Update the connection string in `appsettings.json` to point to your SQL Server instance.
    - Run the following command to apply migrations:
```
    dotnet ef database update
```

3. **Run the application**:
```
    dotnet run
```


4. **Open your browser** and navigate to `https://localhost:9000`. Modify it at `launchSettings.json` as your need.

## Usage

- **Admin Role**:
  - Create, update, and delete events.
  - View all events and participants.
  - Access to all features.

- **Regular User**:
  - View events.
  - Search and filter events by title, category, and date range.

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature`).
3. Commit your changes (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature/your-feature`).
5. Open a Pull Request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.md) file for details.

## Contact

For any questions or concerns, please contact us at:

- **Email**: hoangvhhe176169@fpt.edu.vn
- **Address**: Please don't find this

Thank you for using the Event Management System!
