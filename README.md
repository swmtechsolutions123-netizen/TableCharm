# TableCharm API

A comprehensive .NET API for managing direct selling networks with support for multi-level commission calculations.

## Features

### Distributors Management
- Create, read, update, and delete distributors
- Manage hierarchical relationships (parent-child)
- View distributor hierarchy and downlines
- Filter by state or location

### Sales Management
- Record sales transactions
- Track sales by distributor and date range
- Generate sales summaries
- Support for multiple sale statuses

### Commission Management
- Automatic multi-level commission calculation
  - Level 0 (Self): 5% of own sales
  - Level 1 (Direct Downline): 3% of downline sales
  - Level 2 (Second-Level): 1% of second-level sales
- Track commission status and payment
- Detailed commission history

## Technology Stack

- **Framework**: .NET 6.0
- **Database**: SQL Server with Entity Framework Core
- **API**: RESTful API with Swagger/OpenAPI documentation
- **Logging**: Microsoft.Extensions.Logging
- **Dependency Injection**: Built-in ASP.NET Core DI

## API Endpoints

### Distributors
- `GET /api/distributors` - Get all distributors
- `GET /api/distributors/{id}` - Get distributor by ID
- `GET /api/distributors/{id}/hierarchy` - Get distributor hierarchy
- `GET /api/distributors/{id}/downline` - Get direct downline
- `GET /api/distributors/state/{state}` - Get distributors by state
- `POST /api/distributors` - Create new distributor
- `PUT /api/distributors/{id}` - Update distributor
- `DELETE /api/distributors/{id}` - Delete distributor

### Sales
- `GET /api/sales` - Get all sales
- `GET /api/sales/{id}` - Get sale by ID
- `GET /api/sales/distributor/{distributorId}` - Get sales by distributor
- `GET /api/sales/date-range` - Get sales by date range
- `GET /api/sales/summary/{distributorId}` - Get sales summary
- `POST /api/sales` - Create new sale
- `PUT /api/sales/{id}` - Update sale
- `DELETE /api/sales/{id}` - Delete sale

### Commissions
- `GET /api/commissions` - Get all commissions
- `GET /api/commissions/{id}` - Get commission by ID
- `GET /api/commissions/distributor/{distributorId}` - Get commissions by distributor
- `GET /api/commissions/status/{status}` - Get commissions by status
- `GET /api/commissions/total/{distributorId}` - Get total commission
- `POST /api/commissions/calculate` - Calculate commission

## Setup Instructions

### Prerequisites
- .NET 6.0 SDK or later
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
```bash
git clone https://github.com/swmtechsolutions123-netizen/TableCharm.git
cd TableCharm
```

2. Update connection string in `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=TableCharm;Integrated Security=true;Encrypt=false;"
}
```

3. Install dependencies
```bash
dotnet restore
```

4. Apply migrations
```bash
dotnet ef database update
```

5. Run the application
```bash
dotnet run
```

The API will be available at `https://localhost:5001` with Swagger UI at `https://localhost:5001/swagger`

## Development

### Creating Migrations
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Running Tests
```bash
dotnet test
```

## Project Structure

```
TableCharm/
├── src/
│   ├── Controllers/      # API endpoints
│   ├── Services/         # Business logic
│   ├── Models/           # Entity models
│   ├── DTOs/             # Data transfer objects
│   ├── Data/             # DbContext
│   ├── Program.cs        # Entry point
│   └── Startup.cs        # Configuration
├── appsettings.json      # Configuration
├── TableCharm.csproj     # Project file
└── README.md             # Documentation
```

## Commission Calculation Formula

For a distributor with ID `X` during period `Start` to `End`:

```
Level 0 Commission = (Sum of X's sales) × 0.05
Level 1 Commission = (Sum of X's direct downlines' sales) × 0.03
Level 2 Commission = (Sum of X's second-level downlines' sales) × 0.01
Total Commission = Level 0 + Level 1 + Level 2
```

## Error Handling

The API returns standard HTTP status codes:
- `200 OK` - Successful request
- `201 Created` - Resource created
- `204 No Content` - Successful deletion
- `400 Bad Request` - Invalid input
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## License

This project is licensed under the MIT License - see LICENSE file for details.

## Contributing

Contributions are welcome! Please follow the standard GitHub workflow:
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## Support

For issues and questions, please open an issue on the GitHub repository.
