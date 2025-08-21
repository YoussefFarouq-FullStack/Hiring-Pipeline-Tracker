import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CandidateService } from '../../services/candidate.service';
import { Candidate } from '../../models/candidate.model';
import { CandidateDialogComponent } from './candidate-dialog/candidate-dialog';

@Component({
  selector: 'app-candidates',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatDialogModule, MatSnackBarModule],
  templateUrl: './candidates.html',
  styleUrl: './candidates.scss'
})
export class CandidatesComponent implements OnInit {
  candidates: Candidate[] = [];
  dataSource: MatTableDataSource<Candidate>;
  displayedColumns: string[] = ['id', 'name', 'phone', 'status', 'actions'];
  
  // Loading and error states
  isLoading = false;
  hasError = false;
  errorMessage = '';

  // Computed properties for stats
  get totalCandidates(): number {
    return this.candidates.length;
  }

  get activeCandidates(): number {
    return this.candidates.filter(c => c.status && (c.status === 'Applied' || c.status === 'Screening' || c.status === 'Interviewing')).length;
  }

  get inProgressCandidates(): number {
    return this.candidates.filter(c => c.status && c.status === 'Interviewing').length;
  }

  get hiredCandidates(): number {
    return this.candidates.filter(c => c.status && c.status === 'Hired').length;
  }

  // Safe access to dataSource data
  get dataSourceData(): Candidate[] {
    return this.dataSource?.data || [];
  }

  get dataSourceLength(): number {
    return this.dataSourceData.length;
  }

  get hasTableData(): boolean {
    return this.dataSourceData.length > 0;
  }

  // Safe access to first candidate
  get firstCandidateId(): string | number {
    return this.candidates && this.candidates.length > 0 ? this.candidates[0].candidateId : 'None';
  }

  constructor(
    private candidateService: CandidateService, 
    public dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new MatTableDataSource<Candidate>([]);
  }

  ngOnInit(): void {
    this.loadCandidates();
  }

  loadCandidates(): void {
    this.isLoading = true;
    this.hasError = false;
    this.errorMessage = '';
    
    this.candidateService.getCandidates().subscribe({
      next: (data: Candidate[]) => {
        // Ensure data is an array and handle potential null/undefined
        if (Array.isArray(data)) {
          // Load candidates as-is from the backend
          this.candidates = data;
          
          // Safely update dataSource
          if (this.dataSource) {
            this.dataSource.data = this.candidates;
          } else {
            this.dataSource = new MatTableDataSource<Candidate>(this.candidates);
          }
        } else {
          this.candidates = [];
          if (this.dataSource) {
            this.dataSource.data = [];
          }
        }
        
        this.isLoading = false;
      },
      error: (error: Error) => {
        console.error('Error loading candidates:', error);
        this.hasError = true;
        this.errorMessage = error.message || 'Failed to load candidates. Please try again.';
        this.isLoading = false;
        this.showError(this.errorMessage);
      }
    });
  }

  addCandidate() {
    const dialogRef = this.dialog.open(CandidateDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      maxHeight: '90vh',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('Dialog result:', result);
      if (result) {
        console.log('Creating candidate with data:', result);
        this.isLoading = true;
        this.candidateService.createCandidate(result).subscribe({
          next: () => {
            console.log('Candidate created successfully');
            this.showSuccess('Candidate created successfully!');
            this.loadCandidates();
          },
          error: (error) => {
            console.error('Failed to create candidate:', error);
            this.showError(error.message || 'Failed to create candidate. Please try again.');
            this.isLoading = false;
          }
        });
      }
    });
  }

  editCandidate(candidate: Candidate) {
    console.log('Editing candidate:', candidate);
    console.log('Candidate status:', candidate.status);

    const dialogRef = this.dialog.open(CandidateDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      maxHeight: '90vh',
      data: { mode: 'edit', candidate }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('Edit dialog result:', result);
      if (result) {
        console.log('Updating candidate with data:', result);
        console.log('Status in result:', result.status);
        
        // Ensure all required fields are included in the update
        const updateData: Candidate = {
          candidateId: candidate.candidateId,
          firstName: result.firstName || candidate.firstName,
          lastName: result.lastName || candidate.lastName,
          email: result.email || candidate.email,
          phone: result.phone || candidate.phone,
          status: result.status || candidate.status || 'Applied'
        };
        
        console.log('Final update data:', updateData);
        
        this.isLoading = true;
        this.candidateService.updateCandidate(candidate.candidateId, updateData).subscribe({
          next: () => {
            console.log('Candidate updated successfully');
            this.showSuccess('Candidate updated successfully!');
            this.loadCandidates();
          },
          error: (error) => {
            console.error('Failed to update candidate:', error);
            this.showError(error.message || 'Failed to update candidate. Please try again.');
            this.isLoading = false;
          }
        });
      }
    });
  }

  deleteCandidate(id: number) {
    if (confirm('Are you sure you want to delete this candidate? This action cannot be undone.')) {
      this.isLoading = true;
      this.candidateService.deleteCandidate(id).subscribe({
        next: () => {
          this.showSuccess('Candidate deleted successfully!');
          this.loadCandidates();
        },
        error: (error) => {
          console.error('Failed to delete candidate:', error);
          this.showError(error.message || 'Failed to delete candidate. Please try again.');
          this.isLoading = false;
        }
      });
    }
  }

  retryLoad(): void {
    this.loadCandidates();
  }

  private showSuccess(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['success-snackbar']
    });
  }

  private showError(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['error-snackbar']
    });
  }
}
