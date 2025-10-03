# E-Commerce .NET Core Web API Backend

A robust .NET Core Web API for an e-commerce application, providing full CRUD functionality for products and categories, with JWT-based authentication.

## Features

- **Authentication**: JWT token-based authentication and authorization.
- **Product Management**: 
  - CRUD operations for products.
  - Image upload support.
  - Pagination and filtering.
- **Category Management**: CRUD operations for categories.
- **File Uploads**: Secure handling of file uploads for product images.

## Endpoints

### Authentication Endpoints
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/refresh` - Token refresh
- `POST /api/auth/revoke` - Token revocation

### Product Endpoints
- `GET /api/product` - Get paginated products
- `GET /api/product/{id}` - Get product by ID
- `GET /api/product/by-code/{code}` - Get product by code
- `GET /api/product/by-category/{categoryId}` - Get products by category
- `POST /api/product` - Create product (with image upload)
- `PUT /api/product/{id}` - Update product (supports multipart/form-data for image upload)
- `DELETE /api/product/{id}` - Delete product

### Category Endpoints
- `GET /api/category` - Get all categories
- `GET /api/category/{id}` - Get category by ID
- `POST /api/category` - Create category
- `PUT /api/category/{id}` - Update category
- `DELETE /api/category/{id}` - Delete category

## Getting Started

### Prerequisites
- .NET SDK (v8.0 or higher)
- SQL Server (or other compatible database)

### Installation

1. Clone the repository.
2. Update the database connection string in `appsettings.json`.
3. Run database migrations:
```bash
dotnet ef database update
```
4. Build and run the application:
```bash
dotnet run
```

## Technologies Used

- **.NET 8**: Latest version of .NET for building web APIs.
- **ASP.NET Core**: Web framework for building APIs.
- **Entity Framework Core**: ORM for database interaction.
- **JWT Authentication**: Token-based security.
- **Swagger/OpenAPI**: API documentation.

## Contributing

1. Fork the repository.
2. Create a feature branch.
3. Make your changes.
4. Test thoroughly.
5. Submit a pull request.

## License

This project is part of an e-commerce application demonstration.