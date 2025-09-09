import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { FileUploadService, FileUploadResult } from '../../../services/file-upload.service';
import { switchMap, catchError } from 'rxjs/operators';
import { of, Observable } from 'rxjs';

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
  templateUrl: './candidate-dialog.html',
  styleUrls: ['./candidate-dialog.scss']
})
export class CandidateDialogComponent {
  form: FormGroup;
  isEditMode: boolean;
  isSubmitting = false;
  selectedFile: File | null = null;



  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CandidateDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackBar: MatSnackBar,
    private fileUploadService: FileUploadService
  ) {
    this.isEditMode = data?.mode === 'edit';

    this.form = this.fb.group({
      candidateId: [this.isEditMode ? data?.candidate?.candidateId : null],
      firstName: [this.isEditMode ? data?.candidate?.firstName : '', [Validators.required, Validators.minLength(2)]],
      lastName: [this.isEditMode ? data?.candidate?.lastName : '', [Validators.required, Validators.minLength(2)]],
      email: [this.isEditMode ? data?.candidate?.email : '', [Validators.required, Validators.email]],
      phone: [this.isEditMode ? data?.candidate?.phone : '', [Validators.pattern(/^[\+]?[0-9]{8,15}$/)]],
      resumeFileName: [this.isEditMode ? data?.candidate?.resumeFileName : ''],
      resumeFilePath: [this.isEditMode ? data?.candidate?.resumeFilePath : ''],
      description: [this.isEditMode ? data?.candidate?.description : '', [Validators.maxLength(2000)]],
      skills: [this.isEditMode ? data?.candidate?.skills : '', [Validators.required, Validators.maxLength(1000)]],
      status: [this.isEditMode ? (data?.candidate?.status || 'Applied') : '', Validators.required]
    });
  }



  save() {
    if (this.form.valid && !this.isSubmitting) {
      this.isSubmitting = true;

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

      // Handle file upload if a file is selected
      const uploadObservable: Observable<FileUploadResult | null> = this.selectedFile 
        ? this.fileUploadService.uploadResume(this.selectedFile)
        : of(null);

      uploadObservable.pipe(
        switchMap((uploadResult: FileUploadResult | null) => {
          if (uploadResult) {
            formData.resumeFileName = uploadResult.fileName;
            formData.resumeFilePath = uploadResult.filePath;
          }
          return of(formData);
        }),
        catchError((error: any) => {
          console.error('File upload failed:', error);
          this.showError('Failed to upload resume file. Please try again.');
          this.isSubmitting = false;
          return of(null);
        })
      ).subscribe((result: any) => {
        if (result) {
          console.log('Final form data being sent:', result);
          this.dialogRef.close(result);
        }
      });
    } else {
      // Mark all fields as touched to show validation errors
      this.form.markAllAsTouched();
      
      if (this.form.errors) {
        this.showError('Please fix the errors in the form before submitting.');
      }
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      // Validate file type
      const allowedTypes = ['application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
      if (!allowedTypes.includes(file.type)) {
        this.showError('Please select a valid file type (PDF, DOC, or DOCX).');
        return;
      }
      
      // Validate file size (5MB limit)
      const maxSize = 5 * 1024 * 1024; // 5MB
      if (file.size > maxSize) {
        this.showError('File size must be less than 5MB.');
        return;
      }
      
      this.selectedFile = file;
      this.form.patchValue({
        resumeFileName: file.name,
        resumeFilePath: file.name // For now, we'll use the filename as path
      });
    }
  }

  removeFile() {
    this.selectedFile = null;
    this.form.patchValue({
      resumeFileName: '',
      resumeFilePath: ''
    });
    // Reset file input
    const fileInput = document.getElementById('resume-upload') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  private isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  private isValidPhone(phone: string): boolean {
    const phoneRegex = /^[\+]?[1-9][\d]{0,15}$/;
    return phoneRegex.test(phone);
  }

  close() {
    this.dialogRef.close();
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

  get resumeError(): string {
    const control = this.form.get('resumeFileName');
    if (control?.errors && control.touched) {
      if (control.errors['maxlength']) return 'Resume filename cannot exceed 255 characters';
    }
    return '';
  }

  get descriptionError(): string {
    const control = this.form.get('description');
    if (control?.errors && control.touched) {
      if (control.errors['maxlength']) return 'Description cannot exceed 2000 characters';
    }
    return '';
  }

  get skillsError(): string {
    const control = this.form.get('skills');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Skills are required';
      if (control.errors['maxlength']) return 'Skills cannot exceed 1000 characters';
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
