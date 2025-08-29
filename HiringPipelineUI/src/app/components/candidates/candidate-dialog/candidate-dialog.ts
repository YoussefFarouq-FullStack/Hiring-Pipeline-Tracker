import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-candidate-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './candidate-dialog.html'
})
export class CandidateDialogComponent {
  form: FormGroup;
  isEditMode: boolean;
  isSubmitting = false;



  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CandidateDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackBar: MatSnackBar
  ) {
    this.isEditMode = data?.mode === 'edit';

    this.form = this.fb.group({
      candidateId: [this.isEditMode ? data?.candidate?.candidateId : null],
      firstName: [this.isEditMode ? data?.candidate?.firstName : '', [Validators.required, Validators.minLength(2)]],
      lastName: [this.isEditMode ? data?.candidate?.lastName : '', [Validators.required, Validators.minLength(2)]],
      email: [this.isEditMode ? data?.candidate?.email : '', [Validators.required, Validators.email]],
      phone: [this.isEditMode ? data?.candidate?.phone : '', [Validators.required, Validators.pattern(/^[\+]?[0-9]{8,15}$/)]],
      status: [this.isEditMode ? (data?.candidate?.status || 'Applied') : '', Validators.required]
    });
  }



  save() {
    if (this.form.valid && !this.isSubmitting) {
      this.isSubmitting = true;

      try {
        const formData = this.form.value;
        console.log('Form data before processing:', formData);

        // For new candidates, don't send candidateId (let backend assign it)
        if (!this.isEditMode) {
          delete formData.candidateId;
        }

        // Ensure status is always set
        if (!formData.status || formData.status === '') {
          console.log('Status is required but not provided');
          this.showError('Please select a status for the candidate.');
          this.isSubmitting = false;
          return;
        }



        // Validate email format
        if (formData.email && !this.isValidEmail(formData.email)) {
          this.showError('Please enter a valid email address.');
          this.isSubmitting = false;
          return;
        }

        // Validate phone format if provided
        if (formData.phone && !this.isValidPhone(formData.phone)) {
          this.showError('Please enter a valid phone number.');
          this.isSubmitting = false;
          return;
        }

        console.log('Final form data being sent:', formData);
        this.dialogRef.close(formData);
      } catch (error) {
        console.error('Error processing form data:', error);
        this.showError('An error occurred while processing your request. Please try again.');
        this.isSubmitting = false;
      }
    } else {
      // Mark all fields as touched to show validation errors
      this.form.markAllAsTouched();
      
      if (this.form.errors) {
        this.showError('Please fix the errors in the form before submitting.');
      }
    }
  }

  close() {
    this.dialogRef.close();
  }

  private isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  private isValidPhone(phone: string): boolean {
    const phoneRegex = /^[\+]?[1-9][\d]{0,15}$/;
    return phoneRegex.test(phone);
  }

  private showError(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['error-snackbar']
    });
  }

  // Getter methods for form validation
  get firstNameError(): string {
    const control = this.form.get('firstName');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'First name is required';
      if (control.errors['minlength']) return 'First name must be at least 2 characters';
    }
    return '';
  }

  get phoneError(): string {
    const control = this.form.get('phone');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Phone number is required';
      if (control.errors['pattern']) return 'Please enter a valid phone number';
    }
    return '';
  }

  get lastNameError(): string {
    const control = this.form.get('lastName');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Last name is required';
      if (control.errors['minlength']) return 'Last name must be at least 2 characters';
    }
    return '';
  }

  get emailError(): string {
    const control = this.form.get('email');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Email is required';
      if (control.errors['email']) return 'Please enter a valid email address';
    }
    return '';
  }



  get statusError(): string {
    const control = this.form.get('status');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Status is required';
    }
    return '';
  }
}
