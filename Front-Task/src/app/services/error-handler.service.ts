import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

export interface ValidationError {
  field: string;
  message: string;
}

export interface ApiError {
  message: string;
  errors?: ValidationError[];
  statusCode?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  
  handleError(error: any): ApiError {
    console.error('Error occurred:', error);

    if (error instanceof HttpErrorResponse) {
      return this.handleHttpError(error);
    }

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      return {
        message: 'A network error occurred. Please check your connection and try again.',
        statusCode: 0
      };
    }

    // Generic error
    return {
      message: 'An unexpected error occurred. Please try again.',
      statusCode: 500
    };
  }

  private handleHttpError(error: HttpErrorResponse): ApiError {
    const apiError: ApiError = {
      message: 'An error occurred',
      statusCode: error.status
    };

    switch (error.status) {
      case 400:
        apiError.message = this.extractBadRequestMessage(error);
        break;
      case 401:
        apiError.message = 'Authentication failed. Please check your credentials.';
        break;
      case 403:
        apiError.message = 'You do not have permission to perform this action.';
        break;
      case 404:
        apiError.message = 'The requested resource was not found.';
        break;
      case 409:
        apiError.message = 'A conflict occurred. The resource may already exist.';
        break;
      case 422:
        apiError.message = 'Validation failed. Please check your input.';
        apiError.errors = this.extractValidationErrors(error);
        break;
      case 500:
        apiError.message = 'A server error occurred. Please try again later.';
        break;
      case 0:
        apiError.message = 'Unable to connect to the server. Please check your connection.';
        break;
      default:
        apiError.message = error.error?.message || `An error occurred (${error.status}).`;
    }

    return apiError;
  }

  private extractBadRequestMessage(error: HttpErrorResponse): string {
    if (typeof error.error === 'string') {
      return error.error;
    }
    
    if (error.error?.message) {
      return error.error.message;
    }

    if (error.error?.errors && Array.isArray(error.error.errors)) {
      return error.error.errors.map((e: any) => e.message || e).join(', ');
    }

    return 'Invalid request. Please check your input.';
  }

  private extractValidationErrors(error: HttpErrorResponse): ValidationError[] {
    if (error.error?.errors && Array.isArray(error.error.errors)) {
      return error.error.errors.map((e: any) => ({
        field: e.field || e.property || 'unknown',
        message: e.message || e.error || 'Invalid value'
      }));
    }

    if (error.error?.errors && typeof error.error.errors === 'object') {
      return Object.keys(error.error.errors).map(field => ({
        field,
        message: Array.isArray(error.error.errors[field]) 
          ? error.error.errors[field].join(', ')
          : error.error.errors[field]
      }));
    }

    return [];
  }

  getFieldError(errors: ValidationError[], fieldName: string): string | null {
    if (!errors || !Array.isArray(errors)) {
      return null;
    }

    const fieldError = errors.find(error => 
      error.field.toLowerCase() === fieldName.toLowerCase()
    );

    return fieldError ? fieldError.message : null;
  }
}
