import { Component, Inject, OnInit, Injectable } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApplicationService } from '../../../services/application.service';
import { Application } from '../../../models/application.model';
import { Candidate } from '../../../models/candidate.model';
import { Requisition } from '../../../models/requisition.model';

interface DialogData {
  mode: 'create' | 'edit';
  application?: Application;
}

@Component({
  selector: 'app-application-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './application-dialog.html'
})
@Injectable()
export class ApplicationDialogComponent implements OnInit {
  applicationForm!: FormGroup;
  candidates: Candidate[] = [];
  requisitions: Requisition[] = [];
  isSubmitting = false;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private appService: ApplicationService,
    private dialogRef: MatDialogRef<ApplicationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private snackBar: MatSnackBar
  ) {
    this.isEditMode = data?.mode === 'edit';
    if (this.isEditMode && !data?.application) {
      console.error('Edit mode requested but no application data provided');
      this.dialogRef.close();
      return;
    }
    
    this.applicationForm = this.fb.group({
      applicationId: [this.isEditMode ? data?.application?.applicationId : null],
      candidateId: [this.isEditMode ? data?.application?.candidateId : '', [Validators.required]],
      requisitionId: [this.isEditMode ? data?.application?.requisitionId : '', [Validators.required]],
      currentStage: [this.isEditMode ? data?.application?.currentStage || 'Applied' : 'Applied', [Validators.required]],
      status: [this.isEditMode ? data?.application?.status || 'Active' : 'Active', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadCandidates();
    this.loadRequisitions();
  }

  loadCandidates(): void {
    this.appService.getCandidates().subscribe({
      next: (data: Candidate[]) => {
        this.candidates = data;
      },
      error: (error: Error) => {
        console.error('Error loading candidates:', error);
        this.showError('Failed to load candidates. Please try again.');
        this.candidates = [];
      }
    });
  }

  loadRequisitions(): void {
    this.appService.getRequisitions().subscribe({
      next: (data: Requisition[]) => {
        this.requisitions = data;
      },
      error: (error: Error) => {
        console.error('Error loading requisitions:', error);
        this.showError('Failed to load requisitions. Please try again.');
        this.requisitions = [];
      }
    });
  }

  save(): void {
    if (this.applicationForm.valid && !this.isSubmitting && this.candidates.length > 0 && this.requisitions.length > 0) {
      this.isSubmitting = true;
      
      try {
        const formData = this.applicationForm.value;
        
        if (!this.isEditMode) {
          delete formData.applicationId;
        }

        if (this.isEditMode && this.data?.application) {
          this.appService.updateApplication(this.data.application.applicationId, formData).subscribe({
            next: () => {
              this.dialogRef.close(formData);
            },
            error: (error: Error) => {
              console.error('Error updating application:', error);
              this.showError('Failed to update application. Please try again.');
              this.isSubmitting = false;
            }
          });
        } else {
          this.appService.createApplication(formData).subscribe({
            next: (res: Application) => {
              this.dialogRef.close(res);
            },
            error: (error: Error) => {
              console.error('Error creating application:', error);
              this.showError('Failed to create application. Please try again.');
              this.isSubmitting = false;
            }
          });
        }
      } catch (error) {
        console.error('Error processing form data:', error);
        this.showError('An error occurred while processing your request. Please try again.');
        this.isSubmitting = false;
      }
    } else {
      this.applicationForm.markAllAsTouched();
      if (this.applicationForm.errors) {
        this.showError('Please fix the errors in the form before submitting.');
      } else if (this.candidates.length === 0 || this.requisitions.length === 0) {
        this.showError('Unable to load required data. Please refresh the page and try again.');
      }
    }
  }

  close(): void {
    this.dialogRef.close();
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
  get candidateIdError(): string {
    const control = this.applicationForm.get('candidateId');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Candidate is required';
    }
    return '';
  }

  get requisitionIdError(): string {
    const control = this.applicationForm.get('requisitionId');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Requisition is required';
    }
    return '';
  }

  get currentStageError(): string {
    const control = this.applicationForm.get('currentStage');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Current stage is required';
    }
    return '';
  }

  get statusError(): string {
    const control = this.applicationForm.get('status');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Status is required';
    }
    return '';
  }
}
