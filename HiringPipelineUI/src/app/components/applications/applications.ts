import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApplicationService } from '../../services/application.service';
import { Application } from '../../models/application.model';
import { ApplicationDialogComponent } from './application-dialog/application-dialog';

@Component({
  selector: 'app-applications',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatDialogModule, MatSnackBarModule],
  templateUrl: './applications.html',
  styleUrl: './applications.scss'
})
export class ApplicationsComponent implements OnInit {
  applications: Application[] = [];
  dataSource: MatTableDataSource<Application>;
  displayedColumns: string[] = ['id', 'candidateId', 'requisitionId', 'currentStage', 'status', 'actions'];
  
  isLoading = false;
  hasError = false;
  errorMessage = '';

  constructor(
    private appService: ApplicationService, 
    public dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new MatTableDataSource<Application>([]);
  }

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications(): void {
    this.isLoading = true;
    this.hasError = false;
    this.errorMessage = '';

    this.appService.getApplications().subscribe({
      next: (data: Application[]) => {
        if (Array.isArray(data)) {
          this.applications = data;
          this.dataSource.data = this.applications;
        } else {
          this.applications = [];
          this.dataSource.data = [];
        }
        this.isLoading = false;
      },
      error: (error: Error) => {
        console.error('Error loading applications:', error);
        this.hasError = true;
        this.errorMessage = error.message || 'Failed to load applications. Please try again.';
        this.isLoading = false;
        this.showError(this.errorMessage);
      }
    });
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(ApplicationDialogComponent, {
      width: '500px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadApplications();
        this.showSuccess('Application created successfully!');
      }
    });
  }

  deleteApplication(id: number): void {
    if (confirm('Are you sure you want to delete this application?')) {
      this.appService.deleteApplication(id).subscribe({
        next: () => {
          this.loadApplications();
          this.showSuccess('Application deleted successfully!');
        },
        error: (error: Error) => {
          console.error('Error deleting application:', error);
          this.showError('Failed to delete application. Please try again.');
        }
      });
    }
  }

  private showSuccess(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['success-snackbar']
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['error-snackbar']
    });
  }

  retryLoad(): void {
    this.loadApplications();
  }

  // Safe access to dataSource data
  get dataSourceData(): Application[] {
    return this.dataSource?.data || [];
  }

  get dataSourceLength(): number {
    return this.dataSourceData.length;
  }

  get hasTableData(): boolean {
    return this.dataSourceData.length > 0;
  }

  // Computed properties for stats
  get totalApplications(): number {
    return this.applications.length;
  }

  get activeApplications(): number {
    return this.applications.filter(app => app.status === 'Active').length;
  }

  get inProgressApplications(): number {
    return this.applications.filter(app => app.currentStage === 'Interviewing').length;
  }

  get completedApplications(): number {
    return this.applications.filter(app => app.status === 'Completed').length;
  }
}
