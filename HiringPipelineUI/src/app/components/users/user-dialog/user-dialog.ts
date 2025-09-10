import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { UserService } from '../../../services/user.service';
import { RoleService } from '../../../services/role.service';
import { User, Role } from '../../../models/user.model';

@Component({
  selector: 'app-user-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatSnackBarModule,
    MatCheckboxModule
  ],
  template: `
    <div class="dialog-header">
      <h2 class="dialog-title">
        {{ isEditMode ? 'Edit User' : 'Add New User' }}
      </h2>
      <p class="dialog-subtitle">
        {{ isEditMode ? 'Update the user information below.' : 'Fill in the details to add a new user to the system.' }}
      </p>
    </div>

    <form [formGroup]="userForm" (ngSubmit)="save()" class="dialog-form">
      <!-- User ID (Edit Mode Only) -->
      <div *ngIf="isEditMode" class="form-group">
        <label class="form-label">User ID</label>
        <input
          formControlName="id"
          type="text"
          class="form-input-modern readonly-field"
          readonly
        />
        <p class="field-help-text">This field cannot be modified</p>
      </div>

      <!-- Username -->
      <div class="form-group">
        <label class="form-label">
          Username <span class="text-red-500">*</span>
        </label>
        <input
          formControlName="username"
          type="text"
          class="form-input-modern"
          [ngClass]="{'error': usernameError}"
          placeholder="Enter username"
          required
        />
        <div *ngIf="usernameError" class="form-error">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
          </svg>
          {{ usernameError }}
        </div>
      </div>

      <!-- Email -->
      <div class="form-group">
        <label class="form-label">
          Email <span class="text-red-500">*</span>
        </label>
        <input
          formControlName="email"
          type="email"
          class="form-input-modern"
          [ngClass]="{'error': emailError}"
          placeholder="Enter email address"
          required
        />
        <div *ngIf="emailError" class="form-error">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
          </svg>
          {{ emailError }}
        </div>
      </div>

      <!-- First Name -->
      <div class="form-group">
        <label class="form-label">
          First Name <span class="text-red-500">*</span>
        </label>
        <input
          formControlName="firstName"
          type="text"
          class="form-input-modern"
          [ngClass]="{'error': firstNameError}"
          placeholder="Enter first name"
          required
        />
        <div *ngIf="firstNameError" class="form-error">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
          </svg>
          {{ firstNameError }}
        </div>
      </div>

      <!-- Last Name -->
      <div class="form-group">
        <label class="form-label">
          Last Name <span class="text-red-500">*</span>
        </label>
        <input
          formControlName="lastName"
          type="text"
          class="form-input-modern"
          [ngClass]="{'error': lastNameError}"
          placeholder="Enter last name"
          required
        />
        <div *ngIf="lastNameError" class="form-error">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
          </svg>
          {{ lastNameError }}
        </div>
      </div>

      <!-- Password (Create Mode Only) -->
      <div *ngIf="!isEditMode" class="form-group">
        <label class="form-label">
          Password <span class="text-red-500">*</span>
        </label>
        <input
          formControlName="password"
          type="password"
          class="form-input-modern"
          [ngClass]="{'error': passwordError}"
          placeholder="Enter password"
          required
        />
        <div *ngIf="passwordError" class="form-error">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
          </svg>
          {{ passwordError }}
        </div>
      </div>

      <!-- Roles -->
      <div class="form-group">
        <label class="form-label">
          Roles <span class="text-red-500">*</span>
        </label>
        <select
          formControlName="roleId"
          class="form-input-modern"
          [ngClass]="{'error': roleError}"
          required
        >
          <option value="">Select a role</option>
          <option *ngFor="let role of roles" [value]="role.id">
            {{ role.name }} - {{ role.description }}
          </option>
        </select>
        <div *ngIf="roleError" class="form-error">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
          </svg>
          {{ roleError }}
        </div>
      </div>

      <!-- Active Status -->
      <div class="form-group">
        <label class="flex items-center space-x-2">
          <input
            formControlName="isActive"
            type="checkbox"
            class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
          />
          <span class="form-label">Active User</span>
        </label>
        <p class="text-xs text-gray-500 mt-1">Inactive users cannot log into the system</p>
      </div>

      <!-- Actions -->
      <div class="flex justify-end gap-3 pt-3 border-t border-gray-200">
        <button
          type="button"
          (click)="close()"
          [disabled]="isSubmitting"
          class="btn btn-secondary"
        >
          Cancel
        </button>
        <button
          type="submit"
          [disabled]="!userForm.valid || isSubmitting"
          class="btn btn-primary"
        >
          <svg *ngIf="!isSubmitting" class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
          </svg>
          <svg *ngIf="isSubmitting" class="animate-spin w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
          </svg>
          {{ isSubmitting ? 'Saving...' : (isEditMode ? 'Update User' : 'Add User') }}
        </button>
      </div>
    </form>
  `,
  styles: [`
    .dialog-header {
      padding: 1.5rem 1.5rem 0 1.5rem;
    }

    .dialog-title {
      font-size: 1.25rem;
      font-weight: 600;
      color: #111827;
      margin: 0 0 0.5rem 0;
    }

    .dialog-subtitle {
      color: #6b7280;
      margin: 0 0 1.5rem 0;
    }

    .dialog-form {
      padding: 1.5rem;
    }

    .form-group {
      margin-bottom: 1rem;
    }

    .form-label {
      display: block;
      font-weight: 500;
      color: #374151;
      margin-bottom: 0.5rem;
    }

    .form-input-modern {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #d1d5db;
      border-radius: 0.375rem;
      font-size: 0.875rem;
      transition: border-color 0.2s;
    }

    .form-input-modern:focus {
      outline: none;
      border-color: #3b82f6;
      box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
    }

    .form-input-modern.error {
      border-color: #dc2626;
    }

    .readonly-field {
      background-color: #f9fafb;
      color: #6b7280;
    }

    .field-help-text {
      font-size: 0.75rem;
      color: #6b7280;
      margin-top: 0.25rem;
    }

    .form-error {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      color: #dc2626;
      font-size: 0.75rem;
      margin-top: 0.25rem;
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

    .btn-primary:hover:not(:disabled) {
      background-color: #2563eb;
    }

    .btn-secondary {
      background-color: #f3f4f6;
      color: #374151;
    }

    .btn-secondary:hover:not(:disabled) {
      background-color: #e5e7eb;
    }

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .text-red-500 {
      color: #dc2626;
    }

    .text-gray-500 {
      color: #6b7280;
    }

    .text-xs {
      font-size: 0.75rem;
    }

    .mt-1 {
      margin-top: 0.25rem;
    }

    .pt-3 {
      padding-top: 0.75rem;
    }

    .border-t {
      border-top: 1px solid #e5e7eb;
    }

    .border-gray-200 {
      border-color: #e5e7eb;
    }

    .flex {
      display: flex;
    }

    .justify-end {
      justify-content: flex-end;
    }

    .gap-3 {
      gap: 0.75rem;
    }

    .items-center {
      align-items: center;
    }

    .space-x-2 > * + * {
      margin-left: 0.5rem;
    }

    .rounded {
      border-radius: 0.25rem;
    }

    .border-gray-300 {
      border-color: #d1d5db;
    }

    .text-blue-600 {
      color: #2563eb;
    }

    .focus\\:ring-blue-500:focus {
      --tw-ring-color: #3b82f6;
    }

    .w-4 {
      width: 1rem;
    }

    .h-4 {
      height: 1rem;
    }

    .mr-2 {
      margin-right: 0.5rem;
    }

    .animate-spin {
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      from { transform: rotate(0deg); }
      to { transform: rotate(360deg); }
    }
  `]
})
export class UserDialogComponent {
  userForm!: FormGroup;
  isEditMode: boolean;
  isSubmitting = false;
  roles: Role[] = [];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<UserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private userService: UserService,
    private roleService: RoleService,
    private snackBar: MatSnackBar
  ) {
    this.isEditMode = data?.mode === 'edit';
    this.loadRoles();
    this.initializeForm();
  }

  private initializeForm(): void {
    this.userForm = this.fb.group({
      id: [this.isEditMode ? this.data?.user?.id : null],
      username: [this.isEditMode ? this.data?.user?.username : '', [Validators.required, Validators.minLength(3)]],
      email: [this.isEditMode ? this.data?.user?.email : '', [Validators.required, Validators.email]],
      firstName: [this.isEditMode ? this.data?.user?.firstName : '', [Validators.required, Validators.minLength(2), Validators.pattern(/^[a-zA-Z\s]+$/)]],
      lastName: [this.isEditMode ? this.data?.user?.lastName : '', [Validators.required, Validators.minLength(2), Validators.pattern(/^[a-zA-Z\s]+$/)]],
      password: [this.isEditMode ? '' : '', this.isEditMode ? [] : [Validators.required, Validators.minLength(8)]],
      roleId: [this.isEditMode ? this.data?.user?.roles?.[0]?.id : null, Validators.required],
      isActive: [this.isEditMode ? this.data?.user?.isActive : true]
    });
  }

  private loadRoles(): void {
    this.roleService.getAllRoles().subscribe({
      next: (roles: Role[]) => {
        this.roles = roles;
      },
      error: (error: any) => {
        console.error('Error loading roles:', error);
        this.snackBar.open('Failed to load roles', 'Close', { duration: 5000 });
      }
    });
  }

  save(): void {
    if (this.userForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;

      const formData = this.userForm.value;
      console.log('Original form data:', formData);

      // Transform roleId to RoleIds array for backend compatibility
      if (formData.roleId) {
        formData.roleIds = [formData.roleId];
        delete formData.roleId;
      }

      console.log('Transformed form data:', formData);

      // For edit mode, don't send password if it's empty
      if (this.isEditMode && !formData.password) {
        delete formData.password;
      }

      // Remove isActive field as backend DTO doesn't include it
      delete formData.isActive;

      // For edit mode, don't send id
      if (this.isEditMode) {
        delete formData.id;
      }

      const operation = this.isEditMode 
        ? this.userService.updateUser(this.data.user.id, formData)
        : this.userService.createUser(formData);

      operation.subscribe({
        next: (result: User) => {
          this.snackBar.open(
            `User ${this.isEditMode ? 'updated' : 'created'} successfully`, 
            'Close', 
            { duration: 5000 }
          );
          this.dialogRef.close(result);
        },
        error: (error: any) => {
          console.error('Error saving user:', error);
          console.error('Request payload:', formData);
          console.error('Error details:', error.error);
          
          // Extract specific validation errors
          let errorMessage = 'Failed to save user';
          
          if (error.error) {
            if (typeof error.error === 'string') {
              errorMessage = error.error;
            } else if (error.error.message) {
              errorMessage = error.error.message;
            } else if (error.error.errors) {
              // Handle ModelState validation errors
              const validationErrors = Object.values(error.error.errors).flat();
              errorMessage = validationErrors.join(', ');
            } else if (error.error.title) {
              errorMessage = error.error.title;
            }
          } else if (error.message) {
            errorMessage = error.message;
          }
          
          this.snackBar.open(errorMessage, 'Close', { duration: 7000 });
          this.isSubmitting = false;
        }
      });
    } else {
      this.userForm.markAllAsTouched();
      this.snackBar.open('Please fix the errors in the form', 'Close', { duration: 5000 });
    }
  }

  close(): void {
    this.dialogRef.close();
  }

  // Error getters
  get usernameError(): string {
    const control = this.userForm.get('username');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Username is required';
      if (control.errors['minlength']) return 'Username must be at least 3 characters';
    }
    return '';
  }

  get emailError(): string {
    const control = this.userForm.get('email');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Email is required';
      if (control.errors['email']) return 'Please enter a valid email address';
    }
    return '';
  }

  get firstNameError(): string {
    const control = this.userForm.get('firstName');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'First name is required';
      if (control.errors['minlength']) return 'First name must be at least 2 characters';
      if (control.errors['pattern']) return 'First name can only contain letters and spaces';
    }
    return '';
  }

  get lastNameError(): string {
    const control = this.userForm.get('lastName');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Last name is required';
      if (control.errors['minlength']) return 'Last name must be at least 2 characters';
      if (control.errors['pattern']) return 'Last name can only contain letters and spaces';
    }
    return '';
  }

  get passwordError(): string {
    const control = this.userForm.get('password');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Password is required';
      if (control.errors['minlength']) return 'Password must be at least 8 characters';
    }
    return '';
  }

  get roleError(): string {
    const control = this.userForm.get('roleId');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Role is required';
    }
    return '';
  }
}
