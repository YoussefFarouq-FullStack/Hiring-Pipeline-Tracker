import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Candidate } from '../../models/candidate.model';
import { CandidateService } from '../../services/candidate.service';
import { ApplicationService } from '../../services/application.service';
import { AuditLogService } from '../../services/audit-log.service';
import { Application } from '../../models/application.model';
import { Requisition } from '../../models/requisition.model';
import { CandidateDialogComponent } from './candidate-dialog/candidate-dialog';
import { CandidateDetailComponent } from './candidate-detail/candidate-detail';
import { ConfirmationDialogService } from '../shared/confirmation-dialog/confirmation-dialog.service';
import { AuthService } from '../../services/auth.service';
import { LoadingSpinnerComponent } from '../shared/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-candidates',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatCardModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatChipsModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    LoadingSpinnerComponent
],
  templateUrl: './candidates.html',
  styleUrls: ['./candidates.scss']
})
export class CandidatesComponent implements OnInit {
  candidates: Candidate[] = [];
  filteredCandidates: Candidate[] = [];
  pagedCandidates: Candidate[] = [];
  applications: Application[] = [];
  searchTerm: string = '';
  selectedStatus: string = '';
  selectedRequisition: string = '';
  selectedStat: string = 'all'; // New property for stats filtering
  statuses: string[] = ['Applied', 'Interviewing', 'Hired', 'Rejected', 'Withdrawn'];
  requisitions: Requisition[] = [];
  isLoading = false;
  hasError = false;
  errorMessage = '';

  // Pagination
  pageSize = 6;
  currentPage = 0;
  totalItems = 0;

  constructor(
    private candidateService: CandidateService, 
    private applicationService: ApplicationService,
    private auditLogService: AuditLogService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private authService: AuthService,
    private router: Router,
    private confirmationDialog: ConfirmationDialogService
  ) {}

  ngOnInit(): void {
    // Note: Audit logging is now handled by the middleware automatically
    // The middleware will log "View Candidates" when the candidates page is accessed
    // and log background data fetches as "BackgroundFetch" type
    
    this.loadCandidates();
    this.loadApplications();
    this.loadRequisitions();
  }

  loadCandidates(): void {
    this.isLoading = true;
    this.hasError = false;
    this.errorMessage = '';
    
    this.candidateService.getCandidates().subscribe({
      next: (data: Candidate[]) => {
          this.candidates = data;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (error: Error) => {
        console.error('Error loading candidates:', error);
        this.hasError = true;
        this.errorMessage = error.message || 'Failed to load candidates';
        this.isLoading = false;
        this.showError(this.errorMessage);
      }
    });
  }

  loadApplications(): void {
    this.applicationService.getApplications().subscribe({
      next: (data: Application[]) => {
        this.applications = data;
        this.applyFilters(); // Refresh filters to update requisition and applied data
      },
      error: (error: Error) => {
        console.error('Error loading applications:', error);
        // Don't show error for applications, just log it
      }
    });
  }

  loadRequisitions(): void {
    this.applicationService.getRequisitions().subscribe({
      next: (data: Requisition[]) => {
        this.requisitions = data;
        this.applyFilters(); // Refresh filters to update requisition data
      },
      error: (error: Error) => {
        console.error('Error loading requisitions:', error);
        // Don't show error for requisitions, just log it
      }
    });
  }

  applyFilters(): void {
    this.filteredCandidates = this.candidates.filter(candidate => {
      const term = this.searchTerm.trim().toLowerCase();
      const matchesSearch = !term || 
        `${candidate.firstName} ${candidate.lastName}`.toLowerCase().includes(term) ||
        candidate.email.toLowerCase().includes(term) ||
        candidate.status.toLowerCase().includes(term);

      const matchesStatus = this.selectedStatus ? candidate.status === this.selectedStatus : true;
      const matchesRequisition = this.selectedRequisition ? 
        (candidate as any).requisitionId?.toString() === this.selectedRequisition : true;

      return matchesSearch && matchesStatus && matchesRequisition;
    });

    this.totalItems = this.filteredCandidates.length;
    this.currentPage = 0;
    this.updatePagination();
  }

  filterCandidates(): void {
    this.applyFilters();
  }

  filterByStat(stat: string): void {
    this.selectedStat = stat;
    
    // Clear other filters when selecting a stat
    if (stat === 'all') {
      this.selectedStatus = '';
      this.selectedRequisition = '';
    } else {
      this.selectedStatus = stat;
    }
    
    this.applyFilters();
  }

  filterByStatus(status: string): void {
    this.selectedStatus = this.selectedStatus === status ? '' : status;
    this.applyFilters();
  }

  updatePagination(): void {
    const startIndex = this.currentPage * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.pagedCandidates = this.filteredCandidates.slice(startIndex, endIndex);
  }

  // Pagination methods
  getTotalPages(): number {
    return Math.ceil(this.totalItems / this.pageSize);
  }

  getPageNumbers(): number[] {
    const totalPages = this.getTotalPages();
    const current = this.currentPage;
    const pages: number[] = [];
    
    // Show up to 5 page numbers around current page
    const start = Math.max(0, current - 2);
    const end = Math.min(totalPages - 1, current + 2);
    
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  previousPage(): void {
    if (this.currentPage > 0) {
      this.currentPage--;
      this.updatePagination();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.getTotalPages() - 1) {
      this.currentPage++;
      this.updatePagination();
    }
  }

  goToPage(page: number): void {
    if (page >= 0 && page < this.getTotalPages()) {
      this.currentPage = page;
      this.updatePagination();
    }
  }

  // Helper methods for template
  getRequisitionTitle(candidate: Candidate): string {
    // Find the application for this candidate
    const application = this.applications.find(a => a.candidateId === candidate.candidateId);
    if (application) {
      // Find the requisition for this application
      const requisition = this.requisitions.find(r => r.requisitionId === application.requisitionId);
      return requisition ? requisition.title : 'N/A';
    }
    return 'N/A';
  }

  getAppliedDate(candidate: Candidate): string {
    // Find the application for this candidate
    const application = this.applications.find(a => a.candidateId === candidate.candidateId);
    if (application && application.createdAt) {
      return this.formatDate(application.createdAt);
    }
    return 'N/A';
  }

  exportToCSV(): void {
    const headers = ['Name', 'Email', 'Phone', 'Status', 'Applied Date'];
    const csvContent = [
      headers.join(','),
      ...this.filteredCandidates.map(candidate => {
        const application = this.applications.find(a => a.candidateId === candidate.candidateId);
        return [
          `"${candidate.firstName} ${candidate.lastName}"`,
          `"${candidate.email}"`,
          `"${candidate.phone || ''}"`,
          `"${candidate.status}"`,
          `"${this.formatDate(application?.createdAt || new Date().toISOString())}"`
        ].join(',');
      })
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `candidates-${new Date().toISOString().split('T')[0]}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);

    this.showSuccess('Candidates exported successfully!');
  }

  openDialog(candidate?: Candidate): void {
    const dialogRef = this.dialog.open(CandidateDialogComponent, {
      width: '500px',
      data: { 
        mode: candidate ? 'edit' : 'create',
        candidate: candidate
      }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        if (candidate) {
          // Update existing candidate
          this.candidateService.updateCandidate(candidate.candidateId, result).subscribe({
          next: () => {
              this.loadCandidates();
            this.showSuccess('Candidate updated successfully!');
            },
            error: (error: Error) => {
              console.error('Error updating candidate:', error);
              this.showError('Failed to update candidate. Please try again.');
            }
          });
        } else {
          // Create new candidate
          this.candidateService.createCandidate(result).subscribe({
            next: () => {
            this.loadCandidates();
              this.showSuccess('Candidate created successfully!');
            },
            error: (error: Error) => {
              console.error('Error creating candidate:', error);
              this.showError('Failed to create candidate. Please try again.');
            }
          });
        }
      }
    });
  }

  viewCandidateDetails(candidate: Candidate): void {
    this.dialog.open(CandidateDetailComponent, {
      width: '600px',
      maxHeight: '90vh',
      data: {
        candidate: candidate
      }
    });
  }

  deleteCandidate(id: number): void {
    this.confirmationDialog.confirmDanger(
      'Delete Candidate',
      'Are you sure you want to delete this candidate? This action cannot be undone.',
      'Delete',
      'Cancel'
    ).subscribe(confirmed => {
      if (confirmed) {
        this.candidateService.deleteCandidate(id).subscribe({
          next: () => {
            this.loadCandidates();
            this.showSuccess('Candidate deleted successfully!');
          },
          error: (error: Error) => {
            console.error('Error deleting candidate:', error);
            this.showError('Failed to delete candidate. Please try again.');
          }
        });
      }
    });
  }

  getInitials(firstName: string, lastName: string): string {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      month: 'numeric', 
      day: 'numeric', 
      year: 'numeric' 
    });
  }

  getApplicationCount(candidateId: number): number {
    // This would need to be implemented based on your application data
    // For now, returning a placeholder
    return 0; // Placeholder until backend provides actual count
  }

  getStatusBadgeClass(status: string): string {
    if (!status) return 'bg-gray-100 text-gray-800';
    
    const statusLower = status.toLowerCase();
    if (statusLower.includes('hired')) return 'bg-emerald-100 text-emerald-800';
    if (statusLower.includes('interviewing')) return 'bg-purple-100 text-purple-800';
    if (statusLower.includes('applied')) return 'bg-blue-100 text-blue-800';
    if (statusLower.includes('rejected')) return 'bg-red-100 text-red-800';
    if (statusLower.includes('withdrawn')) return 'bg-gray-100 text-gray-800';
    return 'bg-gray-100 text-gray-800';
  }

  getStatusDotClass(status: string): string {
    if (!status) return 'bg-gray-400';
    
    const statusLower = status.toLowerCase();
    if (statusLower.includes('hired')) return 'bg-emerald-500';
    if (statusLower.includes('interviewing')) return 'bg-purple-500';
    if (statusLower.includes('applied')) return 'bg-blue-500';
    if (statusLower.includes('rejected')) return 'bg-red-500';
    if (statusLower.includes('withdrawn')) return 'bg-gray-500';
    return 'bg-gray-400';
  }

  getStatusColor(status: string): string {
    if (!status) return 'basic';
    
    const statusLower = status.toLowerCase();
    if (statusLower.includes('hired')) return 'accent';
    if (statusLower.includes('interviewing')) return 'warn';
    if (statusLower.includes('applied')) return 'primary';
    if (statusLower.includes('rejected')) return 'warn';
    if (statusLower.includes('withdrawn')) return 'basic';
    return 'basic';
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

  // Getters for statistics
  get totalCandidates(): number {
    return this.candidates.length;
  }

  get appliedCandidates(): number {
    return this.candidates.filter(candidate => candidate.status === 'Applied').length;
  }

  get interviewingCandidates(): number {
    return this.candidates.filter(candidate => candidate.status === 'Interviewing').length;
  }

  get hiredCandidates(): number {
    return this.candidates.filter(candidate => candidate.status === 'Hired').length;
  }

  get firstCandidateId(): number {
    return this.candidates.length > 0 ? this.candidates[0].candidateId : 0;
  }

  get lastCandidateId(): number {
    return this.candidates.length > 0 ? this.candidates[this.candidates.length - 1].candidateId : 0;
  }

  // Role-based visibility methods
  canCreateCandidate(): boolean {
    const user = this.authService.getCurrentUser();
    if (!user) return false;
    const role = user.role?.toLowerCase();
    return ['admin', 'recruiter'].includes(role);
  }

  canEditCandidate(): boolean {
    const user = this.authService.getCurrentUser();
    if (!user) return false;
    const role = user.role?.toLowerCase();
    return ['admin', 'recruiter'].includes(role);
  }

  canDeleteCandidate(): boolean {
    const user = this.authService.getCurrentUser();
    if (!user) return false;
    const role = user.role?.toLowerCase();
    return role === 'admin';
  }

  canViewCandidateDetails(): boolean {
    const user = this.authService.getCurrentUser();
    if (!user) return false;
    const role = user.role?.toLowerCase();
    return ['admin', 'recruiter', 'hiring manager', 'interviewer', 'read-only'].includes(role);
  }

  retryLoad(): void {
    this.loadCandidates();
  }

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.filterCandidates();
  }

  navigateToRequisitions(): void {
    this.router.navigate(['/requisitions']);
  }
}
