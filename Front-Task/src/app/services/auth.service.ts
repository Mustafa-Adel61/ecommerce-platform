import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { LoginRequest, RegisterRequest, AuthResponse, RefreshRequest, RevokeRequest } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  private tokenKey = 'accessToken';
  private refreshTokenKey = 'refreshToken';
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) {}

  get isAuthenticated$(): Observable<boolean> {
    return this.isAuthenticatedSubject.asObservable();
  }

  get isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, credentials)
      .pipe(
        tap(response => {
          this.setTokens(response);
          this.isAuthenticatedSubject.next(true);
        })
      );
  }

  register(userData: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/register`, userData).pipe(
      tap(response => response),
      catchError(error => {
        console.error('Registration error:', error);
        throw error;
      })
    );
  }

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = localStorage.getItem(this.refreshTokenKey);
    if (!refreshToken) {
      throw new Error('No refresh token available');
    }

    const request: RefreshRequest = { refreshToken };
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/refresh`, request)
      .pipe(
        tap(response => {
          this.setTokens(response);
        })
      );
  }

  logout(): Observable<any> {
    const refreshToken = localStorage.getItem(this.refreshTokenKey);
    
    // Always clear tokens locally first
    this.clearTokens();
    this.isAuthenticatedSubject.next(false);
    
    // If we have a refresh token, try to revoke it on the server
    if (refreshToken) {
      const request: RevokeRequest = { refreshToken };
      return this.http.post(`${this.apiUrl}/auth/revoke`, request)
        .pipe(
          tap(() => {
            // Success - token was revoked
            }),
          // Handle errors gracefully - even if revoke fails, we're still logged out locally
            catchError((error) => {
            
            // Return a successful observable since we've already cleared tokens locally
            return new Observable(observer => observer.next(null));
          })
        );
    }
    
    // No refresh token to revoke, just return success
    return new Observable(observer => observer.next(null));
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private setTokens(response: AuthResponse): void {
    localStorage.setItem(this.tokenKey, response.accessToken);
    localStorage.setItem(this.refreshTokenKey, response.refreshToken);
  }

  private clearTokens(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshTokenKey);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
}
