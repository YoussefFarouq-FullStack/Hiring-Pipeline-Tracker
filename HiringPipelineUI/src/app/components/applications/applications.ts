import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Application } from '../../models/application.model';
import { Candidate } from '../../models/candidate.model';
import { Requisition } from '../../models/requisition.model';
import { ApplicationService, SearchResponse } from '../../services/application.service';
import { AuditLogService } from '../../services/audit-log.service';
import { ApplicationDialogComponent } from './application-dialog/application-dialog';
import { StageHistoryDialogComponent } from '../stage-history/stage-history-dialog/stage-history-dialog';
import { StageHistoryComponent } from '../stage-history/stage-history';
import { ConfirmationDialogService } from '../shared/confirmation-dialog/confirmation-dialog.service';
import { LoadingSpinnerComponent } from '../shared/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-applications',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatSnackBarModule,
    StageHistoryComponent,
    LoadingSpinnerComponent
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
  selectedDepartment: string = '';
  selectedStage: string = '';
  departments: string[] = [];

  isLoading = false;
  hasError = false;
  errorMessage = '';

  // Stage history viewing
  selectedApplicationId: number | undefined = undefined;
  selectedCandidateName: string | undefined = undefined;
  selectedCurrentStage: string | undefined = undefined;

  // pagination
  currentPage = 1;
  pageSize = 6;
  totalPages = 1;
  totalItemsCount = 0;

  private candidatesLoaded = false;
  private requisitionsLoaded = false;
  
  // Search state
  useServerSideSearch = true;
  searchTimeout: any;

  constructor(
    private appService: ApplicationService,
    private auditLogService: AuditLogService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private router: Router,
    private confirmationDialog: ConfirmationDialogService
  ) {}

  ngOnInit(): void {
    // Note: Audit logging is now handled by the middleware automatically
    // The middleware will log "View Applications" when the applications page is accessed
    // and log background data fetches as "BackgroundFetch" type
    
    this.loadApplications();
  }

  loadApplications(): void {
    if (this.useServerSideSearch) {
      this.performSearch();
    } else {
      this.loadAllApplications();
    }
  }

  private loadAllApplications(): void {
    this.isLoading = true;
    this.hasError = false;
    this.errorMessage = '';

    this.appService.getApplications().subscribe({
      next: (data: Application[]) => {
        console.log('Raw applications data from API:', data);
        if (data.length > 0) {
          console.log('First application object keys:', Object.keys(data[0]));
          console.log('First application applicationId:', data[0].applicationId);
          console.log('First application ApplicationId (PascalCase):', (data[0] as any).ApplicationId);
        }
        
        // Temporary fix: Convert PascalCase to camelCase if needed
        const normalizedData = data.map(app => {
          const anyApp = app as any;
          if (!anyApp.applicationId && anyApp.ApplicationId) {
            return {
              ...anyApp,
              applicationId: anyApp.ApplicationId,
              candidateId: anyApp.CandidateId,
              requisitionId: anyApp.RequisitionId,
              currentStage: anyApp.CurrentStage,
              status: anyApp.Status,
              createdAt: anyApp.CreatedAt,
              updatedAt: anyApp.UpdatedAt
            } as Application;
          }
          return app;
        });
        
        this.applications = normalizedData;
        this.filteredApplications = normalizedData;
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

  private performSearch(): void {
    this.isLoading = true;
    this.hasError = false;
    this.errorMessage = '';

    const searchParams = {
      searchTerm: this.searchTerm || undefined,
      status: this.selectedStat !== 'all' ? this.selectedStat : undefined,
      stage: this.selectedStage || undefined,
      department: this.selectedDepartment || undefined,
      skip: (this.currentPage - 1) * this.pageSize,
      take: this.pageSize
    };

    this.appService.searchApplications(searchParams).subscribe({
      next: (response: SearchResponse<Application>) => {
        console.log('Search response:', response);
        
        // Normalize data if needed
        const normalizedData = response.items.map(app => {
          const anyApp = app as any;
          if (!anyApp.applicationId && anyApp.ApplicationId) {
            return {
              ...anyApp,
              applicationId: anyApp.ApplicationId,
              candidateId: anyApp.CandidateId,
              requisitionId: anyApp.RequisitionId,
              currentStage: anyApp.CurrentStage,
              status: anyApp.Status,
              createdAt: anyApp.CreatedAt,
              updatedAt: anyApp.UpdatedAt
            } as Application;
          }
          return app;
        });
        
        this.applications = normalizedData;
        this.filteredApplications = normalizedData;
        this.totalItemsCount = response.totalCount;
        this.totalPages = Math.ceil(response.totalCount / this.pageSize);
        this.isLoading = false;
        this.loadRelatedData();
      },
      error: (error: Error) => {
        console.error('Error searching applications:', error);
        this.hasError = true;
        this.errorMessage = error.message || 'Failed to search applications';
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

    // Extract unique departments for filtering
    this.departments = [...new Set(this.requisitions.map(r => r.department).filter(d => d))];

    this.filteredApplications = [...this.applications];
    this.updatePagination();
  }

  filterApplications(): void {
    if (this.useServerSideSearch) {
      this.currentPage = 1; // Reset to first page when filtering
      // If search term is empty or very short, search immediately
      if (!this.searchTerm || this.searchTerm.length <= 1) {
        this.performSearch();
      } else {
        this.debouncedSearch();
      }
    } else {
      this.applyClientSideFilters();
    }
  }

  private debouncedSearch(): void {
    if (this.searchTimeout) {
      clearTimeout(this.searchTimeout);
    }
    this.searchTimeout = setTimeout(() => {
      this.performSearch();
    }, 150); // Reduced to 150ms for faster response
  }

  private applyClientSideFilters(): void {
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

    // Filter by department
    if (this.selectedDepartment) {
      filtered = filtered.filter(app => {
        const requisition = this.requisitions.find(r => r.requisitionId === app.requisitionId);
        return requisition?.department === this.selectedDepartment;
      });
    }

    // Filter by stage
    if (this.selectedStage) {
      filtered = filtered.filter(app => app.currentStage === this.selectedStage);
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
    this.confirmationDialog.confirmDanger(
      'Delete Application',
      'Are you sure you want to delete this application? This action cannot be undone.',
      'Delete',
      'Cancel'
    ).subscribe(confirmed => {
      if (confirmed) {
        this.appService.deleteApplication(id).subscribe({
          next: () => {
            // Reload applications to get the fresh data from the server
            this.loadApplications();
            this.showSuccess('Application deleted successfully!');
          },
          error: () => this.showError('Failed to delete application. Please try again.')
        });
      }
    });
  }

  viewStageHistory(applicationId: number, currentStage: string): void {
    // Find the application to get candidate name
    const application = this.applications.find(app => app.applicationId === applicationId);
    if (!application) return;

    // Find the candidate name
    const candidate = this.candidates.find(c => c.candidateId === application.candidateId);
    const candidateName = candidate ? `${candidate.firstName} ${candidate.lastName}` : `Candidate #${application.candidateId}`;

    // Set the selected application data for stage history viewing
    this.selectedApplicationId = applicationId;
    this.selectedCandidateName = candidateName;
    this.selectedCurrentStage = currentStage;
  }

  moveToStage(applicationId: number, currentStage: string): void {
    console.log('=== moveToStage DEBUG START ===');
    console.log('moveToStage called with applicationId:', applicationId, 'type:', typeof applicationId);
    console.log('moveToStage called with currentStage:', currentStage);
    
    // Early validation
    if (!applicationId || applicationId <= 0) {
      console.error('Invalid applicationId passed to moveToStage:', applicationId);
      this.showError('Invalid application ID. Cannot move to next stage.');
      return;
    }
    console.log('All applications count:', this.applications.length);
    
    // Debug: Check all application objects and their IDs
    this.applications.forEach((app, index) => {
      console.log(`Application ${index}:`, {
        applicationId: app.applicationId,
        ApplicationId: (app as any).ApplicationId,
        candidateId: app.candidateId,
        currentStage: app.currentStage,
        allKeys: Object.keys(app)
      });
    });
    
    // Find the application to get candidate name
    let application = this.applications.find(app => app.applicationId === applicationId);
    console.log('Found application by applicationId:', application);
    
    // If not found by camelCase, try PascalCase
    if (!application) {
      application = this.applications.find(app => (app as any).ApplicationId === applicationId);
      console.log('Found application by ApplicationId (PascalCase):', application);
    }
    
    console.log('=== moveToStage DEBUG END ===');
    
    if (!application) {
      console.error('Application not found for ID:', applicationId);
      this.showError('Application not found. Cannot move to next stage.');
      return;
    }

    // Find the candidate name
    const candidate = this.candidates.find(c => c.candidateId === application.candidateId);
    const candidateName = candidate ? `${candidate.firstName} ${candidate.lastName}` : `Candidate #${application.candidateId}`;

    console.log('Opening dialog with data:', {
      applicationId: applicationId,
      currentStage: currentStage,
      candidateName: candidateName
    });

    // Open the stage transition dialog
    const dialogRef = this.dialog.open(StageHistoryDialogComponent, {
      width: '500px',
      data: { 
        applicationId: applicationId,
        currentStage: currentStage
      }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        // Reload applications to get the updated stage
        this.loadApplications();
        this.showSuccess(`Candidate moved to ${result.toStage} successfully!`);
        
        // Log the stage transition
        this.auditLogService.logUserAction(
          `Move Candidate to ${result.toStage}`,
          'StageHistory',
          applicationId,
          `Moved ${candidateName} from ${result.fromStage} to ${result.toStage}`
        );
      }
    });
  }

  closeStageHistory(): void {
    this.selectedApplicationId = undefined;
    this.selectedCandidateName = undefined;
    this.selectedCurrentStage = undefined;
  }

  // Pagination helpers
  updatePagination(): void {
    if (this.useServerSideSearch) {
      // For server-side search, pagination is handled by the search response
      this.totalPages = Math.max(1, Math.ceil(this.totalItemsCount / this.pageSize));
      if (this.currentPage > this.totalPages) this.currentPage = this.totalPages;
    } else {
      // For client-side pagination
      this.totalPages = Math.max(1, Math.ceil(this.filteredApplications.length / this.pageSize));
      if (this.currentPage > this.totalPages) this.currentPage = this.totalPages;
    }
  }

  get paginatedApplications(): Application[] {
    if (this.useServerSideSearch) {
      // For server-side search, data is already paginated
      return this.applications;
    } else {
      // For client-side pagination
      const start = (this.currentPage - 1) * this.pageSize;
      return this.filteredApplications.slice(start, start + this.pageSize);
    }
  }

  get totalItems(): number {
    if (this.useServerSideSearch) {
      return this.totalItemsCount || this.applications.length;
    } else {
      return this.filteredApplications.length;
    }
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
      if (this.useServerSideSearch) {
        this.performSearch();
      }
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      if (this.useServerSideSearch) {
        this.performSearch();
      }
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      if (this.useServerSideSearch) {
        this.performSearch();
      }
    }
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { month: 'numeric', day: 'numeric', year: 'numeric' });
  }

  getStageBadgeClass(stage: string | null | undefined): string {
    if (!stage || typeof stage !== 'string') return 'bg-gray-100 text-gray-800';
    const s = stage.toLowerCase();
    if (s.includes('interview')) return 'bg-purple-100 text-purple-800';
    if (s.includes('offer')) return 'bg-yellow-100 text-yellow-800';
    if (s.includes('hired')) return 'bg-emerald-100 text-emerald-800';
    if (s.includes('rejected')) return 'bg-red-100 text-red-800';
    if (s.includes('withdrawn')) return 'bg-gray-100 text-gray-800';
    return 'bg-blue-100 text-blue-800';
  }

  getStageDotClass(stage: string | null | undefined): string {
    if (!stage || typeof stage !== 'string') return 'bg-gray-400';
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
    this.dialog.open(ApplicationDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      data: {
        mode: 'view', // Use view mode to show all details in read-only
        application: application
      },
      disableClose: false
    });
  }

  getEndItemNumber(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalItems);
  }

  exportApplications(): void {
    // Create CSV content
    const headers = ['Application ID', 'Candidate Name', 'Email', 'Position', 'Department', 'Applied Date', 'Current Stage'];
    const csvContent = [
      headers.join(','),
      ...this.filteredApplications.map(app => [
        app.applicationId,
        `"${this.getCandidateName(app.candidateId)}"`,
        `"${this.getCandidateEmail(app.candidateId)}"`,
        `"${this.getRequisitionTitle(app.requisitionId)}"`,
        `"${this.getRequisitionDepartment(app.requisitionId)}"`,
        this.formatDate(app.createdAt),
        `"${app.currentStage || 'N/A'}"`
      ].join(','))
    ].join('\n');

    // Create and download file
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', `applications_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    this.showSuccess('Applications exported successfully!');
  }

  // 🔹 Empty state methods
  clearSearch(): void {
    this.searchTerm = '';
    this.selectedStat = 'all';
    this.selectedStage = '';
    this.selectedDepartment = '';
    this.currentPage = 1;
    this.filterApplications();
  }

  navigateToRequisitions(): void {
    this.router.navigate(['/requisitions']);
  }

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }
}
