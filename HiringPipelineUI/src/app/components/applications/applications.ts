import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Application } from '../../models/application.model';
import { Candidate } from '../../models/candidate.model';
import { Requisition } from '../../models/requisition.model';
import { ApplicationService } from '../../services/application.service';
import { ApplicationDialogComponent } from './application-dialog/application-dialog';
import { StageHistoryDialogComponent } from '../stage-history/stage-history-dialog/stage-history-dialog';

@Component({
  selector: 'app-applications',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatSnackBarModule
  ],
  templateUrl: './applications.html',
  styleUrls: ['./applications.scss']
})
export class ApplicationsComponent implements OnInit {
  applications: Application[] = [];
  filteredApplications: Application[] = [];
  candidates: Candidate[] = [];
  requisitions: Requisition[] = [];
  searchTerm: string = '';
  selectedStat: string = 'all'; // New property for stats filtering

  isLoading = false;
  hasError = false;
  errorMessage = '';

  // pagination
  currentPage = 1;
  pageSize = 6;
  totalPages = 1;

  private candidatesLoaded = false;
  private requisitionsLoaded = false;

  constructor(
    private appService: ApplicationService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications(): void {
    this.isLoading = true;
    this.hasError = false;
    this.errorMessage = '';

    this.appService.getApplications().subscribe({
      next: (data: Application[]) => {
        this.applications = data;
        this.filteredApplications = data;
        this.isLoading = false;
        this.loadRelatedData();
      },
      error: () => {
        this.hasError = true;
        this.errorMessage = 'Unable to load applications. Please try again later.';
        this.isLoading = false;
        this.showError(this.errorMessage);
      }
    });
  }

  private loadRelatedData(): void {
    this.candidatesLoaded = false;
    this.requisitionsLoaded = false;

    this.appService.getCandidates().subscribe({
      next: (candidates: Candidate[]) => {
        this.candidates = candidates;
        this.candidatesLoaded = true;
        this.tryMergeData();
      },
      error: () => this.showError('Failed to load candidate data')
    });

    this.appService.getRequisitions().subscribe({
      next: (requisitions: Requisition[]) => {
        this.requisitions = requisitions;
        this.requisitionsLoaded = true;
        this.tryMergeData();
      },
      error: () => this.showError('Failed to load requisition data')
    });
  }

  private tryMergeData(): void {
    if (this.candidatesLoaded && this.requisitionsLoaded) {
      this.mergeRelatedData();
    }
  }

  private mergeRelatedData(): void {
    const candidateMap = new Map(this.candidates.map(c => [c.candidateId, c]));
    const requisitionMap = new Map(this.requisitions.map(r => [r.requisitionId, r]));

    this.applications = this.applications.map(app => {
      const candidate = candidateMap.get(app.candidateId);
      const requisition = requisitionMap.get(app.requisitionId);

      return {
        ...app,
        candidate,
        requisition
      };
    });

    this.filteredApplications = [...this.applications];
    this.updatePagination();
  }

  filterApplications(): void {
    let filtered = [...this.applications];

    // Filter by selected stat
    if (this.selectedStat && this.selectedStat !== 'all') {
      switch (this.selectedStat) {
        case 'Active':
          filtered = filtered.filter(app =>
            app.currentStage && !['Hired', 'Rejected', 'Withdrawn'].includes(app.currentStage)
          );
          break;
        case 'In Progress':
          filtered = filtered.filter(app =>
            app.currentStage && ['Interview', 'Technical Interview', 'Onsite Interview'].includes(app.currentStage)
          );
          break;
        case 'Completed':
          filtered = filtered.filter(app =>
            app.currentStage && ['Hired', 'Rejected', 'Withdrawn'].includes(app.currentStage)
          );
          break;
        default:
          // If no matching stat, show all
          break;
      }
    }

    // Filter by search term
    if (this.searchTerm.trim()) {
      const searchLower = this.searchTerm.toLowerCase();
      filtered = filtered.filter(app =>
        this.getCandidateName(app.candidateId).toLowerCase().includes(searchLower) ||
        this.getRequisitionTitle(app.requisitionId).toLowerCase().includes(searchLower) ||
        (app.currentStage?.toLowerCase().includes(searchLower) || false)
      );
    }

    this.filteredApplications = filtered;
    this.currentPage = 1;
    this.updatePagination();
  }

  filterByStat(stat: string): void {
    this.selectedStat = stat;
    
    // Clear search term when selecting a stat
    if (stat === 'all') {
      this.searchTerm = '';
    }
    
    this.filterApplications();
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(ApplicationDialogComponent, {
      width: '500px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe((result: Application) => {
      if (result) {
        // Reload applications to get the fresh data from the server
        this.loadApplications();
        this.showSuccess('Application created successfully!');
      }
    });
  }

  deleteApplication(id: number): void {
    if (confirm('Are you sure you want to delete this application?')) {
      this.appService.deleteApplication(id).subscribe({
        next: () => {
          // Reload applications to get the fresh data from the server
          this.loadApplications();
          this.showSuccess('Application deleted successfully!');
        },
        error: () => this.showError('Failed to delete application. Please try again.')
      });
    }
  }

  viewStageHistory(applicationId: number, currentStage: string): void {
    const dialogRef = this.dialog.open(StageHistoryDialogComponent, {
      width: '900px',
      maxWidth: '95vw',
      data: { applicationId, currentStage }
    });

    dialogRef.afterClosed().subscribe(updatedStage => {
      if (updatedStage) {
        const app = this.applications.find(a => a.applicationId === applicationId);
        if (app) {
          app.currentStage = updatedStage;
          this.mergeRelatedData();
        }
        this.showSuccess('Stage updated successfully!');
      }
    });
  }

  // Pagination helpers
  updatePagination(): void {
    this.totalPages = Math.max(1, Math.ceil(this.filteredApplications.length / this.pageSize));
    if (this.currentPage > this.totalPages) this.currentPage = this.totalPages;
  }

  get paginatedApplications(): Application[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredApplications.slice(start, start + this.pageSize);
  }

  get totalItems(): number {
    return this.filteredApplications.length;
  }

  getTotalPages(): number {
    return this.totalPages;
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxVisible = 5;
    let start = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
    let end = Math.min(this.totalPages, start + maxVisible - 1);
    
    if (end - start + 1 < maxVisible) {
      start = Math.max(1, end - maxVisible + 1);
    }
    
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { month: 'numeric', day: 'numeric', year: 'numeric' });
  }

  getStageBadgeClass(stage: string): string {
    if (!stage) return 'bg-gray-100 text-gray-800';
    const s = stage.toLowerCase();
    if (s.includes('interview')) return 'bg-purple-100 text-purple-800';
    if (s.includes('offer')) return 'bg-yellow-100 text-yellow-800';
    if (s.includes('hired')) return 'bg-emerald-100 text-emerald-800';
    if (s.includes('rejected')) return 'bg-red-100 text-red-800';
    if (s.includes('withdrawn')) return 'bg-gray-100 text-gray-800';
    return 'bg-blue-100 text-blue-800';
  }

  getStageDotClass(stage: string): string {
    if (!stage) return 'bg-gray-400';
    const s = stage.toLowerCase();
    if (s.includes('interview')) return 'bg-purple-500';
    if (s.includes('offer')) return 'bg-yellow-500';
    if (s.includes('hired')) return 'bg-emerald-500';
    if (s.includes('rejected')) return 'bg-red-500';
    if (s.includes('withdrawn')) return 'bg-gray-500';
    return 'bg-blue-500';
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

  // Stats getters
  get totalApplications(): number {
    return this.applications.length;
  }

  get activeApplications(): number {
    return this.applications.filter(app =>
      app.currentStage && !['Hired', 'Rejected', 'Withdrawn'].includes(app.currentStage)
    ).length;
  }

  get inProgressApplications(): number {
    return this.applications.filter(app =>
      app.currentStage && ['Interview', 'Technical Interview', 'Onsite Interview'].includes(app.currentStage)
    ).length;
  }

  get completedApplications(): number {
    return this.applications.filter(app =>
      app.currentStage && ['Hired', 'Rejected', 'Withdrawn'].includes(app.currentStage)
    ).length;
  }

  // Helper methods for template
  getCandidateName(candidateId: number): string {
    const candidate = this.candidates.find(c => c.candidateId === candidateId);
    return candidate ? `${candidate.firstName} ${candidate.lastName}` : `Candidate #${candidateId}`;
  }

  getCandidateEmail(candidateId: number): string {
    const candidate = this.candidates.find(c => c.candidateId === candidateId);
    return candidate?.email || '';
  }

  getCandidateInitials(candidateId: number): string {
    const candidate = this.candidates.find(c => c.candidateId === candidateId);
    if (candidate) {
      return `${candidate.firstName[0]}${candidate.lastName[0]}`.toUpperCase();
    }
    return 'NA';
  }

  getRequisitionTitle(requisitionId: number): string {
    const requisition = this.requisitions.find(r => r.requisitionId === requisitionId);
    return requisition?.title || `Requisition #${requisitionId}`;
  }

  getRequisitionDepartment(requisitionId: number): string {
    const requisition = this.requisitions.find(r => r.requisitionId === requisitionId);
    return requisition?.department || '';
  }

  viewApplication(application: Application): void {
    // For now, just show a success message
    // You can implement a detailed view dialog later
    this.showSuccess(`Viewing application for ${this.getCandidateName(application.candidateId)}`);
  }

  getEndItemNumber(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalItems);
  }
}
