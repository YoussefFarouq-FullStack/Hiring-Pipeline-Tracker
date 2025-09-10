import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user.model';
import { UserDialogComponent } from './user-dialog/user-dialog';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    MatCardModule,
    MatChipsModule,
    MatMenuModule
  ],
  template: `
    <div class="users-container">
      <!-- Header -->
      <div class="header-section">
        <div class="header-content">
          <h1 class="page-title">User Management</h1>
          <p class="page-subtitle">Manage system users and their roles</p>
        </div>
        <button 
          *ngIf="canCreateUser()" 
          (click)="openDialog()" 
          class="btn btn-primary"
        >
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
          </svg>
          Add New User
        </button>
      </div>

      <!-- Users Table -->
      <div class="table-container">
        <div class="table-header">
          <h3 class="table-title">System Users</h3>
          <div class="table-actions">
            <button 
              (click)="loadUsers()" 
              class="btn btn-secondary"
              [disabled]="isLoading"
            >
              <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
              </svg>
              {{ isLoading ? 'Loading...' : 'Refresh' }}
            </button>
          </div>
        </div>

        <!-- Loading State -->
        <div *ngIf="isLoading" class="loading-state">
          <div class="loading-spinner"></div>
          <p>Loading users...</p>
        </div>

        <!-- Empty State -->
        <div *ngIf="!isLoading && users.length === 0" class="empty-state">
          <svg class="w-16 h-16 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"></path>
          </svg>
          <h3 class="empty-title">No Users Found</h3>
          <p class="empty-description">Get started by adding your first user to the system.</p>
          <button 
            *ngIf="canCreateUser()" 
            (click)="openDialog()" 
            class="btn btn-primary"
          >
            <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
            </svg>
            Add First User
          </button>
        </div>

        <!-- Users Table -->
        <div *ngIf="!isLoading && users.length > 0" class="table-wrapper">
          <table class="users-table">
            <thead>
              <tr>
                <th>User</th>
                <th>Email</th>
                <th>Roles</th>
                <th>Status</th>
                <th>Last Login</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let user of users" class="user-row">
                <td class="user-info">
                  <div class="user-avatar">
                    <svg class="w-8 h-8" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd"></path>
                    </svg>
                  </div>
                  <div class="user-details">
                    <div class="user-name">{{ user.firstName }} {{ user.lastName }}</div>
                    <div class="user-username">@{{ user.username }}</div>
                  </div>
                </td>
                <td class="user-email">{{ user.email }}</td>
                <td class="user-roles">
                  <span 
                    *ngFor="let role of user.roles" 
                    class="role-chip"
                    [ngClass]="getRoleChipClass(role)"
                  >
                    {{ role || 'Unknown' }}
                  </span>
                  <span 
                    *ngIf="!user.roles || user.roles.length === 0" 
                    class="role-chip no-role"
                  >
                    No Role
                  </span>
                </td>
                <td class="user-status">
                  <span 
                    class="status-badge"
                    [ngClass]="user.isActive ? 'status-active' : 'status-inactive'"
                  >
                    {{ user.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td class="user-last-login">
                  {{ user.lastLoginAt ? (user.lastLoginAt | date:'short') : 'Never' }}
                </td>
                <td class="user-actions">
                  <button 
                    *ngIf="canEditUser(user)" 
                    (click)="openDialog(user)" 
                    class="action-btn edit-btn"
                    title="Edit User"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
                    </svg>
                  </button>
                  <button 
                    *ngIf="canDeleteUser(user)" 
                    (click)="deleteUser(user)" 
                    class="action-btn delete-btn"
                    title="Delete User"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                    </svg>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .users-container {
      padding: 1.5rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    .header-section {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 2rem;
      padding-bottom: 1rem;
      border-bottom: 1px solid #e5e7eb;
    }

    .page-title {
      font-size: 1.875rem;
      font-weight: 700;
      color: #111827;
      margin: 0;
    }

    .page-subtitle {
      color: #6b7280;
      margin: 0.5rem 0 0 0;
    }

    .table-container {
      background: white;
      border-radius: 0.5rem;
      box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
    }

    .table-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1.5rem;
      border-bottom: 1px solid #e5e7eb;
    }

    .table-title {
      font-size: 1.125rem;
      font-weight: 600;
      color: #111827;
      margin: 0;
    }

    .loading-state, .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 4rem 2rem;
      text-align: center;
    }

    .loading-spinner {
      width: 2rem;
      height: 2rem;
      border: 2px solid #e5e7eb;
      border-top: 2px solid #3b82f6;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .empty-title {
      font-size: 1.125rem;
      font-weight: 600;
      color: #111827;
      margin: 1rem 0 0.5rem 0;
    }

    .empty-description {
      color: #6b7280;
      margin: 0 0 1.5rem 0;
    }

    .table-wrapper {
      overflow-x: auto;
    }

    .users-table {
      width: 100%;
      border-collapse: collapse;
      table-layout: fixed;
    }

    .users-table th {
      background-color: #f9fafb;
      padding: 0.75rem 1rem;
      text-align: left;
      font-weight: 600;
      color: #374151;
      border-bottom: 1px solid #e5e7eb;
    }

    .users-table th:nth-child(1) { width: 25%; } /* User */
    .users-table th:nth-child(2) { width: 20%; } /* Email */
    .users-table th:nth-child(3) { width: 15%; } /* Roles */
    .users-table th:nth-child(4) { width: 10%; } /* Status */
    .users-table th:nth-child(5) { width: 15%; } /* Last Login */
    .users-table th:nth-child(6) { width: 15%; } /* Actions */

    .users-table td {
      padding: 1rem;
      border-bottom: 1px solid #e5e7eb;
      word-wrap: break-word;
    }

    .user-row:hover {
      background-color: #f9fafb;
    }

    .user-info {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .user-avatar {
      width: 2rem;
      height: 2rem;
      background-color: #e5e7eb;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #6b7280;
    }

    .user-name {
      font-weight: 500;
      color: #111827;
    }

    .user-username {
      font-size: 0.875rem;
      color: #6b7280;
    }

    .user-email {
      color: #374151;
    }

    .user-roles {
      display: flex;
      gap: 0.25rem;
      flex-wrap: wrap;
    }

    .role-chip {
      padding: 0.25rem 0.5rem;
      border-radius: 0.25rem;
      font-size: 0.75rem;
      font-weight: 500;
    }

    .role-chip.admin {
      background-color: #fef2f2;
      color: #dc2626;
    }

    .role-chip.recruiter {
      background-color: #eff6ff;
      color: #2563eb;
    }

    .role-chip.hiring-manager {
      background-color: #f0fdf4;
      color: #16a34a;
    }

    .role-chip.interviewer {
      background-color: #fefce8;
      color: #ca8a04;
    }

    .role-chip.read-only {
      background-color: #f3f4f6;
      color: #6b7280;
    }

    .role-chip.no-role {
      background-color: #fef2f2;
      color: #dc2626;
    }

    .status-badge {
      padding: 0.25rem 0.5rem;
      border-radius: 0.25rem;
      font-size: 0.75rem;
      font-weight: 500;
    }

    .status-active {
      background-color: #f0fdf4;
      color: #16a34a;
    }

    .status-inactive {
      background-color: #fef2f2;
      color: #dc2626;
    }

    .user-last-login {
      color: #6b7280;
      font-size: 0.875rem;
    }

    .user-actions {
      display: flex;
      gap: 0.5rem;
    }

    .action-btn {
      padding: 0.5rem;
      border: none;
      border-radius: 0.25rem;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .edit-btn {
      background-color: #eff6ff;
      color: #2563eb;
    }

    .edit-btn:hover {
      background-color: #dbeafe;
    }

    .delete-btn {
      background-color: #fef2f2;
      color: #dc2626;
    }

    .delete-btn:hover {
      background-color: #fee2e2;
    }

    .btn {
      display: inline-flex;
      align-items: center;
      padding: 0.5rem 1rem;
      border-radius: 0.375rem;
      font-weight: 500;
      text-decoration: none;
      border: none;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn-primary {
      background-color: #3b82f6;
      color: white;
    }

    .btn-primary:hover {
      background-color: #2563eb;
    }

    .btn-secondary {
      background-color: #f3f4f6;
      color: #374151;
    }

    .btn-secondary:hover {
      background-color: #e5e7eb;
    }

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  `]
})
export class UsersComponent implements OnInit {
  users: User[] = [];
  isLoading = false;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.userService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.snackBar.open('Failed to load users', 'Close', { duration: 5000 });
        this.isLoading = false;
      }
    });
  }

  openDialog(user?: User): void {
    const dialogRef = this.dialog.open(UserDialogComponent, {
      width: '600px',
      data: { user, mode: user ? 'edit' : 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers();
      }
    });
  }

  deleteUser(user: User): void {
    if (confirm(`Are you sure you want to delete user "${user.firstName} ${user.lastName}"?`)) {
      this.userService.deleteUser(user.id).subscribe({
        next: () => {
          this.snackBar.open('User deleted successfully', 'Close', { duration: 5000 });
          this.loadUsers();
        },
        error: (error) => {
          console.error('Error deleting user:', error);
          this.snackBar.open('Failed to delete user', 'Close', { duration: 5000 });
        }
      });
    }
  }

  canCreateUser(): boolean {
    const user = this.authService.getCurrentUser();
    return user?.role?.toLowerCase() === 'admin';
  }

  canEditUser(user: User): boolean {
    const currentUser = this.authService.getCurrentUser();
    return currentUser?.role?.toLowerCase() === 'admin';
  }

  canDeleteUser(user: User): boolean {
    const currentUser = this.authService.getCurrentUser();
    return currentUser?.role?.toLowerCase() === 'admin' && user.id !== currentUser.id;
  }

  getRoleChipClass(role: string | undefined): string {
    if (!role) {
      return 'role-chip';
    }
    const roleClass = role.toLowerCase().replace(' ', '-');
    return `role-chip ${roleClass}`;
  }
}
