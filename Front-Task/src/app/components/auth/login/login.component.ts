import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { ErrorHandlerService, ApiError } from '../../../services/error-handler.service';
import { ValidationService } from '../../../services/validation.service';
import { LoginRequest } from '../../../models/auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  apiError: ApiError | null = null;
  showPassword = false;
  maxAttempts = 5;
  attempts = 0;
  isLocked = false;
  lockoutTime: Date | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private errorHandler: ErrorHandlerService
  ) {
    this.loginForm = this.fb.group({
      userName: [{
        value: '',
        disabled: false
      }, [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(50),
        ValidationService.noWhitespace()
      ]],
      password: [{
        value: '',
        disabled: false
      }, [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(100)
      ]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid && !this.isLocked) {
      this.isLoading = true;
      this.errorMessage = '';
      this.apiError = null;

      const loginRequest: LoginRequest = this.loginForm.value;
      
      this.authService.login(loginRequest).subscribe({
        next: () => {
          this.isLoading = false;
          this.attempts = 0;
          this.isLocked = false;
          this.router.navigate(['/products']);
        },
        error: (error) => {
          this.isLoading = false;
          this.attempts++;
          
          this.apiError = this.errorHandler.handleError(error);
          this.errorMessage = this.apiError.message;
          
          if (this.attempts >= this.maxAttempts) {
            this.lockAccount();
          }
        }
      });
    } else if (this.isLocked) {
      this.errorMessage = 'Account is temporarily locked due to too many failed attempts.';
    }
  }

  private lockAccount(): void {
    this.isLocked = true;
    this.loginForm.disable();
    this.lockoutTime = new Date(Date.now() + 15 * 60 * 1000); // 15 minutes
    this.errorMessage = 'Account locked for 15 minutes due to too many failed attempts.';

    setTimeout(() => {
      this.isLocked = false;
      this.attempts = 0;
      this.lockoutTime = null;
      this.loginForm.enable();
    }, 15 * 60 * 1000);
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  getFieldError(fieldName: string): string | null {
    const control = this.loginForm.get(fieldName);
    if (control && control.invalid && control.touched) {
      return ValidationService.getErrorMessage(control.errors!, fieldName);
    }
    return null;
  }

  get userName() { return this.loginForm.get('userName'); }
  get password() { return this.loginForm.get('password'); }
}
