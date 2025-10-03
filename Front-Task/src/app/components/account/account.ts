import { CommonModule, NgIf } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user.model';
import { ApiError, ErrorHandlerService } from '../../services/error-handler.service';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-account',
  imports: [NgIf,CommonModule],
  templateUrl: './account.html',
  styleUrl: './account.css'
})
export class Account implements OnInit {

 user: User | null = null;
  isLoading = false;
  errorMessage = '';
  apiError: ApiError | null = null;

  constructor(
    private accountService: AccountService,
    private errorHandler: ErrorHandlerService
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isLoading = true;
    this.accountService.getProfile().subscribe({
      next: (data) => {
        this.user = data;
        this.isLoading = false;
      },

      error: (error) => {
        this.apiError = this.errorHandler.handleError(error);
        this.errorMessage = this.apiError.message;
        this.isLoading = false;
      }
    });
  }
}
