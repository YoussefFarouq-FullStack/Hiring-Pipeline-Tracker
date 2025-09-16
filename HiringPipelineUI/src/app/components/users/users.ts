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
import { ConfirmationDialogService } from '../shared/confirmation-dialog/confirmation-dialog.service';
import { LoadingSpinnerComponent } from '../shared/loading-spinner/loading-spinner.component';

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
    MatMenuModule,
    LoadingSpinnerComponent
  ],
  templateUrl: './users.html',
  styleUrls: ['./users.scss']
})
export class UsersComponent implements OnInit {
  users: User[] = [];
  isLoading = false;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private confirmationDialog: ConfirmationDialogService
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
    this.confirmationDialog.confirmDanger(
      'Delete User',
      `Are you sure you want to delete user "${user.firstName} ${user.lastName}"? This action cannot be undone.`,
      'Delete',
      'Cancel'
    ).subscribe(confirmed => {
      if (confirmed) {
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
    });
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
