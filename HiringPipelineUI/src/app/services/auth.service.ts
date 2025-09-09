import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError, of } from 'rxjs';
import { catchError, tap, delay } from 'rxjs/operators';
import { Router } from '@angular/router';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken?: string;
  user: {
    id: number;
    username: string;
    email: string;
    role: string;
  };
  expiresAt: string;
}

export interface User {
  id: number;
  username: string;
  email: string;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = '/api/auth';
  private tokenKey = 'hiring_pipeline_token';
  private refreshTokenKey = 'hiring_pipeline_refresh_token';
  private userKey = 'hiring_pipeline_user';

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  private refreshTokenSubject = new BehaviorSubject<string | null>(null);
  public refreshToken$ = this.refreshTokenSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadStoredAuth();
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred during authentication';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Authentication Error: ${error.error.message}`;
      console.error('Client-side error:', error.error);
    } else {
      // Server-side error
      console.error('Server-side error:', error);

      if (error.status === 0) {
        errorMessage = 'Unable to connect to server. Please check your internet connection.';
      } else if (error.status === 401) {
        // Handle token expiry or invalid token
        this.handleUnauthorized();
        errorMessage = 'Session expired. Please log in again.';
      } else if (error.status === 403) {
        errorMessage = 'Access denied. Your account may be locked.';
      } else if (error.status === 404) {
        errorMessage = 'Authentication service not found.';
      } else if (error.status >= 500) {
        errorMessage = 'Server error. Please try again later.';
      } else if (error.error && typeof error.error === 'object') {
        if (error.error.message) {
          errorMessage = error.error.message;
        } else if (error.error.errors) {
          errorMessage = `Validation errors: ${JSON.stringify(error.error.errors)}`;
        }
      } else if (error.error && typeof error.error === 'string') {
        errorMessage = error.error;
      }
    }

    return throwError(() => new Error(errorMessage));
  }

  private handleUnauthorized(): void {
    console.log('Unauthorized access detected, clearing auth and redirecting to login');
    this.clearAuth();
    this.currentUserSubject.next(null);
    this.router.navigate(['/login'], { 
      queryParams: { reason: 'session_expired' } 
    });
  }

  private loadStoredAuth(): void {
    const token = this.getToken();
    const refreshToken = this.getRefreshToken();
    const user = this.getStoredUser();

    if (token && user) {
      // Check if token is still valid
      if (this.isTokenValid(token)) {
        this.currentUserSubject.next(user);
        if (refreshToken) {
          this.refreshTokenSubject.next(refreshToken);
        }
      } else if (refreshToken) {
        // Try to refresh the token
        this.refreshToken().subscribe({
          next: () => console.log('Token refreshed successfully'),
          error: () => this.clearAuth()
        });
      } else {
        this.clearAuth();
      }
    }
  }

  private isTokenValid(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = payload.exp * 1000; // Convert to milliseconds
      return Date.now() < expirationTime;
    } catch (error) {
      console.error('Error parsing token:', error);
      return false;
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    console.log('AuthService.login called with:', { username: credentials.username });

    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        console.log('Login successful, storing auth data');
        this.storeAuth(response);
        this.currentUserSubject.next(response.user);
      }),
      catchError(this.handleError)
    );
  }


  logout(): void {
    console.log('Logging out user');
    this.clearAuth();
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    const user = this.currentUserSubject.value;
    return !!(token && user && this.isTokenValid(token));
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private storeAuth(response: LoginResponse): void {
    localStorage.setItem(this.tokenKey, response.token);
    localStorage.setItem(this.userKey, JSON.stringify(response.user));
    
    if (response.refreshToken) {
      localStorage.setItem(this.refreshTokenKey, response.refreshToken);
      this.refreshTokenSubject.next(response.refreshToken);
    }
  }

  private getStoredUser(): User | null {
    const userStr = localStorage.getItem(this.userKey);
    if (userStr) {
      try {
        return JSON.parse(userStr);
      } catch (error) {
        console.error('Error parsing stored user:', error);
        return null;
      }
    }
    return null;
  }

  private clearAuth(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    localStorage.removeItem(this.refreshTokenKey);
    this.refreshTokenSubject.next(null);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  // Method to refresh token (if needed)
  refreshToken(): Observable<LoginResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      return throwError(() => new Error('No refresh token available'));
    }

    return this.http.post<LoginResponse>(`${this.apiUrl}/refresh`, { 
      refreshToken: refreshToken 
    }).pipe(
      tap(response => {
        console.log('Token refreshed successfully');
        this.storeAuth(response);
        this.currentUserSubject.next(response.user);
      }),
      catchError(error => {
        console.error('Token refresh failed:', error);
        this.clearAuth();
        this.handleUnauthorized();
        return throwError(() => new Error('Token refresh failed'));
      })
    );
  }

  // Method to check if user has specific role
  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }

  // Method to check if user has any of the specified roles
  hasAnyRole(roles: string[]): boolean {
    const user = this.getCurrentUser();
    return user ? roles.includes(user.role) : false;
  }
}
