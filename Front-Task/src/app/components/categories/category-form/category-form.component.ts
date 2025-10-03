import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CategoryService } from '../../../services/category.service';
import { ErrorHandlerService, ApiError } from '../../../services/error-handler.service';
import { ValidationService } from '../../../services/validation.service';
import { Category, CategoryCreate } from '../../../models/category.model';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './category-form.component.html',
  styleUrls: ['./category-form.component.css']
})
export class CategoryFormComponent implements OnInit {
  categoryForm: FormGroup;
  isEditMode = false;
  categoryId: number | null = null;
  isLoading = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';
  apiError: ApiError | null = null;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private categoryService: CategoryService,
    private errorHandler: ErrorHandlerService
  ) {
    this.categoryForm = this.fb.group({
      name: ['', [
        Validators.required, 
        Validators.minLength(2),
        Validators.maxLength(100)
      ]],
      description: ['', [Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.categoryId = parseInt(id);
      this.loadCategory();
    }
  }

  loadCategory(): void {
    if (this.categoryId) {
      this.isLoading = true;
      this.categoryService.getById(this.categoryId).subscribe({
        next: (category) => {
          this.categoryForm.patchValue({
            name: category.name,
            description: category.description || ''
          });
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Error loading category';
          this.isLoading = false;
        }
      });
    }
  }

  onSubmit(): void {
    if (this.categoryForm.valid) {
      this.isSubmitting = true;
      this.errorMessage = '';
      this.successMessage = '';
      this.apiError = null;

      const formValue = this.categoryForm.value;
      const categoryData: CategoryCreate = {
        name: formValue.name,
        description: formValue.description || undefined
      };

      if (this.isEditMode && this.categoryId) {
        this.categoryService.update(this.categoryId, categoryData).subscribe({
          next: () => {
            this.successMessage = 'Category updated successfully!';
            this.isSubmitting = false;
            setTimeout(() => {
              this.router.navigate(['/categories']);
            }, 2000);
          },
          error: (error) => {
            this.isSubmitting = false;
            this.apiError = this.errorHandler.handleError(error);
            this.errorMessage = this.apiError.message;
          }
        });
      } else {
        this.categoryService.create(categoryData).subscribe({
          next: () => {
            this.successMessage = 'Category created successfully!';
            this.isSubmitting = false;
            setTimeout(() => {
              this.router.navigate(['/categories']);
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
    const control = this.categoryForm.get(fieldName);
    if (control && control.invalid && control.touched) {
      return ValidationService.getErrorMessage(control.errors!, fieldName);
    }
    return null;
  }

  get name() { return this.categoryForm.get('name'); }
  get description() { return this.categoryForm.get('description'); }

  goBack(): void {
    this.router.navigate(['/categories']);
  }
}
