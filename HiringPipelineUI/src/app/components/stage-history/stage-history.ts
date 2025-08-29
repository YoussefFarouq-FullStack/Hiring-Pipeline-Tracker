import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { StageHistory, HIRING_STAGES } from '../../models/stage-history.model';
import { StageHistoryService } from '../../services/stage-history.service';
import { StageHistoryDialogComponent } from './stage-history-dialog/stage-history-dialog';

@Component({
  selector: 'app-stage-history',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatTooltipModule
  ],
  templateUrl: './stage-history.html',
  styleUrls: ['./stage-history.scss']
})
export class StageHistoryComponent implements OnInit, OnChanges {
  @Input() applicationId?: number;
  @Input() currentStage?: string;
  @Input() candidateName?: string;
  
  histories: StageHistory[] = [];
  paginatedHistories: StageHistory[] = [];
  isLoading = false;
  hasError = false;
  errorMessage = '';

  // 🔹 Pagination
  currentPage = 1;
  pageSize = 5;
  totalPages = 1;

  // 🔹 Search
  searchTerm = '';

  constructor(
    private stageHistoryService: StageHistoryService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadData();

    // Debounced refresh when tab becomes visible
    let debounceTimer: any;
    const refresh = () => {
      clearTimeout(debounceTimer);
      debounceTimer = setTimeout(() => this.loadData(), 300);
    };
    window.addEventListener('focus', refresh);
    document.addEventListener('visibilitychange', () => {
      if (!document.hidden) refresh();
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['applicationId']) {
      this.loadData();
    }
  }

  loadData(): void {
    if (this.applicationId) {
      this.loadStageHistory();
    } else {
      this.loadAllStageHistory();
    }
  }

  private loadAllStageHistory() {
    this.isLoading = true;
    this.hasError = false;
    this.errorMessage = '';

    this.stageHistoryService.getStageHistory().subscribe({
      next: (data: StageHistory[]) => {
        this.histories = this.sortHistories(data);
        this.updatePagination();
        this.isLoading = false;
      },
      error: (err: Error) => this.handleError(err, 'Failed to load stage history')
    });
  }

  private loadStageHistory() {
    if (!this.applicationId) return;

    this.isLoading = true;
    this.hasError = false;
    this.errorMessage = '';

    this.stageHistoryService.getStageHistoryByApplication(this.applicationId).subscribe({
      next: (data: StageHistory[]) => {
        this.histories = this.sortHistories(data);
        this.updatePagination();
        this.isLoading = false;
      },
      error: (err: Error) => this.handleError(err, 'Failed to load stage history')
    });
  }

  private sortHistories(data: StageHistory[]): StageHistory[] {
    return data.sort((a, b) =>
      new Date(b.movedAt).getTime() - new Date(a.movedAt).getTime()
    );
  }

  // 🔹 Pagination helpers
  updatePagination(): void {
    let filtered = this.histories;

    // Apply search filter
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(h =>
        h.toStage.toLowerCase().includes(term) ||
        (h.fromStage && h.fromStage.toLowerCase().includes(term)) ||
        (this.candidateName && this.candidateName.toLowerCase().includes(term))
      );
    }

    this.totalPages = Math.ceil(filtered.length / this.pageSize);
    if (this.currentPage > this.totalPages) this.currentPage = this.totalPages || 1;

    const start = (this.currentPage - 1) * this.pageSize;
    this.paginatedHistories = filtered.slice(start, start + this.pageSize);
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.updatePagination();
    }
  }

  changePageSize(size: number): void {
    this.pageSize = size;
    this.currentPage = 1;
    this.updatePagination();
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm = target.value;
    this.currentPage = 1;
    this.updatePagination();
  }

  // 🔹 Error handling
  private handleError(err: Error, fallbackMessage: string) {
    console.error(fallbackMessage, err);
    this.hasError = true;
    this.errorMessage = err.message || fallbackMessage;
    this.isLoading = false;
    this.showError(this.errorMessage);
  }

  // 🔹 Stage icon and color helpers
  getStageIcon(stage: string): string {
    const stageLower = stage.toLowerCase();
    if (stageLower.includes('applied')) return 'person_add';
    if (stageLower.includes('interview')) return 'people';
    if (stageLower.includes('offer')) return 'card_giftcard';
    if (stageLower.includes('hired')) return 'check_circle';
    if (stageLower.includes('rejected')) return 'cancel';
    if (stageLower.includes('withdrawn')) return 'exit_to_app';
    return 'help';
  }

  getStageColor(stage: string): string {
    const stageLower = stage.toLowerCase();
    if (stageLower.includes('applied')) return 'blue';
    if (stageLower.includes('interview')) return 'purple';
    if (stageLower.includes('offer')) return 'orange';
    if (stageLower.includes('hired')) return 'green';
    if (stageLower.includes('rejected')) return 'red';
    if (stageLower.includes('withdrawn')) return 'gray';
    return 'blue';
  }

  getStageBadgeClass(stage: string): string {
    const stageLower = stage.toLowerCase();
    if (stageLower.includes('applied')) return 'bg-blue-100 text-blue-800';
    if (stageLower.includes('interview')) return 'bg-purple-100 text-purple-800';
    if (stageLower.includes('offer')) return 'bg-yellow-100 text-yellow-800';
    if (stageLower.includes('hired')) return 'bg-emerald-100 text-emerald-800';
    if (stageLower.includes('rejected')) return 'bg-red-100 text-red-800';
    if (stageLower.includes('withdrawn')) return 'bg-gray-100 text-gray-800';
    return 'bg-blue-100 text-blue-800';
  }

  getStageDotClass(stage: string): string {
    const stageLower = stage.toLowerCase();
    if (stageLower.includes('applied')) return 'bg-blue-500';
    if (stageLower.includes('interview')) return 'bg-purple-500';
    if (stageLower.includes('offer')) return 'bg-yellow-500';
    if (stageLower.includes('hired')) return 'bg-emerald-500';
    if (stageLower.includes('rejected')) return 'bg-red-500';
    if (stageLower.includes('withdrawn')) return 'bg-gray-500';
    return 'bg-blue-500';
  }

  // 🔹 Date and time formatting
  formatDate(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      month: 'short', 
      day: 'numeric', 
      year: 'numeric' 
    });
  }

  formatTime(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleTimeString('en-US', { 
      hour: '2-digit', 
      minute: '2-digit' 
    });
  }

  // 🔹 Dialog methods
  openAddDialog(): void {
    const dialogRef = this.dialog.open(StageHistoryDialogComponent, {
      width: '600px',
      maxWidth: '90vw',
      data: { 
        applicationId: this.applicationId,
        currentStage: this.currentStage 
      }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadData();
        this.showSuccess('Stage history added successfully!');
      }
    });
  }

  openEditDialog(history: StageHistory): void {
    const dialogRef = this.dialog.open(StageHistoryDialogComponent, {
      width: '600px',
      maxWidth: '90vw',
      data: { 
        mode: 'edit',
        history: history,
        applicationId: this.applicationId,
        currentStage: this.currentStage 
      }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadData();
        this.showSuccess('Stage history updated successfully!');
      }
    });
  }

  deleteHistory(history: StageHistory): void {
    if (confirm('Are you sure you want to delete this stage history entry?')) {
      this.stageHistoryService.deleteStageHistory(history.stageHistoryId).subscribe({
        next: () => {
          this.loadData();
          this.showSuccess('Stage history deleted successfully!');
        },
        error: (err: Error) => {
          this.showError('Failed to delete stage history. Please try again.');
        }
      });
    }
  }

  // 🔹 Utility methods
  getTotalItems(): number {
    return this.histories.length;
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

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updatePagination();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.updatePagination();
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.updatePagination();
    }
  }

  // 🔹 Notification methods
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

  // 🔹 Retry method
  retryLoad(): void {
    this.loadData();
  }
}
