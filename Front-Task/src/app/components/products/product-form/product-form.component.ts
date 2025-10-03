import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { CategoryService } from '../../../services/category.service';
import { ErrorHandlerService, ApiError } from '../../../services/error-handler.service';
import { ValidationService } from '../../../services/validation.service';
import { Product, ProductCreate, ProductUpdate } from '../../../models/product.model';
import { Category } from '../../../models/category.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css']
})
export class ProductFormComponent implements OnInit {
  productForm: FormGroup;
  categories: Category[] = [];
  isEditMode = false;
  productId: number | null = null;
  isLoading = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';
  apiError: ApiError | null = null;
  selectedFile: File | null = null;
  previewUrl: string | null = null;
  existingCodes: string[] = [];
  maxFileSize = 5; // MB
  allowedFileTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private categoryService: CategoryService,
    private errorHandler: ErrorHandlerService
  ) {
    this.productForm = this.fb.group({
      code: ['', [
        Validators.required, 
        Validators.minLength(2),
        Validators.maxLength(50),
        ValidationService.alphanumericOnly()
      ]],
      name: ['', [
        Validators.required, 
        Validators.minLength(2),
        Validators.maxLength(100)
      ]],
      price: [0, [
        Validators.required, 
        ValidationService.priceRange(0.01, 999999.99)
      ]],
      minimumQuantity: [1, [
        Validators.required, 
        Validators.min(1),
        Validators.max(9999)
      ]],
      discountRate: [0, [
        Validators.required, 
        Validators.min(0), 
        Validators.max(100)
      ]],
      categoryId: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.productId = parseInt(id);
      this.loadProduct();
    }
  }

  loadCategories(): void {
    this.isLoading = true;
    this.categoryService.getAll().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Error loading categories';
        this.isLoading = false;
      }
    });
  }

  loadProduct(): void {
    if (this.productId) {
      this.isLoading = true;
      this.productService.getById(this.productId).subscribe({
        next: (product) => {
          this.productForm.patchValue({
            code: product.code,
            name: product.name,
            price: product.price,
            minimumQuantity: product.minimumQuantity,
            discountRate: product.discountRate,
            categoryId: product.categoryId
          });
          
          if (product.imagePath) {
            this.previewUrl = product.imagePath;
          }
          
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Error loading product';
          this.isLoading = false;
        }
      });
    }
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // Validate file size
      if (file.size > this.maxFileSize * 1024 * 1024) {
        this.errorMessage = `File size must not exceed ${this.maxFileSize}MB`;
        return;
      }

      // Validate file type
      if (!this.allowedFileTypes.includes(file.type)) {
        this.errorMessage = `File type must be one of: ${this.allowedFileTypes.join(', ')}`;
        return;
      }

      this.selectedFile = file;
      this.errorMessage = '';
      
      // Create preview URL
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.previewUrl = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      this.isSubmitting = true;
      this.errorMessage = '';
      this.successMessage = '';
      this.apiError = null;

      const formValue = this.productForm.value;

      if (this.isEditMode && this.productId) {
        const updateData: ProductUpdate = {
          name: formValue.name,
          price: formValue.price,
          minimumQuantity: formValue.minimumQuantity,
          discountRate: formValue.discountRate,
          categoryId: formValue.categoryId,
          imageFile: this.selectedFile !== null ? this.selectedFile : null
        };
        this.productService.update(this.productId, updateData).subscribe({
          next: () => {
            this.successMessage = 'Product updated successfully!';
            this.isSubmitting = false;
            setTimeout(() => {
              this.router.navigate(['/products', this.productId]);
            }, 2000);
          },
          error: (error) => {
            this.isSubmitting = false;
            this.apiError = this.errorHandler.handleError(error);
            this.errorMessage = this.apiError.message;
          }
        });
      } else {
        const createData: ProductCreate = {
          code: formValue.code,
          name: formValue.name,
          price: formValue.price,
          minimumQuantity: formValue.minimumQuantity,
          discountRate: formValue.discountRate,
          categoryId: formValue.categoryId,
          imageFile: this.selectedFile || undefined
        };

        this.productService.create(createData).subscribe({
          next: (product) => {
            this.successMessage = 'Product created successfully!';
            this.isSubmitting = false;
            setTimeout(() => {
              this.router.navigate(['/products', product.id]);
            }, 2000);
          },
          error: (error) => {
            this.isSubmitting = false;
            this.apiError = this.errorHandler.handleError(error);
            this.errorMessage = this.apiError.message;
          }
        });
      }
    }
  }

  getFieldError(fieldName: string): string | null {
    const control = this.productForm.get(fieldName);
    if (control && control.invalid && control.touched) {
      return ValidationService.getErrorMessage(control.errors!, fieldName);
    }
    return null;
  }

  calculateDiscountedPrice(): number {
    const price = this.productForm.get('price')?.value || 0;
    const discountRate = this.productForm.get('discountRate')?.value || 0;
    return price * (1 - discountRate / 100);
  }

  get code() { return this.productForm.get('code'); }
  get name() { return this.productForm.get('name'); }
  get price() { return this.productForm.get('price'); }
  get minimumQuantity() { return this.productForm.get('minimumQuantity'); }
  get discountRate() { return this.productForm.get('discountRate'); }
  get categoryId() { return this.productForm.get('categoryId'); }

  goBack(): void {
    if (this.isEditMode && this.productId) {
      this.router.navigate(['/products', this.productId]);
    } else {
      this.router.navigate(['/products']);
    }
  }

  getFullImageUrl(imagePath: string | null): string {
    if (!imagePath) {
      return 'assets/images/placeholder.svg';
    }
    if (imagePath.startsWith('data:')) {
      return imagePath;
    }
    const baseUrl = environment.apiUrl.replace('/api', '');
    return `${baseUrl}/${imagePath}`;
  }
}
