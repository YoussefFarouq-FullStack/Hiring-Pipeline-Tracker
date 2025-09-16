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
  templateUrl: './user-dialog.html',
  styleUrls: ['./user-dialog.scss']
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
