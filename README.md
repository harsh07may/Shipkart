# Shipkart Backend

Shipkart is an .NET Core 8 WebAPI backend service for managing e-commerce shipping operations.

## Features

- User authentication and authorization
- Product and inventory management
- Order processing and tracking
- Shipping rate calculation
- RESTful API endpoints

## Technologies

- ASP.NET Core 8
- Entity Framework Core
- PostgreSQL

## Getting Started

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/shipkart.git
   ```

2. Navigate to the backend directory:

   ```bash
   cd Shipkart
   ```

3. Update the connection string in `appsettings.json`.
4. Run database migrations:

   ```bash
   dotnet ef database update
   ```

5. Start the application:

   ```bash
   dotnet run
   ```

## API Documentation

See [API.md](API.md) for endpoint details.

## Contributing

Pull requests are welcome. For major changes, open an issue first.

## License

This project is licensed under the MIT License.
