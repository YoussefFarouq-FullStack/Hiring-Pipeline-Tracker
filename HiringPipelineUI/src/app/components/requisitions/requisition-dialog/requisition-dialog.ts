import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Requisition, CreateRequisitionRequest, UpdateRequisitionRequest, EMPLOYMENT_TYPES, PRIORITIES, EXPERIENCE_LEVELS, JOB_LEVELS, STATUSES } from '../../../models/requisition.model';

interface DialogData {
  mode: 'create' | 'edit';
  requisition?: Requisition;
}

@Component({
  selector: 'app-requisition-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule, MatCheckboxModule, MatSnackBarModule],
  templateUrl: './requisition-dialog.html',
  styleUrls: ['./requisition-dialog.scss']
})
export class RequisitionDialogComponent {
  requisitionForm!: FormGroup;
  isSubmitting = false;
  isEditMode = false;

  // Constants for dropdowns
  employmentTypes = EMPLOYMENT_TYPES;
  priorities = PRIORITIES;
  experienceLevels = EXPERIENCE_LEVELS;
  jobLevels = JOB_LEVELS;
  statuses = STATUSES;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<RequisitionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private snackBar: MatSnackBar
  ) {
    this.isEditMode = data?.mode === 'edit';
    
    // Ensure we have valid data for edit mode
    if (this.isEditMode && !data?.requisition) {
      console.error('Edit mode requested but no requisition data provided');
      this.dialogRef.close();
      return;
    }
    
    this.requisitionForm = this.fb.group({
      requisitionId: [this.isEditMode ? data?.requisition?.requisitionId : null],
      title: [this.isEditMode ? data?.requisition?.title || '' : '', [Validators.required, Validators.minLength(3), Validators.maxLength(200)]],
      description: [this.isEditMode ? data?.requisition?.description || '' : '', [Validators.maxLength(2000)]],
      department: [this.isEditMode ? data?.requisition?.department || '' : '', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      location: [this.isEditMode ? data?.requisition?.location || '' : '', [Validators.maxLength(100)]],
      employmentType: [this.isEditMode ? data?.requisition?.employmentType || '' : ''],
      salary: [this.isEditMode ? data?.requisition?.salary || '' : '', [Validators.maxLength(100)]],
      isDraft: [this.isEditMode ? data?.requisition?.isDraft || false : true],
      priority: [this.isEditMode ? data?.requisition?.priority || 'Medium' : 'Medium', [Validators.required]],
      requiredSkills: [this.isEditMode ? data?.requisition?.requiredSkills || '' : '', [Validators.maxLength(1000)]],
      experienceLevel: [this.isEditMode ? data?.requisition?.experienceLevel || '' : ''],
      jobLevel: [this.isEditMode ? data?.requisition?.jobLevel || '' : ''],
      status: [this.isEditMode ? data?.requisition?.status || 'Open' : 'Open', [Validators.required]]
    });
  }

  save(): void {
    if (this.requisitionForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;
      
      try {
        const formData = this.requisitionForm.value;
        
        // For new requisitions, don't send requisitionId (let backend assign it)
        if (!this.isEditMode) {
          delete formData.requisitionId;
        }
        
        // Validate title format
        if (formData.title && !this.isValidTitle(formData.title)) {
          this.showError('Please enter a valid job title.');
          this.isSubmitting = false;
          return;
        }

        // Validate department format
        if (formData.department && !this.isValidDepartment(formData.department)) {
          this.showError('Please enter a valid department name.');
          this.isSubmitting = false;
          return;
        }

        this.dialogRef.close(formData);
      } catch (error) {
        console.error('Error processing form data:', error);
        this.showError('An error occurred while processing your request. Please try again.');
        this.isSubmitting = false;
      }
    } else {
      // Mark all fields as touched to show validation errors
      this.requisitionForm.markAllAsTouched();
      
      if (this.requisitionForm.errors) {
        this.showError('Please fix the errors in the form before submitting.');
      }
    }
  }

  cancel(): void {
    this.dialogRef.close();
  }

  private isValidTitle(title: string): boolean {
    // Title should contain only letters, numbers, spaces, and common punctuation
    const titleRegex = /^[a-zA-Z0-9\s\-_&.,()]+$/;
    return titleRegex.test(title);
  }

  private isValidDepartment(department: string): boolean {
    // Department should contain only letters, spaces, and common punctuation
    const deptRegex = /^[a-zA-Z\s\-_&.]+$/;
    return deptRegex.test(department);
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['error-snackbar']
    });
  }

  // Getter methods for form validation
  get titleError(): string {
    const control = this.requisitionForm.get('title');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Job title is required';
      if (control.errors['minlength']) return 'Job title must be at least 3 characters';
      if (control.errors['maxlength']) return 'Job title cannot exceed 200 characters';
    }
    return '';
  }

  get descriptionError(): string {
    const control = this.requisitionForm.get('description');
    if (control?.errors && control.touched) {
      if (control.errors['maxlength']) return 'Description cannot exceed 2000 characters';
    }
    return '';
  }

  get departmentError(): string {
    const control = this.requisitionForm.get('department');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Department is required';
      if (control.errors['minlength']) return 'Department must be at least 2 characters';
      if (control.errors['maxlength']) return 'Department cannot exceed 100 characters';
    }
    return '';
  }

  get locationError(): string {
    const control = this.requisitionForm.get('location');
    if (control?.errors && control.touched) {
      if (control.errors['maxlength']) return 'Location cannot exceed 100 characters';
    }
    return '';
  }

  get salaryError(): string {
    const control = this.requisitionForm.get('salary');
    if (control?.errors && control.touched) {
      if (control.errors['maxlength']) return 'Salary cannot exceed 100 characters';
    }
    return '';
  }

  get priorityError(): string {
    const control = this.requisitionForm.get('priority');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Priority is required';
    }
    return '';
  }

  get requiredSkillsError(): string {
    const control = this.requisitionForm.get('requiredSkills');
    if (control?.errors && control.touched) {
      if (control.errors['maxlength']) return 'Required skills cannot exceed 1000 characters';
    }
    return '';
  }

  get statusError(): string {
    const control = this.requisitionForm.get('status');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Status is required';
    }
    return '';
  }
}
