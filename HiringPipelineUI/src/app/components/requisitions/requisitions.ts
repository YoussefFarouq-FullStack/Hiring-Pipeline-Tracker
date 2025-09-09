import { Component, OnInit, ChangeDetectorRef, AfterViewInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSelectModule } from '@angular/material/select';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { take } from 'rxjs/operators';

import { Requisition, EMPLOYMENT_TYPES, PRIORITIES, EXPERIENCE_LEVELS, JOB_LEVELS, STATUSES } from '../../models/requisition.model';
import { RequisitionService } from '../../services/requisition.service';
import { RequisitionDialogComponent } from './requisition-dialog/requisition-dialog';
import { RequisitionDetailComponent } from './requisition-detail/requisition-detail';

@Component({
  selector: 'app-requisitions',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatSelectModule,
    MatPaginatorModule
  ],
  templateUrl: './requisitions.html',
  styleUrls: ['./requisitions.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RequisitionsComponent implements OnInit, AfterViewInit {
  requisitions: Requisition[] = [];
  filteredRequisitions: Requisition[] = [];
  paginatedRequisitions: Requisition[] = [];

  // Filters
  searchTerm: string = '';
  selectedDepartment: string = '';
  selectedStatus: string = '';
  selectedPriority: string = '';
  selectedEmploymentType: string = '';
  selectedExperienceLevel: string = '';
  selectedStat: string = 'all';
  
  // Filter options
  departments: string[] = [];
  employmentTypes = EMPLOYMENT_TYPES;
  priorities = PRIORITIES;
  experienceLevels = EXPERIENCE_LEVELS;
  jobLevels = JOB_LEVELS;
  statuses = STATUSES;

  // UI states
  isLoading = false;
  hasError = false;
  errorMessage = '';

  // Pagination
  pageSize = 6;
  currentPage = 0;
  totalItems = 0;

  constructor(
    private requisitionService: RequisitionService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadRequisitions();
  }

  ngAfterViewInit(): void {
    // Trigger change detection after view is initialized
    this.cdr.detectChanges();
  }

  loadRequisitions(): void {
    this.isLoading = true;
    this.hasError = false;
    this.errorMessage = '';

    this.requisitionService.getRequisitions()
      .pipe(take(1))
      .subscribe({
        next: (data: Requisition[]) => {
          this.requisitions = data;
          this.extractDepartments();
          this.applyFilters();
          this.isLoading = false;
          // Trigger change detection after data is loaded
          this.cdr.detectChanges();
        },
        error: (error: any) => {
          console.error('Error loading requisitions:', error);
          this.hasError = true;
          this.errorMessage = error?.message || 'Unexpected error occurred while loading requisitions';
          this.isLoading = false;
          this.showError(this.errorMessage);
          // Trigger change detection after error
          this.cdr.detectChanges();
        }
      });
  }

  extractDepartments(): void {
    const deptSet = new Set<string>();
    this.requisitions.forEach(req => {
      if (req.department) {
        deptSet.add(req.department);
      }
    });
    this.departments = Array.from(deptSet).sort();
  }

  applyFilters(): void {
    this.filteredRequisitions = this.requisitions.filter(req => {
      const term = this.searchTerm.trim().toLowerCase();

      const matchesSearch =
        !term ||
        req.title.toLowerCase().includes(term) ||
        (req.department?.toLowerCase().includes(term)) ||
        (req.status?.toLowerCase().includes(term)) ||
        (req.description?.toLowerCase().includes(term)) ||
        (req.location?.toLowerCase().includes(term)) ||
        (req.requiredSkills?.toLowerCase().includes(term));

      const matchesStatus = this.selectedStatus ? req.status === this.selectedStatus : true;
      const matchesDepartment = this.selectedDepartment ? req.department === this.selectedDepartment : true;
      const matchesPriority = this.selectedPriority ? req.priority === this.selectedPriority : true;
      const matchesEmploymentType = this.selectedEmploymentType ? req.employmentType === this.selectedEmploymentType : true;
      const matchesExperienceLevel = this.selectedExperienceLevel ? req.experienceLevel === this.selectedExperienceLevel : true;

      return matchesSearch && matchesStatus && matchesDepartment && matchesPriority && matchesEmploymentType && matchesExperienceLevel;
    });

    this.totalItems = this.filteredRequisitions.length;
    this.currentPage = 0; // reset page when filters change
    this.updatePagination();
    
    // Trigger change detection after filters are applied
    this.cdr.detectChanges();
  }

  filterRequisitions(): void {
    this.applyFilters();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedDepartment = '';
    this.selectedStatus = '';
    this.selectedPriority = '';
    this.selectedEmploymentType = '';
    this.selectedExperienceLevel = '';
    this.selectedStat = 'all';
    this.applyFilters();
  }

  filterByStat(stat: string): void {
    this.selectedStat = stat;
    
    // Clear other filters when selecting a stat
    if (stat === 'all') {
      this.selectedStatus = '';
      this.selectedDepartment = '';
      this.selectedPriority = '';
      this.selectedEmploymentType = '';
      this.selectedExperienceLevel = '';
    } else {
      this.selectedStatus = stat;
    }
    
    this.applyFilters();
  }

  updatePagination(): void {
    const startIndex = this.currentPage * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedRequisitions = this.filteredRequisitions.slice(startIndex, endIndex);
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.updatePagination();
  }

  openDialog(requisition?: Requisition): void {
    const dialogRef = this.dialog.open(RequisitionDialogComponent, {
      width: '500px',
      maxHeight: '85vh',
      data: {
        mode: requisition ? 'edit' : 'create',
        requisition: requisition
      }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        if (requisition) {
          // Update existing requisition
          this.requisitionService.updateRequisition(requisition.requisitionId, result)
            .pipe(take(1))
            .subscribe({
              next: () => {
                this.loadRequisitions();
                this.showSuccess('Requisition updated successfully!');
              },
              error: (error: any) => {
                console.error('Error updating requisition:', error);
                this.showError('Failed to update requisition. Please try again.');
              }
            });
        } else {
          // Create new requisition
          this.requisitionService.createRequisition(result)
            .pipe(take(1))
            .subscribe({
              next: () => {
                this.loadRequisitions();
                this.showSuccess('Requisition created successfully!');
              },
              error: (error: any) => {
                console.error('Error creating requisition:', error);
                this.showError('Failed to create requisition. Please try again.');
              }
            });
        }
      }
    });
  }

  viewRequisitionDetails(requisition: Requisition): void {
    this.dialog.open(RequisitionDetailComponent, {
      width: '700px',
      maxHeight: '90vh',
      data: {
        requisition: requisition
      }
    });
  }

  deleteRequisition(id: number): void {
    if (confirm('Are you sure you want to delete this requisition?')) {
      this.requisitionService.deleteRequisition(id)
        .pipe(take(1))
        .subscribe({
          next: () => {
            this.loadRequisitions();
            this.showSuccess('Requisition deleted successfully!');
          },
          error: (error: any) => {
            console.error('Error deleting requisition:', error);
            this.showError('Failed to delete requisition. Please try again.');
          }
        });
    }
  }

  getStatusBadgeClass(status: string): string {
    return this.requisitionService.getStatusColor(status);
  }

  getPriorityBadgeClass(priority: string): string {
    return this.requisitionService.getPriorityColor(priority);
  }

  getDraftBadgeClass(isDraft: boolean): string {
    return this.requisitionService.getDraftStatusColor(isDraft);
  }

  getStatusDotClass(status: string): string {
    if (!status) return 'bg-gray-400';
    const statusLower = status.toLowerCase();
    if (statusLower.includes('open')) return 'bg-green-500';
    if (statusLower.includes('in progress')) return 'bg-purple-500';
    if (statusLower.includes('closed')) return 'bg-red-500';
    if (statusLower.includes('on hold')) return 'bg-yellow-500';
    return 'bg-gray-400';
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { month: 'numeric', day: 'numeric', year: 'numeric' });
  }

  // TODO: Replace with actual backend application count
  getApplicationCount(requisitionId: number): number {
    return 0; // Placeholder until backend provides actual count
  }

  retryLoad(): void {
    this.loadRequisitions();
  }

  exportRequisitions(): void {
    const headers = ['Title', 'Description', 'Department', 'Location', 'Employment Type', 'Salary', 'Priority', 'Experience Level', 'Job Level', 'Status', 'Draft', 'Required Skills', 'Created At'];
    const csvContent = [
      headers.join(','),
      ...this.filteredRequisitions.map(req =>
        [
          `"${req.title.replace(/"/g, '""')}"`,
          `"${(req.description || '').replace(/"/g, '""')}"`,
          `"${(req.department || '').replace(/"/g, '""')}"`,
          `"${(req.location || '').replace(/"/g, '""')}"`,
          `"${(req.employmentType || '').replace(/"/g, '""')}"`,
          `"${(req.salary || '').replace(/"/g, '""')}"`,
          `"${(req.priority || '').replace(/"/g, '""')}"`,
          `"${(req.experienceLevel || '').replace(/"/g, '""')}"`,
          `"${(req.jobLevel || '').replace(/"/g, '""')}"`,
          `"${(req.status || '').replace(/"/g, '""')}"`,
          req.isDraft ? 'Yes' : 'No',
          `"${(req.requiredSkills || '').replace(/"/g, '""')}"`,
          this.formatDate(req.createdAt)
        ].join(',')
      )
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `requisitions-${new Date().toISOString().split('T')[0]}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);

    this.showSuccess('Requisitions exported successfully!');
  }

  // --- Stats getters ---
  get totalRequisitions(): number {
    return this.requisitions.length;
  }
  get openRequisitions(): number {
    return this.requisitions.filter(r => r.status?.toLowerCase().includes('open')).length;
  }
  get inProgressRequisitions(): number {
    return this.requisitions.filter(r => r.status?.toLowerCase().includes('in progress')).length;
  }
  get closedRequisitions(): number {
    return this.requisitions.filter(r => r.status?.toLowerCase().includes('closed')).length;
  }
  get draftRequisitions(): number {
    return this.requisitions.filter(r => r.isDraft).length;
  }
  get highPriorityRequisitions(): number {
    return this.requisitions.filter(r => r.priority?.toLowerCase() === 'high').length;
  }

  // --- Snackbar helpers ---
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

  // Make Math available in template
  readonly Math = Math;
}
