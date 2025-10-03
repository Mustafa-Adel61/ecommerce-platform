import { Injectable } from '@angular/core';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class ValidationService {

  // Custom validators
  static noWhitespace(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const hasWhitespace = /\s/.test(control.value);
      return hasWhitespace ? { whitespace: { value: control.value } } : null;
    };
  }

  static noSpecialCharacters(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const hasSpecialChars = /[!@#$%^&*(),.?":{}|<>]/.test(control.value);
      return hasSpecialChars ? { specialCharacters: { value: control.value } } : null;
    };
  }

  static alphanumericOnly(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const isAlphanumeric = /^[a-zA-Z0-9]+$/.test(control.value);
      return !isAlphanumeric ? { alphanumeric: { value: control.value } } : null;
    };
  }

  static strongPassword(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const password = control.value;
      const hasUpperCase = /[A-Z]/.test(password);
      const hasLowerCase = /[a-z]/.test(password);
      const hasNumbers = /\d/.test(password);
      const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);
      
      const errors: any = {};
      
      if (!hasUpperCase) errors.uppercase = true;
      if (!hasLowerCase) errors.lowercase = true;
      if (!hasNumbers) errors.numbers = true;
      if (!hasSpecialChar) errors.specialChar = true;
      
      return Object.keys(errors).length > 0 ? { strongPassword: errors } : null;
    };
  }

  static fileSize(maxSizeInMB: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const file = control.value as File;
      if (file && file.size > maxSizeInMB * 1024 * 1024) {
        return { fileSize: { maxSize: maxSizeInMB, actualSize: file.size } };
      }
      
      return null;
    };
  }

  static fileType(allowedTypes: string[]): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const file = control.value as File;
      if (file && !allowedTypes.includes(file.type)) {
        return { fileType: { allowedTypes, actualType: file.type } };
      }
      
      return null;
    };
  }

  static priceRange(min: number, max: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const price = parseFloat(control.value);
      if (isNaN(price) || price < min || price > max) {
        return { priceRange: { min, max, actual: price } };
      }
      
      return null;
    };
  }

  static uniqueCode(existingCodes: string[]): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const code = control.value.toLowerCase();
      const isDuplicate = existingCodes.some(existing => 
        existing.toLowerCase() === code
      );
      
      return isDuplicate ? { uniqueCode: { value: control.value } } : null;
    };
  }

  // Helper methods for error messages
  static getErrorMessage(error: ValidationErrors, fieldName: string): string {
    if (error['required']) {
      return `${fieldName} is required`;
    }
    
    if (error['email']) {
      return 'Please enter a valid email address';
    }
    
    if (error['minlength']) {
      return `${fieldName} must be at least ${error['minlength'].requiredLength} characters`;
    }
    
    if (error['maxlength']) {
      return `${fieldName} must not exceed ${error['maxlength'].requiredLength} characters`;
    }
    
    if (error['min']) {
      return `${fieldName} must be at least ${error['min'].min}`;
    }
    
    if (error['max']) {
      return `${fieldName} must not exceed ${error['max'].max}`;
    }
    
    if (error['whitespace']) {
      return `${fieldName} cannot contain whitespace`;
    }
    
    if (error['specialCharacters']) {
      return `${fieldName} cannot contain special characters`;
    }
    
    if (error['alphanumeric']) {
      return `${fieldName} can only contain letters and numbers`;
    }
    
    if (error['strongPassword']) {
      const errors = error['strongPassword'];
      const requirements = [];
      if (errors.uppercase) requirements.push('uppercase letter');
      if (errors.lowercase) requirements.push('lowercase letter');
      if (errors.numbers) requirements.push('number');
      if (errors.specialChar) requirements.push('special character');
      return `Password must contain at least one ${requirements.join(', ')}`;
    }
    
    if (error['fileSize']) {
      const maxSize = error['fileSize'].maxSize;
      return `File size must not exceed ${maxSize}MB`;
    }
    
    if (error['fileType']) {
      const allowedTypes = error['fileType'].allowedTypes.join(', ');
      return `File type must be one of: ${allowedTypes}`;
    }
    
    if (error['priceRange']) {
      const { min, max } = error['priceRange'];
      return `Price must be between $${min} and $${max}`;
    }
    
    if (error['uniqueCode']) {
      return 'This code is already in use';
    }
    
    if (error['passwordMismatch']) {
      return 'Passwords do not match';
    }
    
    return `${fieldName} is invalid`;
  }
}
