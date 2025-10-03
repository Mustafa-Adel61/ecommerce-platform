import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { CategoryService } from '../../../services/category.service';
import { Product, PagedResult } from '../../../models/product.model';
import { Category } from '../../../models/category.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  categories: Category[] = [];
  pagedResult: PagedResult<Product> | null = null;
  currentPage = 1;
  pageSize = 12;
  selectedCategoryId: number | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';

    if (this.selectedCategoryId) {
      this.productService.getByCategory(this.selectedCategoryId).subscribe({
        next: (products) => {
          this.products = products;
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Error loading products';
          this.isLoading = false;
        }
      });
    } else {
      this.productService.getAll(this.currentPage, this.pageSize).subscribe({
        next: (result) => {
          this.pagedResult = result;
          this.products = result.items;
            this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Error loading products';
          this.isLoading = false;
        }
      });
    }
  }

  onCategoryChange(categoryId: string): void {
    this.selectedCategoryId = categoryId ? parseInt(categoryId) : null;
    this.currentPage = 1;
    this.loadProducts();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadProducts();
  }

  get totalPages(): number {
    return this.pagedResult?.totalPages || 0;
  }

  get pages(): number[] {
    const pages: number[] = [];
    for (let i = 1; i <= this.totalPages; i++) {
      pages.push(i);
    }
    return pages;
  }

  calculateDiscountedPrice(price: number, discountRate: number): number {
    return price * (1 - discountRate / 100);
  }

  getFullImageUrl(imagePath: string | null): string {
    if (!imagePath) {
      return 'assets/images/placeholder.svg';
    }
    const baseUrl = environment.apiUrl.replace('/api', '');
    return `${baseUrl}/${imagePath}`;
  }
}
