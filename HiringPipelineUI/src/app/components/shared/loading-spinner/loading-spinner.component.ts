import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="loading-container" [ngClass]="containerClass">
      <div class="loading-content">
        <!-- Animated Spinner -->
        <div class="spinner-container">
          <div class="spinner-ring">
            <div class="spinner-ring-inner"></div>
          </div>
          <div class="spinner-dots">
            <div class="dot dot-1"></div>
            <div class="dot dot-2"></div>
            <div class="dot dot-3"></div>
          </div>
        </div>
        
        <!-- Loading Text -->
        <div class="loading-text">
          <h3 class="loading-title">{{ title || 'Loading...' }}</h3>
          <p class="loading-subtitle">{{ subtitle || 'Please wait while we fetch the data' }}</p>
        </div>
        
        <!-- Progress Bar (optional) -->
        <div *ngIf="showProgress" class="progress-container">
          <div class="progress-bar">
            <div class="progress-fill" [style.width.%]="progress"></div>
          </div>
          <span class="progress-text">{{ progress }}%</span>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./loading-spinner.component.scss']
})
export class LoadingSpinnerComponent {
  @Input() title: string = 'Loading...';
  @Input() subtitle: string = 'Please wait while we fetch the data';
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Input() variant: 'default' | 'minimal' | 'card' = 'default';
  @Input() showProgress: boolean = false;
  @Input() progress: number = 0;
  @Input() containerClass: string = '';

  get spinnerSize(): string {
    switch (this.size) {
      case 'small': return 'w-8 h-8';
      case 'large': return 'w-16 h-16';
      default: return 'w-12 h-12';
    }
  }
}
