# Web API for Managing Orders, Products, and Customers

## Overview

This project is a Web API implemented in .NET for managing orders, products, and customers. It provides endpoints to perform CRUD (Create, Read, Update, Delete) operations on these entities. The project follows Domain-Driven Design (DDD) principles to ensure a clear separation of concerns and a robust domain model.

## Features

- **Orders Management**: Create, view, update, and delete orders.
- **Products Management**: Create, view, update, and delete products.
- **Customers Management**: Create, view, update, and delete customers.
- **JWT Authentication**: Secure the API endpoints using JSON Web Tokens (JWT).

## Technologies Used

- **.NET 6**
- **Entity Framework Core**
- **PostgreSQL**
- **Swagger for API Documentation**
- **JWT Authentication**

## Getting Started

### Prerequisites

- .NET8 SDK
- PostgreSQL

### Installation

1. Clone the repository:
    ```sh
    git clone git@github.com:cpilao/line10-sales.git
    ```
2. Navigate to the project directory:
    ```sh
    cd line10-sales
    ```
3. Restore the dependencies:
    ```sh
    dotnet restore
    ```

### Configuration

1. Update the `appsettings.json` file with your PostgreSQL connection string and JWT settings:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=your_host;Database=your_db;Username=your_user;Password=your_password"
      },
      "TokenValidation": {
            "Issuer": "your-issuer",
            "Audience": "your-audience"
        }
    }
    ```

### Database Migration

Database migrations are executed in background on service startup

### Running the Application

1. Run the application:
    ```sh
    dotnet run
    ```

2. The API will be available at `http://localhost:5126`.

### API Documentation

- Swagger UI is available at `http://localhost:5126/swagger`.

## Usage

### Authentication

This API uses JWT (JSON Web Token) for authentication. To access the protected endpoints, you need to include a valid JWT token in the `Authorization` header of your requests.

### Endpoints

- **Orders**
  - `GET /api/orders`
  - `GET /api/orders/{id}`
  - `POST /api/orders`
  - `POST /api/orders/{id}/products`
  - `DELETE /api/orders/{id}/products`
  - `DELETE /api/orders/{id}`

- **Products**
  - `GET /api/products`
  - `GET /api/products/{id}`
  - `POST /api/products`
  - `PUT /api/products/{id}`
  - `DELETE /api/products/{id}`

- **Customers**
  - `GET /api/customers`
  - `GET /api/customers/{id}`
  - `POST /api/customers`
  - `PUT /api/customers/{id}`
  - `DELETE /api/customers/{id}`

## Domain-Driven Design (DDD)

This project follows Domain-Driven Design (DDD) principles to create a rich domain model that encapsulates the business logic. The main components include:

- **Entities**: Represent the core business objects with a unique identity.
- **Repositories**: Provide methods to access and manipulate aggregate entities.

## Application Layer

The application layer is responsible for managing domain entities and orchestrating the application's use cases. It acts as a mediator between the domain layer and the presentation layer (e.g., API controllers). The main components include:

- **Commands**: Represent actions that change the state of the system (e.g., creating an order).
- **Queries**: Represent actions that retrieve data without changing the state of the system (e.g., fetching a list of products).
- **Handlers**: Implement the logic for handling commands and queries, often using repositories and domain services.
- **DTOs (Data Transfer Objects)**: Simplify data transfer between layers and ensure that only necessary data is exposed.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Contact

Carlos Pilão - carlospilao@gmail.com

Project Link: https://github.com/cpilao/line10-sales