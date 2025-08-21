import { Requisition } from './../../models/requisition.model';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { RequisitionService } from '../../services/requisition.service';
import { RequisitionDialogComponent } from './requisition-dialog/requisition-dialog';

@Component({
  selector: 'app-requisitions',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatDialogModule, MatSnackBarModule],
  templateUrl: './requisitions.html',
  styleUrl: './requisitions.scss'
})
export class RequisitionsComponent implements OnInit {
  requisitions: Requisition[] = [];
  dataSource: MatTableDataSource<Requisition>;
  displayedColumns: string[] = ['id', 'title', 'department', 'status', 'actions'];
  
  // Loading and error states
  isLoading = false;
  hasError = false;
  errorMessage = '';

  // Computed properties for stats
  get totalRequisitions(): number {
    return this.requisitions.length;
  }

  get openRequisitions(): number {
    return this.requisitions.filter(r => r.status === 'Open').length;
  }

  get inProgressRequisitions(): number {
    return this.requisitions.filter(r => r.status === 'In Progress').length;
  }

  get closedRequisitions(): number {
    return this.requisitions.filter(r => r.status === 'Closed').length;
  }

  // Safe access to dataSource data
  get dataSourceData(): Requisition[] {
    return this.dataSource?.data || [];
  }

  get dataSourceLength(): number {
    return this.dataSourceData.length;
  }

  get hasTableData(): boolean {
    return this.dataSourceData.length > 0;
  }

  constructor(
    private requisitionService: RequisitionService, 
    public dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new MatTableDataSource<Requisition>([]);
  }

  ngOnInit(): void {
    this.loadRequisitions();
  }

  loadRequisitions(): void {
    this.isLoading = true;
    this.hasError = false;
    this.errorMessage = '';
    
    this.requisitionService.getRequisitions().subscribe({
      next: (data: Requisition[]) => {
        // Ensure data is an array and handle potential null/undefined
        if (Array.isArray(data)) {
          this.requisitions = data;
          this.dataSource.data = this.requisitions;
        } else {
          console.warn('Expected array but received:', data);
          this.requisitions = [];
          this.dataSource.data = [];
        }
        this.isLoading = false;
      },
      error: (error: Error) => {
        console.error('Error loading requisitions:', error);
        this.hasError = true;
        this.errorMessage = error.message || 'Failed to load requisitions. Please try again.';
        this.isLoading = false;
        this.showError(this.errorMessage);
      }
    });
  }

  addRequisition(): void {
    const dialogRef = this.dialog.open(RequisitionDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      maxHeight: '90vh',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe((result: Partial<Requisition> | undefined) => {
      if (result) {
        this.isLoading = true;
        this.requisitionService.createRequisition(result as Omit<Requisition, 'requisitionId'>).subscribe({
          next: () => {
            this.showSuccess('Requisition created successfully!');
            this.loadRequisitions();
          },
          error: (error: Error) => {
            console.error('Failed to create requisition:', error);
            this.showError(error.message || 'Failed to create requisition. Please try again.');
            this.isLoading = false;
          }
        });
      }
    });
  }

  editRequisition(requisition: Requisition): void {
    if (!requisition || !requisition.requisitionId) {
      console.error('Invalid requisition data for editing:', requisition);
      this.showError('Invalid requisition data. Please try again.');
      return;
    }

    console.log('Editing requisition:', requisition);

    const dialogRef = this.dialog.open(RequisitionDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      maxHeight: '90vh',
      data: { mode: 'edit', requisition }
    });

    dialogRef.afterClosed().subscribe((result: Partial<Requisition> | undefined) => {
      if (result) {
        console.log('Updating requisition with data:', result);
        this.isLoading = true;
        
        // Merge the existing requisition with the updated data
        const updatedRequisition: Requisition = {
          ...requisition,
          ...result
        };
        
        this.requisitionService.updateRequisition(requisition.requisitionId, updatedRequisition).subscribe({
          next: () => {
            this.showSuccess('Requisition updated successfully!');
            this.loadRequisitions();
          },
          error: (error: Error) => {
            console.error('Failed to update requisition:', error);
            this.showError(error.message || 'Failed to update requisition. Please try again.');
            this.isLoading = false;
          }
        });
      }
    });
  }

  deleteRequisition(id: number): void {
    if (!id || isNaN(id)) {
      console.error('Invalid requisition ID for deletion:', id);
      this.showError('Invalid requisition ID. Please try again.');
      return;
    }

    if (confirm("Are you sure you want to delete this requisition? This action cannot be undone.")) {
      this.isLoading = true;
      this.requisitionService.deleteRequisition(id).subscribe({
        next: () => {
          this.showSuccess('Requisition deleted successfully!');
          this.loadRequisitions();
        },
        error: (error: Error) => {
          console.error('Failed to delete requisition:', error);
          this.showError(error.message || 'Failed to delete requisition. Please try again.');
          this.isLoading = false;
        }
      });
    }
  }

  retryLoad(): void {
    this.loadRequisitions();
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
}
