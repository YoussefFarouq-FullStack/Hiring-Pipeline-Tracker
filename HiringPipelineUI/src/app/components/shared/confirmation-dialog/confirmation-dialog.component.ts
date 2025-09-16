import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

export interface ConfirmationDialogData {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: 'danger' | 'warning' | 'info' | 'success';
}

@Component({
  selector: 'app-confirmation-dialog',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  template: `
    <div class="confirmation-dialog">
      <!-- Header -->
      <div class="dialog-header" [ngClass]="getHeaderClass()">
        <div class="icon-container">
          <svg class="dialog-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <ng-container [ngSwitch]="data.type">
              <!-- Danger Icon -->
              <path *ngSwitchCase="'danger'" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                    d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
              <!-- Warning Icon -->
              <path *ngSwitchCase="'warning'" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                    d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              <!-- Info Icon -->
              <path *ngSwitchCase="'info'" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                    d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              <!-- Success Icon -->
              <path *ngSwitchCase="'success'" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                    d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              <!-- Default Icon -->
              <path *ngSwitchDefault stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                    d="M8.228 9c.549-1.165 2.03-2 3.772-2 2.21 0 4 1.343 4 3 0 1.4-1.278 2.575-3.006 2.907-.542.104-.994.54-.994 1.093m0 3h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </ng-container>
          </svg>
        </div>
        <h2 class="dialog-title">{{ data.title }}</h2>
      </div>

      <!-- Content -->
      <div class="dialog-content">
        <p class="dialog-message">{{ data.message }}</p>
      </div>

      <!-- Footer -->
      <div class="dialog-footer">
        <button
          type="button"
          (click)="onCancel()"
          class="cancel-button"
        >
          {{ data.cancelText || 'Cancel' }}
        </button>
        <button
          type="button"
          (click)="onConfirm()"
          class="confirm-button"
          [ngClass]="getConfirmButtonClass()"
        >
          {{ data.confirmText || 'Confirm' }}
        </button>
      </div>
    </div>
  `,
  styles: [`
    .confirmation-dialog {
      background: white;
      border-radius: 1.5rem;
      box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
      max-width: 500px;
      width: 100%;
      overflow: hidden;
      transform: scale(1);
      transition: all 0.3s ease;
    }

    .dialog-header {
      padding: 2rem 2rem 1rem 2rem;
      text-align: center;
      position: relative;
      overflow: hidden;
    }

    .dialog-header.danger {
      background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
    }

    .dialog-header.warning {
      background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
    }

    .dialog-header.info {
      background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
    }

    .dialog-header.success {
      background: linear-gradient(135deg, #10b981 0%, #059669 100%);
    }

    .dialog-header.default {
      background: linear-gradient(135deg, #6b7280 0%, #4b5563 100%);
    }

    .icon-container {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 4rem;
      height: 4rem;
      background: rgba(255, 255, 255, 0.2);
      border-radius: 1rem;
      margin-bottom: 1rem;
      backdrop-filter: blur(10px);
    }

    .dialog-icon {
      width: 2rem;
      height: 2rem;
      color: white;
    }

    .dialog-title {
      color: white;
      font-size: 1.5rem;
      font-weight: 700;
      margin: 0;
      text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .dialog-content {
      padding: 1.5rem 2rem;
    }

    .dialog-message {
      color: #374151;
      font-size: 1rem;
      line-height: 1.6;
      margin: 0;
      text-align: center;
    }

    .dialog-footer {
      padding: 1rem 2rem 2rem 2rem;
      display: flex;
      gap: 1rem;
      justify-content: flex-end;
    }

    .cancel-button,
    .confirm-button {
      padding: 0.75rem 1.5rem;
      border-radius: 0.75rem;
      font-weight: 600;
      font-size: 0.875rem;
      border: 2px solid transparent;
      cursor: pointer;
      transition: all 0.3s ease;
      min-width: 100px;
    }

    .cancel-button {
      background: white;
      color: #6b7280;
      border-color: #d1d5db;
    }

    .cancel-button:hover {
      background: #f9fafb;
      border-color: #9ca3af;
      transform: translateY(-1px);
    }

    .confirm-button {
      color: white;
      font-weight: 700;
    }

    .confirm-button.danger {
      background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
    }

    .confirm-button.warning {
      background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
    }

    .confirm-button.info {
      background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
    }

    .confirm-button.success {
      background: linear-gradient(135deg, #10b981 0%, #059669 100%);
    }

    .confirm-button.default {
      background: linear-gradient(135deg, #6b7280 0%, #4b5563 100%);
    }

    .confirm-button:hover {
      transform: translateY(-2px);
      box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.2);
    }

    .confirm-button:active {
      transform: translateY(0);
    }

    /* Responsive */
    @media (max-width: 640px) {
      .confirmation-dialog {
        margin: 1rem;
        max-width: calc(100vw - 2rem);
      }

      .dialog-header {
        padding: 1.5rem 1.5rem 1rem 1.5rem;
      }

      .dialog-content {
        padding: 1rem 1.5rem;
      }

      .dialog-footer {
        padding: 1rem 1.5rem 1.5rem 1.5rem;
        flex-direction: column;
      }

      .cancel-button,
      .confirm-button {
        width: 100%;
      }
    }
  `]
})
export class ConfirmationDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ConfirmationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmationDialogData
  ) {}

  onConfirm(): void {
    this.dialogRef.close(true);
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  getHeaderClass(): string {
    return this.data.type || 'default';
  }

  getConfirmButtonClass(): string {
    return this.data.type || 'default';
  }
}
