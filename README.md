# Northwind E-Commerce API

A simple REST API built with **ASP.NET Core Web API** and **Entity Framework Core** on top of Microsoft's **Northwind** sample database (SQL Server).

This is my first Web API. The goal was to keep the code simple and learn how a backend engineer thinks: designing endpoints, separating the database model from what the API exposes, validating input, and returning correct HTTP status codes.

## Features

- **Products**: list all products, get one product, create a product
- **Orders**: create an order with multiple items, get an order with its items
- Database-first approach: the models were scaffolded from the existing Northwind database
- DTOs (Data Transfer Objects) so the API never exposes database entities directly
- Input validation with clear `400` / `404` responses
- Swagger UI for testing every endpoint

## Tech Stack

- C# and ASP.NET Core Web API
- Entity Framework Core (database-first, scaffolded)
- SQL Server and SQL Server Management Studio (SSMS)
- Swagger (Swashbuckle)
- Visual Studio 2022

## Project Structure

```
Northwind_ecomm/
├── Controllers/
│   ├── OrdersController.cs
│   └── ProductsController.cs
├── Data/
│   └── NorthwindContext.cs      (scaffolded DbContext)
├── DTOs/
│   ├── CreateProductDto.cs
│   ├── ProductResponseDto.cs
│   └── OrderDtos.cs             (create/response DTOs for orders)
├── Models/                      (scaffolded entities)
├── appsettings.json
└── Program.cs
```

## Getting Started

### Prerequisites

- .NET SDK (the version this project targets)
- SQL Server and SSMS
- Visual Studio 2022 (or the `dotnet` CLI)

### 1. Set up the database

1. Open `Northwind_Ms_SQL_2005.sql` in SSMS.
2. Run it. **Warning:** the script drops the `Northwind` database if it already exists and recreates it with the sample data.
3. Check the **Messages** tab for errors, and confirm the tables have data (for example 77 products and 830 orders).

### 2. Configure the connection string

Edit `appsettings.json` and set the connection string to your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Run the API

```bash
dotnet run
```

Or press the green **https** button in Visual Studio. Then open Swagger:

```
https://localhost:<port>/swagger
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Products` | List all products |
| GET | `/api/Products/{id}` | Get one product |
| POST | `/api/Products` | Create a product |
| GET | `/api/Orders/{id}` | Get an order with its items |
| POST | `/api/Orders` | Create an order with items |

### Create a product

`POST /api/Products`

```json
{
  "productName": "Test Product",
  "supplierId": 1,
  "categoryId": 1,
  "quantityPerUnit": "10 boxes",
  "unitPrice": 25,
  "unitsInStock": 50
}
```

- `productName` is required. `supplierId` and `categoryId` are optional, but if sent they must exist.
- Returns `201 Created` with the new product and a `Location` header pointing to `GET /api/Products/{id}`.
- Returns `400` for a negative price or stock, and `404` if the category or supplier does not exist.

### Create an order

`POST /api/Orders`

```json
{
  "customerId": "ALFKI",
  "items": [
    { "productId": 1, "quantity": 2 },
    { "productId": 2, "quantity": 1 }
  ]
}
```

- The client sends only the customer and the items. The server sets the order date and takes each item's unit price from the `Products` table, so clients cannot choose their own prices.
- The order and all its items are saved together in a single `SaveChangesAsync()` call.
- Returns `201 Created` with the new order id and a `Location` header pointing to `GET /api/Orders/{id}`.
- Returns `400` for an empty item list or a quantity of 0 or less, and `404` if the customer or a product does not exist.

### Get an order

`GET /api/Orders/{id}` returns the order with its items, or `404` if it does not exist.

## Design Decisions

- **DTOs instead of entities.** The scaffolded entities mirror the database tables and link to each other (`Order` → `OrderDetails` → `Order`). Returning them directly can cause JSON loops and lets clients set fields they should not control. DTOs expose only what each endpoint needs.
- **Server-controlled values.** Order date, unit prices, and defaults like `Discontinued` are set by the server.
- **Validate before saving.** Checking foreign keys in code returns a clean `404` instead of a database error and a `500`.
- **Database-first.** The Northwind tables and relations already existed, so the models were scaffolded instead of written by hand.

## Roadmap

- [ ] Prevent the same product from appearing twice in one order
- [ ] Check stock (`UnitsInStock`) when placing an order
- [ ] Endpoints to list orders and update or delete products
- [ ] Endpoints for suppliers, employees, and shippers
- [ ] Authentication and authorization (suppliers manage products, customers place orders)

## Author

Omar Ahmed Fouad
