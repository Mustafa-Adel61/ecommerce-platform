import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { Account } from './components/account/account';

export const routes: Routes = [
  { path: '', redirectTo: '/products', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./components/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./components/auth/register/register.component').then(m => m.RegisterComponent) },
  
  // Protected routes
  { 
    path: 'products', 
    loadComponent: () => import('./components/products/product-list/product-list.component').then(m => m.ProductListComponent),
    canActivate: [AuthGuard]
  },
  { 
    path: 'products/create', 
    loadComponent: () => import('./components/products/product-form/product-form.component').then(m => m.ProductFormComponent),
    canActivate: [AuthGuard]
  },
  { 
    path: 'products/:id', 
    loadComponent: () => import('./components/products/product-detail/product-detail.component').then(m => m.ProductDetailComponent),
    canActivate: [AuthGuard]
  },
  { 
    path: 'products/:id/edit', 
    loadComponent: () => import('./components/products/product-form/product-form.component').then(m => m.ProductFormComponent),
    canActivate: [AuthGuard]
  },
  
  { 
    path: 'categories', 
    loadComponent: () => import('./components/categories/category-list/category-list.component').then(m => m.CategoryListComponent),
    canActivate: [AuthGuard]
  },
  { 
    path: 'categories/create', 
    loadComponent: () => import('./components/categories/category-form/category-form.component').then(m => m.CategoryFormComponent),
    canActivate: [AuthGuard]
  },
  { 
    path: 'categories/:id/edit', 
    loadComponent: () => import('./components/categories/category-form/category-form.component').then(m => m.CategoryFormComponent),
    canActivate: [AuthGuard]
  },
  { path: 'account', component: Account },
  
  { path: '**', redirectTo: '/products' }
];