# Roomy - Hotel Room Booking & Search System

A modern .NET 10 microservices-based hotel room booking and search system built with ASP.NET Core, Entity Framework Core, and the CQRS pattern.

## 🏗️ Project Overview

Roomy is a backend service that handles:
- **Room Availability Search**: Search available rooms in hotels based on check-in/check-out dates and capacity requirements
- **Booking Management**: Create and manage hotel room bookings
- **Data Management**: Centralized database with users, hotels, rooms, and bookings

## 🏛️ Architecture

### Modular Structure

- **Roomy.Search** - Room search business logic and CQRS queries
- **Roomy.Search.API** - ASP.NET Core REST API entry point
- **Roomy.Booking** - Booking service and management logic
- **Roomy.Data** - Data access layer with Entity Framework Core
- **Roomy.Search.Tests** - Comprehensive unit tests with xUnit

### Technology Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | .NET 10 |
| **API** | ASP.NET Core 10 |
| **ORM** | Entity Framework Core 10.0.9 |
| **Database** | SQL Server LocalDB |
| **Architecture** | CQRS with MediatR 12.3.0 |
| **Validation** | FluentValidation 11.9.2 |
| **API Documentation** | Swagger/Swashbuckle 6.8.0 |
| **Testing** | xUnit 2.8.1, Moq 4.20.71, FluentAssertions 6.12.1 |

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK or later
- SQL Server LocalDB (included with Visual Studio or install separately)
- Visual Studio Code or Visual Studio

### Installation

1. **Clone the repository**
   ```bash
   cd d:\Sources\Roomy
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Initialize the database**
   ```bash
   cd Roomy.Search.API
   dotnet run
   ```
   The application will automatically seed the database on first run in Development mode.

### Running the API

```bash
cd Roomy.Search.API
dotnet run
```

The API will start on `https://localhost:7213` by default. Swagger UI will be available at the root path in development mode.

### Running Tests

```bash
cd Roomy.Search.Tests
dotnet test
```

Run with verbose output:
```bash
dotnet test --verbosity detailed
```

## 📊 Domain Models

### Users
- Store user information with email uniqueness
- Track user bookings

### Hotels
- Store hotel details (name, city, address)
- Maintain relationships with rooms

### Rooms
- Define room characteristics (type, capacity, price per night)
- Track room availability and bookings
- Composite unique index on (HotelId, RoomNumber)

### Bookings
- Link users to room reservations
- Store check-in/check-out dates
- Include performance-optimized HotelId denormalization
- Enforce business rule: CheckOutDate > CheckInDate

## 🔍 API Endpoints

### Room Search
**GET** `/api/rooms/search`

Search available rooms by hotel and date range.

**Request:**
```json
{
  "hotelId": "uuid",
  "checkInDate": "2026-06-22",
  "checkOutDate": "2026-06-25",
  "numberOfRooms": 1,
  "numberOfAdults": 2,
  "childrenAges": []
}
```

**Response:**
```json
{
  "availableRooms": [
    {
      "roomId": "uuid",
      "roomNumber": "101",
      "type": "Double",
      "capacity": 2,
      "pricePerNight": 120.00
    }
  ],
  "totalCount": 1
}
```

### Bookings
**POST** `/api/bookings`

Create a new booking reservation.

## 🏗️ CQRS Architecture

The project implements the Command Query Responsibility Segregation (CQRS) pattern using MediatR:

```
Controller
    ↓
MediatR IMediator.Send(Query)
    ↓
Query Handler (SearchAvailableRoomsQueryHandler)
    ↓
Repository/Database
```

**Benefits:**
- Separation of concerns between queries and commands
- Easier testing with isolated handlers
- Scalability for independent read/write optimization

## 🧪 Testing

### Test Coverage

**Roomy.Search.Tests** includes 13+ comprehensive tests covering:
- ✅ Successful searches with no bookings
- ✅ Capacity filtering logic
- ✅ Sub-room count validation
- ✅ Booking overlap detection
- ✅ Empty hotel scenarios
- ✅ Complex mixed scenarios
- ✅ Concurrent data fetches
- ✅ Logging verification

### Running Specific Tests

```bash
dotnet test --filter "TestClass=SearchAvailableRoomsQueryHandlerTests"
```

## 📁 Project Structure

```
Roomy/
├── Roomy.Search/              # Search business logic
│   ├── Queries/
│   ├── Handlers/
│   ├── DTOs/
│   ├── Controllers/
│   └── Validators/
├── Roomy.Search.API/          # REST API entry point
│   ├── Program.cs
│   ├── appsettings.json
│   └── Handlers/
├── Roomy.Booking/             # Booking service
│   ├── Controllers/
│   ├── DTOs/
│   └── ServiceCollectionExtensions.cs
├── Roomy.Data/                # Data access layer
│   ├── Models/
│   ├── AppDbContext.cs
│   └── Migrations/
└── Roomy.Search.Tests/        # Unit tests
    ├── Handlers/
    └── ...
```

## ⚙️ Configuration

### Connection String

The application uses a file-based SQL Server LocalDB:

```
Server=(localdb)\mssqllocaldb;
Database=RoomyDb;
Integrated Security=true;
TrustServerCertificate=true;
MultipleActiveResultSets=true
```

Located in: `appsettings.json`

### Database Location

MDF file: `Roomy.Data/RoomyDb.mdf`

## 🔄 Database Migrations

Two existing migrations:

1. **20260620120259_InitialCreate**
   - Creates initial schema with Users, Hotels, Rooms, Bookings tables
   - Establishes relationships and constraints

2. **20260621115126_AddHotelIdToBooking**
   - Adds denormalized HotelId to Bookings for performance optimization

Create new migrations:
```bash
dotnet ef migrations add MigrationName --project Roomy.Data
dotnet ef database update --project Roomy.Search.API
```

## 🛠️ Development Workflow

### Add a Feature

1. Create models in `Roomy.Data`
2. Create migrations: `dotnet ef migrations add FeatureName`
3. Implement business logic in `Roomy.Search` or `Roomy.Booking`
4. Add API endpoints in Controllers
5. Add validation with FluentValidation
6. Write unit tests in `Roomy.Search.Tests`

### Entity Framework Core Commands

```bash
# Add migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Drop database
dotnet ef database drop

# Rollback migration
dotnet ef migrations remove
```

## 📝 Dependencies

All dependencies are managed in [Directory.Packages.props](Directory.Packages.props).

### Core Packages
- Microsoft.EntityFrameworkCore 10.0.9
- Microsoft.EntityFrameworkCore.SqlServer 10.0.9
- MediatR 12.3.0
- FluentValidation 11.9.2
- Swashbuckle.AspNetCore 6.8.0

### Testing Packages
- xUnit 2.8.1
- Moq 4.20.71
- FluentAssertions 6.12.1
- Microsoft.NET.Test.Sdk 17.11.1

## 🤝 Contributing

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Write tests first (TDD approach)
3. Implement features
4. Ensure all tests pass: `dotnet test`
5. Submit a pull request

## 📄 License

[Add your license here]

## 📞 Support

For issues or questions, please create an issue in the repository.