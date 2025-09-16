import { Component, OnInit, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DatabaseService, TableCounts, SeedDataRequest } from '../../services/database.service';
import { DataRefreshService } from '../../services/data-refresh.service';

@Component({
  selector: 'app-database-management',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatSnackBarModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule
  ],
  templateUrl: './database-management.html',
  styleUrls: ['./database-management.scss']
})
export class DatabaseManagementComponent implements OnInit {
  tableCounts: TableCounts | null = null;
  isLoadingCounts = false;
  isClearing = false;
  
  // Table selection for clearing
  selectedTables: { [key: string]: boolean } = {};
  
  // Search and UI state
  searchTerm = '';
  expandedCategories: { [key: string]: boolean } = {
    'hiring': true,
    'system': true,
    'users': true
  };
  availableTables = [
    { name: 'Candidates', description: 'All candidate profiles', category: 'hiring' },
    { name: 'Requisitions', description: 'All job postings', category: 'hiring' },
    { name: 'Applications', description: 'All job applications', category: 'hiring' },
    { name: 'StageHistories', description: 'All stage progression records', category: 'hiring' },
    { name: 'AuditLogs', description: 'All system activity logs', category: 'system' },
    { name: 'RefreshTokens', description: 'All authentication tokens', category: 'system' },
    { name: 'UserRoles', description: 'User role assignments', category: 'users' },
    { name: 'RolePermissions', description: 'Role permission mappings', category: 'users' },
    { name: 'Users', description: 'All user accounts', category: 'users' },
    { name: 'Roles', description: 'System roles', category: 'users' },
    { name: 'Permissions', description: 'System permissions', category: 'users' }
  ];

  constructor(
    private databaseService: DatabaseService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private dataRefreshService: DataRefreshService
  ) {}

  ngOnInit(): void {
    this.loadTableCounts();
  }

  loadTableCounts(): void {
    this.isLoadingCounts = true;
    this.databaseService.getTableCounts().subscribe({
      next: (counts) => {
        this.tableCounts = counts;
        this.isLoadingCounts = false;
      },
      error: (error) => {
        console.error('Error loading table counts:', error);
        this.snackBar.open('Failed to load table counts', 'Close', { duration: 5000 });
        this.isLoadingCounts = false;
      }
    });
  }

  clearHiringData(): void {
    if (confirm('Are you sure you want to clear all hiring pipeline data? This action cannot be undone!')) {
      this.isClearing = true;
      this.databaseService.clearHiringPipelineData().subscribe({
        next: () => {
          this.snackBar.open('Hiring pipeline data cleared successfully', 'Close', { 
            duration: 5000,
            panelClass: ['success-snackbar']
          });
          this.isClearing = false;
          this.loadTableCounts(); // Refresh the counts
          
          // Notify other components about the data clearing
          this.dataRefreshService.notifyHiringDataCleared();
        },
        error: (error) => {
          console.error('Error clearing data:', error);
          this.snackBar.open('Failed to clear hiring pipeline data', 'Close', { duration: 5000 });
          this.isClearing = false;
        }
      });
    }
  }

  clearSelectedData(): void {
    const selectedTableNames = this.getSelectedTableNames();
    
    if (selectedTableNames.length === 0) {
      this.snackBar.open('Please select at least one table to clear', 'Close', { duration: 3000 });
      return;
    }

    // Show confirmation dialog
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '500px',
      data: {
        title: 'Confirm Data Clearing',
        message: `Are you sure you want to permanently delete ${selectedTableNames.length} table(s)?`,
        details: selectedTableNames,
        confirmText: 'Yes, Clear Data',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.isClearing = true;
        this.databaseService.clearSelectedTables(selectedTableNames).subscribe({
          next: () => {
            this.snackBar.open(`Successfully cleared ${selectedTableNames.length} table(s)`, 'Close', { 
              duration: 5000,
              panelClass: ['success-snackbar']
            });
            this.isClearing = false;
            this.clearSelection();
            this.loadTableCounts(); // Refresh the counts
            
            // Notify other components about the data clearing
            this.dataRefreshService.notifyTablesCleared(selectedTableNames);
            
            // If audit logs were cleared, notify specifically
            if (selectedTableNames.includes('AuditLogs')) {
              this.dataRefreshService.notifyAuditLogsCleared();
            }
          },
          error: (error) => {
            console.error('Error clearing selected data:', error);
            this.snackBar.open('Failed to clear selected data', 'Close', { duration: 5000 });
            this.isClearing = false;
          }
        });
      }
    });
  }

  seedDatabase(): void {
    if (confirm('Are you sure you want to seed the database with test data? This will add sample candidates, requisitions, applications, and stage history entries.')) {
      this.isClearing = true;
      this.databaseService.seedDatabase().subscribe({
        next: (response) => {
          this.snackBar.open('Database seeded successfully with test data', 'Close', { 
            duration: 5000,
            panelClass: ['success-snackbar']
          });
          this.isClearing = false;
          this.loadTableCounts(); // Refresh the counts
          
          // Notify other components about the data seeding
          this.dataRefreshService.notifyHiringDataCleared();
        },
        error: (error) => {
          console.error('Error seeding database:', error);
          this.snackBar.open('Failed to seed database with test data', 'Close', { duration: 5000 });
          this.isClearing = false;
        }
      });
    }
  }

  getSelectedTableNames(): string[] {
    return Object.keys(this.selectedTables).filter(key => this.selectedTables[key]);
  }

  getSelectedCount(): number {
    return this.getSelectedTableNames().length;
  }

  selectAll(): void {
    this.availableTables.forEach(table => {
      this.selectedTables[table.name] = true;
    });
  }

  clearSelection(): void {
    this.selectedTables = {};
  }

  selectByCategory(category: string): void {
    this.availableTables
      .filter(table => table.category === category)
      .forEach(table => {
        this.selectedTables[table.name] = true;
      });
  }

  getTablesByCategory(category: string) {
    return this.availableTables.filter(table => table.category === category);
  }

  getCategoryName(category: string): string {
    const categoryNames: { [key: string]: string } = {
      'hiring': 'Hiring Pipeline Data',
      'system': 'System Data',
      'users': 'User Management Data'
    };
    return categoryNames[category] || category;
  }

  getTableCountEntries(): { name: string; value: number }[] {
    if (!this.tableCounts) return [];
    
    return Object.entries(this.tableCounts)
      .map(([name, value]) => ({ name, value }))
      .sort((a, b) => a.name.localeCompare(b.name));
  }

  getTableIconClass(tableName: string): string {
    const iconClasses: { [key: string]: string } = {
      'Users': 'bg-blue-500',
      'Roles': 'bg-purple-500',
      'UserRoles': 'bg-indigo-500',
      'Permissions': 'bg-green-500',
      'RolePermissions': 'bg-teal-500',
      'Candidates': 'bg-orange-500',
      'Requisitions': 'bg-pink-500',
      'Applications': 'bg-cyan-500',
      'StageHistories': 'bg-yellow-500',
      'AuditLogs': 'bg-gray-500',
      'RefreshTokens': 'bg-red-500'
    };
    return iconClasses[tableName] || 'bg-gray-400';
  }

  getTableIcon(tableName: string): string {
    const icons: { [key: string]: string } = {
      'Users': 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z',
      'Roles': 'M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z',
      'UserRoles': 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z',
      'Permissions': 'M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z',
      'RolePermissions': 'M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z',
      'Candidates': 'M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z',
      'Requisitions': 'M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2-2v2m8 0V6a2 2 0 012 2v6a2 2 0 01-2 2H6a2 2 0 01-2-2V8a2 2 0 012-2V6',
      'Applications': 'M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2',
      'StageHistories': 'M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z',
      'AuditLogs': 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z',
      'RefreshTokens': 'M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15'
    };
    return icons[tableName] || 'M4 6h16M4 10h16M4 14h16M4 18h16';
  }

  // 🔹 New methods for enhanced functionality

  getCategoryTotal(category: string): number {
    if (!this.tableCounts) return 0;
    
    const tablesInCategory = this.availableTables
      .filter(table => table.category === category)
      .map(table => table.name);
    
    return tablesInCategory.reduce((total, tableName) => {
      return total + (this.tableCounts?.[tableName] || 0);
    }, 0);
  }

  toggleCategory(category: string): void {
    this.expandedCategories[category] = !this.expandedCategories[category];
  }

  getFilteredTablesByCategory(category: string) {
    let tables = this.getTablesByCategory(category);
    
    if (this.searchTerm.trim()) {
      const searchLower = this.searchTerm.toLowerCase();
      tables = tables.filter(table => 
        table.name.toLowerCase().includes(searchLower) ||
        table.description.toLowerCase().includes(searchLower)
      );
    }
    
    return tables;
  }

  getCategoryIconClass(category: string): string {
    const iconClasses: { [key: string]: string } = {
      'hiring': 'bg-blue-500',
      'system': 'bg-gray-500',
      'users': 'bg-purple-500'
    };
    return iconClasses[category] || 'bg-gray-400';
  }

  getCategoryIcon(category: string): string {
    const icons: { [key: string]: string } = {
      'hiring': 'M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2-2v2m8 0V6a2 2 0 012 2v6a2 2 0 01-2 2H6a2 2 0 01-2-2V8a2 2 0 012-2V6',
      'system': 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z',
      'users': 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z'
    };
    return icons[category] || 'M4 6h16M4 10h16M4 14h16M4 18h16';
  }

  getTableTooltip(tableName: string): string {
    const tooltips: { [key: string]: string } = {
      'Users': 'Contains all user accounts and profile information',
      'Roles': 'System roles that define user permissions',
      'UserRoles': 'Mappings between users and their assigned roles',
      'Permissions': 'Individual permissions that can be granted to roles',
      'RolePermissions': 'Mappings between roles and their permissions',
      'Candidates': 'All candidate profiles and personal information',
      'Requisitions': 'Job postings and position requirements',
      'Applications': 'Job applications submitted by candidates',
      'StageHistories': 'Records of candidates moving through hiring stages',
      'AuditLogs': 'System activity logs for tracking user actions',
      'RefreshTokens': 'Authentication tokens for maintaining user sessions'
    };
    return tooltips[tableName] || `Contains ${tableName.toLowerCase()} data`;
  }

  /**
   * Opens the custom seeding dialog
   */
  openCustomSeedDialog(): void {
    const dialogRef = this.dialog.open(CustomSeedDialogComponent, {
      width: '600px',
      maxWidth: '95vw',
      maxHeight: '90vh'
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.seedDatabaseCustom(result);
      }
    });
  }

  /**
   * Seeds the database with custom data
   */
  seedDatabaseCustom(request: SeedDataRequest): void {
    this.isClearing = true;
    this.databaseService.seedDatabaseCustom(request).subscribe({
      next: (response) => {
        this.snackBar.open(`Custom data seeding completed successfully! Created: ${JSON.stringify(response.data)}`, 'Close', { 
          duration: 8000,
          panelClass: ['success-snackbar']
        });
        this.isClearing = false;
        this.loadTableCounts(); // Refresh the counts
        
        // Notify other components about the data seeding
        this.dataRefreshService.notifyHiringDataCleared();
      },
      error: (error) => {
        console.error('Error seeding database with custom data:', error);
        this.snackBar.open('Failed to seed database with custom data', 'Close', { duration: 5000 });
        this.isClearing = false;
      }
    });
  }
}

// 🔹 Confirmation Dialog Component
@Component({
  selector: 'app-confirmation-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  template: `
    <div class="p-8">
      <div class="flex items-center gap-4 mb-6">
        <div class="w-14 h-14 rounded-2xl bg-gradient-to-r from-red-500 to-red-600 flex items-center justify-center shadow-lg">
          <svg class="w-7 h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M5 19h14c1 0 2-1 1-2L13 5a2 2 0 00-2 0L4 17c-1 1 0 2 1 2z"/>
          </svg>
        </div>
        <h2 class="text-xl font-bold text-gray-900">{{ data.title }}</h2>
      </div>
      
      <p class="text-gray-700 mb-6 text-lg">{{ data.message }}</p>
      
      <div *ngIf="data.details && data.details.length > 0" class="mb-6">
        <h3 class="text-lg font-bold text-gray-900 mb-4">Tables to be cleared:</h3>
        <div class="bg-red-50 border border-red-200 rounded-2xl p-6 shadow-lg">
          <ul class="text-red-800 space-y-3">
            <li *ngFor="let detail of data.details" class="flex items-center gap-3">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
              </svg>
              <span class="font-medium">{{ detail }}</span>
            </li>
          </ul>
        </div>
      </div>
      
      <div class="flex justify-end gap-4">
        <button 
          mat-button 
          (click)="onCancel()"
          class="px-8 py-4 text-gray-700 bg-white border-2 border-gray-300 rounded-2xl font-bold hover:bg-gray-50 hover:border-gray-400 transition-all duration-300 shadow-lg hover:shadow-xl focus:outline-none focus:ring-4 focus:ring-gray-100 transform hover:scale-105">
          {{ data.cancelText || 'Cancel' }}
        </button>
        <button 
          mat-button 
          (click)="onConfirm()"
          class="px-8 py-4 bg-gradient-to-r from-red-600 to-red-700 text-white rounded-2xl font-bold hover:from-red-700 hover:to-red-800 transition-all duration-300 shadow-lg hover:shadow-xl focus:outline-none focus:ring-4 focus:ring-red-100 transform hover:scale-105">
          {{ data.confirmText || 'Confirm' }}
        </button>
      </div>
    </div>
  `
})
export class ConfirmationDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ConfirmationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onConfirm(): void {
    this.dialogRef.close(true);
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}

// 🔹 Custom Seed Dialog Component
@Component({
  selector: 'app-custom-seed-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <div class="bg-white rounded-3xl shadow-2xl w-full max-w-4xl mx-auto overflow-hidden max-h-[90vh] flex flex-col border border-gray-100 transform transition-all duration-300 scale-100">
    <!-- Dialog Header -->
    <div class="bg-gradient-to-r from-green-600 via-green-700 to-emerald-700 px-8 py-6 text-white relative overflow-hidden">
      <!-- Background Pattern -->
      <div class="absolute inset-0 opacity-10">
        <div class="absolute top-0 right-0 w-32 h-32 bg-white rounded-full -translate-y-16 translate-x-16"></div>
        <div class="absolute bottom-0 left-0 w-24 h-24 bg-white rounded-full translate-y-12 -translate-x-12"></div>
      </div>
      
      <div class="flex items-center gap-4 relative z-10">
        <div class="p-3 bg-white bg-opacity-20 rounded-xl shadow-lg backdrop-blur-sm">
          <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
          </svg>
        </div>
        <div>
          <h2 class="text-xl font-bold text-white">Custom Data Seeding</h2>
          <p class="text-green-100 mt-1">Choose what type of data to create and how many records of each type.</p>
        </div>
      </div>
    </div>

    <!-- Form Container -->
    <div class="flex-1 bg-gray-50 overflow-y-auto">
      <div class="px-8 py-8">
        <div class="bg-white rounded-2xl p-8 border border-gray-200 shadow-lg">
      
          <form #seedForm="ngForm" class="space-y-6">
            <!-- Candidates -->
            <div class="space-y-3">
              <label class="block text-sm font-bold text-gray-900">Candidates</label>
              <input 
                type="number" 
                [(ngModel)]="seedData.candidatesCount" 
                name="candidatesCount"
                min="0" 
                max="100"
                class="w-full px-4 py-4 bg-white border-2 border-gray-200 rounded-2xl text-sm text-gray-800 font-medium transition-all duration-300 shadow-lg hover:shadow-xl hover:border-gray-300 focus:outline-none focus:scale-105 focus:border-green-500 focus:ring-4 focus:ring-green-100"
                placeholder="Number of candidates to create">
              <p class="text-xs text-gray-600 font-medium">Create candidate profiles with realistic data</p>
            </div>

            <!-- Requisitions -->
            <div class="space-y-3">
              <label class="block text-sm font-bold text-gray-900">Job Requisitions</label>
              <input 
                type="number" 
                [(ngModel)]="seedData.requisitionsCount" 
                name="requisitionsCount"
                min="0" 
                max="50"
                class="w-full px-4 py-4 bg-white border-2 border-gray-200 rounded-2xl text-sm text-gray-800 font-medium transition-all duration-300 shadow-lg hover:shadow-xl hover:border-gray-300 focus:outline-none focus:scale-105 focus:border-green-500 focus:ring-4 focus:ring-green-100"
                placeholder="Number of job requisitions to create">
              <p class="text-xs text-gray-600 font-medium">Create job postings with various positions and departments</p>
            </div>

            <!-- Applications -->
            <div class="space-y-3">
              <label class="block text-sm font-bold text-gray-900">Applications</label>
              <input 
                type="number" 
                [(ngModel)]="seedData.applicationsCount" 
                name="applicationsCount"
                min="0" 
                max="200"
                class="w-full px-4 py-4 bg-white border-2 border-gray-200 rounded-2xl text-sm text-gray-800 font-medium transition-all duration-300 shadow-lg hover:shadow-xl hover:border-gray-300 focus:outline-none focus:scale-105 focus:border-green-500 focus:ring-4 focus:ring-green-100"
                placeholder="Number of applications to create">
              <p class="text-xs text-gray-600 font-medium">Create job applications linking candidates to requisitions</p>
            </div>

            <!-- Stage History -->
            <div class="space-y-3">
              <label class="block text-sm font-bold text-gray-900">Stage History Entries</label>
              <input 
                type="number" 
                [(ngModel)]="seedData.stageHistoryCount" 
                name="stageHistoryCount"
                min="0" 
                max="500"
                class="w-full px-4 py-4 bg-white border-2 border-gray-200 rounded-2xl text-sm text-gray-800 font-medium transition-all duration-300 shadow-lg hover:shadow-xl hover:border-gray-300 focus:outline-none focus:scale-105 focus:border-green-500 focus:ring-4 focus:ring-green-100"
                placeholder="Number of stage history entries to create">
              <p class="text-xs text-gray-600 font-medium">Create stage progression records for applications</p>
            </div>

            <!-- Additional Users -->
            <div class="space-y-3">
              <label class="flex items-center gap-3">
                <input 
                  type="checkbox" 
                  [(ngModel)]="seedData.createUsers" 
                  name="createUsers"
                  class="w-5 h-5 rounded border-2 border-gray-300 text-green-600 focus:ring-4 focus:ring-green-100">
                <span class="text-sm font-bold text-gray-900">Create Additional Users</span>
              </label>
              <p class="text-xs text-gray-600 font-medium">Create 5 additional test users with different roles</p>
            </div>

            <!-- Info Box -->
            <div class="bg-gradient-to-r from-blue-50 to-blue-100 border border-blue-200 rounded-2xl p-6 shadow-lg">
              <div class="flex items-start gap-4">
                <svg class="w-6 h-6 text-blue-600 mt-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                <div>
                  <h4 class="text-lg font-bold text-blue-900 mb-3">Important Notes:</h4>
                  <ul class="text-blue-800 space-y-2">
                    <li class="flex items-center gap-2">
                      <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4"/>
                      </svg>
                      <span class="font-medium">Applications require candidates and requisitions to exist first</span>
                    </li>
                    <li class="flex items-center gap-2">
                      <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4"/>
                      </svg>
                      <span class="font-medium">Stage history requires applications to exist first</span>
                    </li>
                    <li class="flex items-center gap-2">
                      <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4"/>
                      </svg>
                      <span class="font-medium">All data is created with realistic, randomized values</span>
                    </li>
                    <li class="flex items-center gap-2">
                      <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4"/>
                      </svg>
                      <span class="font-medium">Existing data will not be modified, only new records added</span>
                    </li>
                  </ul>
                </div>
              </div>
            </div>
          </form>
        </div>
      </div>

      <!-- Footer Buttons -->
      <div class="px-8 py-6 bg-gradient-to-r from-gray-50 to-gray-100 border-t border-gray-200 flex-shrink-0">
        <div class="flex flex-col sm:flex-row justify-end gap-4">
          <button 
            mat-button 
            (click)="onCancel()"
            class="w-full sm:w-auto px-8 py-4 text-gray-700 bg-white border-2 border-gray-300 rounded-2xl font-bold hover:bg-gray-50 hover:border-gray-400 transition-all duration-300 shadow-lg hover:shadow-xl text-sm order-2 sm:order-1 focus:outline-none focus:ring-4 focus:ring-gray-100 transform hover:scale-105">
            Cancel
          </button>
          <button 
            mat-button 
            (click)="onConfirm()"
            [disabled]="!isValidInput()"
            class="w-full sm:w-auto px-8 py-4 bg-gradient-to-r from-green-600 to-emerald-600 text-white rounded-2xl font-bold hover:from-green-700 hover:to-emerald-700 transition-all duration-300 shadow-lg hover:shadow-xl text-sm order-1 sm:order-2 focus:outline-none focus:ring-4 focus:ring-green-100 transform hover:scale-105 disabled:opacity-50 disabled:cursor-not-allowed disabled:transform-none">
            Create Data
          </button>
        </div>
      </div>
    </div>
    </div>
  `
})
export class CustomSeedDialogComponent {
  seedData: SeedDataRequest = {
    candidatesCount: 0,
    requisitionsCount: 0,
    applicationsCount: 0,
    stageHistoryCount: 0,
    createUsers: false
  };

  constructor(
    public dialogRef: MatDialogRef<CustomSeedDialogComponent>
  ) {}

  isValidInput(): boolean {
    return this.seedData.candidatesCount > 0 || 
           this.seedData.requisitionsCount > 0 || 
           this.seedData.applicationsCount > 0 || 
           this.seedData.stageHistoryCount > 0 || 
           this.seedData.createUsers;
  }

  onConfirm(): void {
    this.dialogRef.close(this.seedData);
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }
}
