# E-Commerce Angular Frontend

A modern Angular e-commerce application with full CRUD functionality for products and categories, built with Bootstrap for responsive design.

## Features

- **Authentication**: Login and Register functionality with JWT tokens
- **Product Management**: 
  - View products in a responsive card layout
  - Create, Read, Update, Delete products
  - Image upload support
  - Pagination
  - Category filtering
  - Discount calculations
- **Category Management**: Full CRUD operations for categories
- **Responsive Design**: Built with Bootstrap 5 for modern, mobile-friendly UI
- **Route Protection**: Auth guards to protect authenticated routes

## Backend Integration

This frontend integrates with the provided .NET Core Web API backend with the following endpoints:

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

## Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── auth/
│   │   │   ├── login/
│   │   │   └── register/
│   │   ├── products/
│   │   │   ├── product-list/
│   │   │   ├── product-detail/
│   │   │   └── product-form/
│   │   ├── categories/
│   │   │   ├── category-list/
│   │   │   └── category-form/
│   │   └── layout/
│   │       └── navbar/
│   ├── services/
│   │   ├── auth.service.ts
│   │   ├── product.service.ts
│   │   └── category.service.ts
│   ├── models/
│   │   ├── auth.model.ts
│   │   ├── product.model.ts
│   │   └── category.model.ts
│   ├── guards/
│   │   └── auth.guard.ts
│   └── interceptors/
│       └── auth.interceptor.ts
├── assets/
│   └── images/
└── environments/
    ├── environment.ts
    └── environment.prod.ts
```

## Getting Started

### Prerequisites
- Node.js (v18 or higher)
- Angular CLI (v17 or higher)
- .NET Core Web API backend running

### Installation

1. Install dependencies:
```bash
npm install
```

2. Update the API URL in `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7000/api' // Update with your backend URL
};
```

3. Start the development server:
```bash
ng serve
```

4. Open your browser and navigate to `http://localhost:4200`

### Usage

1. **Register/Login**: Create an account or login with existing credentials
2. **Manage Categories**: Navigate to Categories to create and manage product categories
3. **Manage Products**: 
   - View all products in a responsive grid
   - Filter by category
   - Create new products with image upload
   - Edit existing products
   - Delete products
   - View detailed product information

## Technologies Used

- **Angular 20**: Modern Angular with standalone components
- **Bootstrap 5**: Responsive CSS framework
- **TypeScript**: Type-safe JavaScript
- **RxJS**: Reactive programming
- **Angular Forms**: Reactive forms with validation
- **Angular Router**: Client-side routing with guards

## Key Features Implementation

### Authentication
- JWT token-based authentication
- Automatic token refresh
- Route protection with auth guards
- HTTP interceptor for automatic token attachment

### Product Management
- Image upload with preview
- Form validation
- Pagination for large datasets
- Category filtering
- Discount calculations
- Responsive card layout

### Error Handling
- Global error handling
- User-friendly error messages
- Loading states and spinners

### Responsive Design
- Mobile-first approach
- Bootstrap grid system
- Responsive navigation
- Touch-friendly interface

## API Integration

The application uses Angular's HttpClient for API communication with:
- Automatic JWT token attachment via interceptors
- Error handling and user feedback
- Loading states during API calls
- Form data handling for file uploads

## Development

### Building for Production
```bash
ng build --configuration production
```

### Running Tests
```bash
ng test
```

### Code Style
The project uses Prettier for code formatting with Angular-specific configurations.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is part of an e-commerce application demonstration.