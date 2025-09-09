import { ErrorHandler, Injectable, NgZone } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from './services/auth.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(
    private snackBar: MatSnackBar,
    private authService: AuthService,
    private ngZone: NgZone
  ) {}

  handleError(error: any): void {
    console.error('Global error:', error);

    this.ngZone.run(() => {
      // Handle authentication errors
      if (error.status === 401) {
        this.authService.logout();
        this.snackBar.open('Session expired. Please log in again.', 'Close', {
          duration: 5000,
          panelClass: ['warning-snackbar']
        });
        return;
      }

      // Handle other errors
      let message = 'An unexpected error occurred';
      
      if (error.message) {
        message = error.message;
      } else if (error.error?.message) {
        message = error.error.message;
      }

      this.snackBar.open(message, 'Close', {
        duration: 5000,
        panelClass: ['error-snackbar']
      });
    });
  }
}
