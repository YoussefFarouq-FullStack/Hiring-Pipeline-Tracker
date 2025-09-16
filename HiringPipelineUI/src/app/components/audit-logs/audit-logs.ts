import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { Subscription } from 'rxjs';
import { AuditLogService, AuditLog, AuditLogFilter } from '../../services/audit-log.service';
import { AuthService } from '../../services/auth.service';
import { DataRefreshService } from '../../services/data-refresh.service';

@Component({
  selector: 'app-audit-logs',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSnackBarModule,
    MatPaginatorModule
  ],
  templateUrl: './audit-logs.html',
  styleUrls: ['./audit-logs.scss']
})
export class AuditLogsComponent implements OnInit, OnDestroy {
  auditLogs: AuditLog[] = [];
  isLoading = false;
  totalCount = 0;
  currentPage = 0;
  pageSize = 15;
  selectedLogType: string = 'UserAction'; // Default to show only user actions

  selectedLog: AuditLog | null = null;
  private refreshSubscription?: Subscription;

  // Log type options for filtering
  logTypeOptions = [
    { value: '', label: 'All Log Types' },
    { value: 'UserAction', label: 'User Actions' },
    { value: 'BackgroundFetch', label: 'Background Fetches' },
    { value: 'Authentication', label: 'Authentication' },
    { value: 'SystemOperation', label: 'System Operations' },
    { value: 'DatabaseManagement', label: 'Database Management' }
  ];

  constructor(
    private auditLogService: AuditLogService,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private dataRefreshService: DataRefreshService
  ) {}

  ngOnInit(): void {
    this.loadAuditLogs();
    
    // Subscribe to data refresh events
    this.refreshSubscription = this.dataRefreshService.refresh$.subscribe(event => {
      if (event.type === 'audit-logs-cleared' || 
          (event.type === 'table-cleared' && event.tableNames?.includes('AuditLogs')) ||
          (event.type === 'hiring-data-cleared')) {
        this.handleDataRefresh();
      }
    });
  }

  ngOnDestroy(): void {
    if (this.refreshSubscription) {
      this.refreshSubscription.unsubscribe();
    }
  }

  private handleDataRefresh(): void {
    console.log('Audit logs data was cleared, refreshing...');
    // Clear local data immediately
    this.auditLogs = [];
    this.totalCount = 0;
    this.currentPage = 0;
    
    // Show a brief notification
    this.snackBar.open('Audit logs refreshed', 'Close', { 
      duration: 2000,
      panelClass: ['info-snackbar']
    });
    
    // Reload the data
    this.loadAuditLogs();
  }

  loadAuditLogs(): void {
    this.isLoading = true;
    const skip = this.currentPage * this.pageSize;
    const take = this.pageSize;

    console.log('Loading audit logs with pagination:', { skip, take, logType: this.selectedLogType });
    console.log('Current timestamp:', new Date().toISOString());

    const filter: AuditLogFilter = { skip, take };
    if (this.selectedLogType) {
      filter.logType = this.selectedLogType;
    }

    this.auditLogService.getAuditLogs(filter).subscribe({
      next: (response) => {
        console.log('Raw API response:', response);
        this.auditLogs = response.auditLogs;
        this.totalCount = response.totalCount;
        this.isLoading = false;
        console.log('Audit logs loaded:', response.auditLogs.length, 'total:', response.totalCount);
        console.log('First log timestamp:', response.auditLogs[0]?.timestamp);
      },
      error: (error) => {
        console.error('Error loading audit logs:', error);
        this.snackBar.open('Failed to load audit logs', 'Close', { duration: 5000 });
        this.isLoading = false;
      }
    });
  }

  refreshLogs(): void {
    this.currentPage = 0;
    this.loadAuditLogs();
  }


  onPageChange(event: PageEvent): void {
    this.currentPage = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadAuditLogs();
  }

  onPageSizeChange(): void {
    this.currentPage = 0;
    this.loadAuditLogs();
  }

  onLogTypeChange(): void {
    this.currentPage = 0;
    this.loadAuditLogs();
  }

  getLogTypeBadgeClass(logType: string): string {
    const logTypeClasses: { [key: string]: string } = {
      'UserAction': 'bg-blue-100 text-blue-800 border-blue-200',
      'BackgroundFetch': 'bg-gray-100 text-gray-600 border-gray-200',
      'Authentication': 'bg-yellow-100 text-yellow-800 border-yellow-200',
      'SystemOperation': 'bg-purple-100 text-purple-800 border-purple-200',
      'DatabaseManagement': 'bg-red-100 text-red-800 border-red-200'
    };
    
    return logTypeClasses[logType] || 'bg-gray-100 text-gray-800 border-gray-200';
  }

  getActionBadgeClass(action: string): string {
    // Normalize action name for badge classes (handle both old and new terminology)
    const normalizedAction = action.toLowerCase().replace(/\s+/g, '-');
    const actionLower = normalizedAction.replace('pipeline', 'stage-history');
    
    const badgeClasses: { [key: string]: string } = {
      'create': 'bg-green-100 text-green-800 border-green-200',
      'update': 'bg-blue-100 text-blue-800 border-blue-200',
      'delete': 'bg-red-100 text-red-800 border-red-200',
      'view': 'bg-gray-100 text-gray-800 border-gray-200',
      'login': 'bg-yellow-100 text-yellow-800 border-yellow-200',
      'view-users': 'bg-indigo-100 text-indigo-800 border-indigo-200',
      'view-candidates': 'bg-emerald-100 text-emerald-800 border-emerald-200',
      'view-applications': 'bg-cyan-100 text-cyan-800 border-cyan-200',
      'view-requisitions': 'bg-pink-100 text-pink-800 border-pink-200',
      'view-stage-history': 'bg-yellow-100 text-yellow-800 border-yellow-200',
      'view-roles': 'bg-amber-100 text-amber-800 border-amber-200',
      'create-stage-history': 'bg-green-100 text-green-800 border-green-200',
      'update-stage-history': 'bg-blue-100 text-blue-800 border-blue-200',
      'delete-stage-history': 'bg-red-100 text-red-800 border-red-200',
      'create-stage-history-entry': 'bg-green-100 text-green-800 border-green-200',
      'update-stage-history-entry': 'bg-blue-100 text-blue-800 border-blue-200',
      'delete-stage-history-entry': 'bg-red-100 text-red-800 border-red-200'
    };
    
    return badgeClasses[actionLower] || 'bg-gray-100 text-gray-800 border-gray-200';
  }

  getEntityDisplayName(entity: string): string {
    const entityMap: { [key: string]: string } = {
      'uer': 'Users',
      'user': 'Users',
      'candidate': 'Candidates',
      'application': 'Applications',
      'requisition': 'Requisitions',
      'requiition': 'Requisitions',
      'tagehitory': 'Stage History',
      'stagehitory': 'Stage History',
      'stagehistory': 'Stage History',
      'role': 'Roles',
      'permission': 'Permissions'
    };
    return entityMap[entity.toLowerCase()] || entity;
  }

  getActionDisplayName(action: string): string {
    const actionMap: { [key: string]: string } = {
      'View Pipeline': 'View Stage History',
      'Create Pipeline Entry': 'Create Stage History Entry',
      'Update Pipeline Entry': 'Update Stage History Entry',
      'Delete Pipeline Entry': 'Delete Stage History Entry'
    };
    return actionMap[action] || action;
  }

  getDisplayIpAddress(ipAddress?: string): string {
    if (!ipAddress) {
      return 'N/A';
    }
    
    return ipAddress;
  }

  viewLogDetails(log: AuditLog): void {
    console.log('Opening log details for:', log);
    this.selectedLog = log;
    console.log('Selected log set to:', this.selectedLog);
  }

  closeLogDetails(): void {
    console.log('Closing log details dialog');
    this.selectedLog = null;
  }

  getTotalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  getPageNumbers(): number[] {
    const totalPages = this.getTotalPages();
    const current = this.currentPage;
    const pages: number[] = [];
    
    // Show up to 5 page numbers
    const start = Math.max(0, current - 2);
    const end = Math.min(totalPages - 1, start + 4);
    
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  goToPage(page: number): void {
    this.currentPage = page;
    this.loadAuditLogs();
  }

  previousPage(): void {
    if (this.currentPage > 0) {
      this.currentPage--;
      this.loadAuditLogs();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.getTotalPages() - 1) {
      this.currentPage++;
      this.loadAuditLogs();
    }
  }

}
