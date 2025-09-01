import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Subject, takeUntil } from 'rxjs';
import { StageHistoryService } from '../../../services/stage-history.service';
import { CreateStageHistoryDto, HIRING_STAGES, STAGE_PROGRESSION_RULES } from '../../../models/stage-history.model';

interface DialogData {
  applicationId: number;
  currentStage?: string;
}

@Component({
  selector: 'app-stage-history-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    MatSnackBarModule,
    MatTooltipModule
  ],
  templateUrl: './stage-history-dialog.html',
  styleUrls: ['./stage-history-dialog.scss']
})
export class StageHistoryDialogComponent implements OnInit, OnDestroy {
  stageForm: FormGroup;
  isLoading = false;
  availableStages: string[] = [];
  currentStage: string = 'Applied';
  
  // Make constants available in template
  readonly HIRING_STAGES = HIRING_STAGES;
  readonly STAGE_PROGRESSION_RULES = STAGE_PROGRESSION_RULES;

  private destroy$ = new Subject<void>();

  constructor(
    public dialogRef: MatDialogRef<StageHistoryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private stageHistoryService: StageHistoryService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.stageForm = this.fb.group({
      fromStage: ['', [Validators.required]],
      toStage: ['', [Validators.required]],
      movedBy: ['', [
        Validators.required, 
        Validators.minLength(2),
        Validators.pattern(/^[a-zA-Z\s\-\.]+$/)
      ]],
      reason: ['', [Validators.maxLength(200)]],
      notes: ['', [Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    console.log('StageHistoryDialog initialized with data:', this.data);
    console.log('Raw currentStage:', this.data.currentStage);
    
    // Set the current stage if provided
    if (this.data.currentStage) {
      // Clean up the stage name to match expected format
      this.currentStage = this.cleanStageName(this.data.currentStage);
      this.stageForm.patchValue({ fromStage: this.currentStage });
      console.log('Setting fromStage to:', this.currentStage);
    } else {
      this.currentStage = 'Applied';
      this.stageForm.patchValue({ fromStage: this.currentStage });
      console.log('Setting fromStage to default:', this.currentStage);
    }
    
    // Initialize available stages based on current stage
    this.updateAvailableStages();
    
    // Ensure form has valid data
    this.ensureFormValidity();
    
    // Watch for changes in fromStage to update available toStages
    this.stageForm.get('fromStage')?.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        console.log('fromStage changed to:', this.stageForm.get('fromStage')?.value);
        this.updateAvailableStages();
      });

    // Watch for changes in toStage to validate stage progression
    this.stageForm.get('toStage')?.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.validateStageProgression();
      });

    // Log form state after initialization
    setTimeout(() => {
      console.log('Form state after initialization:');
      console.log('Form valid:', this.stageForm.valid);
      console.log('Form value:', this.stageForm.value);
      console.log('Form errors:', this.stageForm.errors);
      console.log('From stage:', this.stageForm.get('fromStage')?.value);
      console.log('To stage:', this.stageForm.get('toStage')?.value);
      console.log('Moved by:', this.stageForm.get('movedBy')?.value);
    }, 100);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private cleanStageName(stageName: string): string {
    console.log('Cleaning stage name:', stageName);
    
    if (!stageName) {
      console.warn('Stage name is null or undefined, defaulting to "Applied"');
      return 'Applied';
    }
    
    // Handle common malformed cases
    if (stageName.includes('assignment')) {
      return 'Applied'; // "assignment Applied" -> "Applied"
    }
    
    // Remove any extra characters and normalize the stage name
    const cleaned = stageName.trim().replace(/[^\w\s]/g, '');
    console.log('Cleaned stage name:', cleaned);
    
    // Try to find a matching stage from HIRING_STAGES
    const matchingStage = HIRING_STAGES.find(stage => 
      stage.toLowerCase() === cleaned.toLowerCase()
    );
    
    if (matchingStage) {
      console.log(`Exact match found: "${stageName}" -> "${matchingStage}"`);
      return matchingStage;
    }
    
    // If no exact match, try partial matching with better logic
    const partialMatch = HIRING_STAGES.find(stage => {
      const stageLower = stage.toLowerCase();
      const cleanedLower = cleaned.toLowerCase();
      
      // Check if cleaned contains the stage name or vice versa
      return stageLower.includes(cleanedLower) || 
             cleanedLower.includes(stageLower) ||
             // Handle cases like "assignment Applied" -> "Applied"
             (cleanedLower.includes('applied') && stageLower === 'applied') ||
             (cleanedLower.includes('interviewing') && stageLower === 'interviewing') ||
             (cleanedLower.includes('hired') && stageLower === 'hired');
    });
    
    if (partialMatch) {
      console.log(`Partial stage match found: "${stageName}" -> "${partialMatch}"`);
      return partialMatch;
    }
    
    // If still no match, try to extract meaningful parts
    const words = cleaned.split(' ').filter(word => word.length > 2);
    for (const word of words) {
      const wordMatch = HIRING_STAGES.find(stage => 
        stage.toLowerCase().includes(word.toLowerCase()) ||
        word.toLowerCase().includes(stage.toLowerCase())
      );
      if (wordMatch) {
        console.log(`Word match found: "${word}" -> "${wordMatch}" from "${stageName}"`);
        return wordMatch;
      }
    }
    
    // If still no match, return the first meaningful word or default to "Applied"
    const firstWord = words[0];
    if (firstWord) {
      console.warn(`No stage match found for: "${stageName}", using first word: "${firstWord}"`);
      return firstWord;
    }
    
    console.warn(`No stage match found for: "${stageName}", defaulting to "Applied"`);
    return 'Applied';
  }

  // Normalize stage names for backend compatibility
  private normalizeStageName(stageName: string): string {
    if (!stageName) return 'Applied';
    
    // Map common variations to standard names
    const stageMap: { [key: string]: string } = {
      'interviewing': 'Technical Interview',
      'phone screen': 'Phone Screen',
      'onsite': 'Onsite Interview',
      'reference': 'Reference Check',
      'offer': 'Offer',
      'hired': 'Hired',
      'rejected': 'Rejected',
      'withdrawn': 'Withdrawn',
      'applied': 'Applied'
    };
    
    const normalized = stageMap[stageName.toLowerCase()];
    if (normalized) {
      console.log(`Normalized stage name: "${stageName}" -> "${normalized}"`);
      return normalized;
    }
    
    return stageName;
  }

  updateAvailableStages(): void {
    const fromStage = this.stageForm.get('fromStage')?.value;
    console.log('updateAvailableStages called with fromStage:', fromStage);
    
    if (fromStage && STAGE_PROGRESSION_RULES[fromStage as keyof typeof STAGE_PROGRESSION_RULES]) {
      // Get allowed next stages from progression rules
      this.availableStages = [...STAGE_PROGRESSION_RULES[fromStage as keyof typeof STAGE_PROGRESSION_RULES]];
      console.log('Using progression rules for', fromStage, ':', this.availableStages);
    } else {
      // Fallback to all stages if no progression rules found
      this.availableStages = [...HIRING_STAGES];
      console.log('Using fallback stages:', this.availableStages);
    }
    
    // Ensure we always have available stages
    if (this.availableStages.length === 0) {
      console.warn(`No progression rules found for stage: ${fromStage}, using all stages as fallback`);
      this.availableStages = [...HIRING_STAGES];
    }
    
    // Reset toStage if current selection is no longer valid
    const currentToStage = this.stageForm.get('toStage')?.value;
    if (currentToStage && !this.availableStages.includes(currentToStage)) {
      this.stageForm.patchValue({ toStage: '' });
    }
    
    console.log('Final available stages:', this.availableStages);
  }

  private ensureFormValidity(): void {
    console.log('Ensuring form validity...');
    
    // Ensure fromStage is set
    const fromStage = this.stageForm.get('fromStage')?.value;
    if (!fromStage) {
      console.warn('fromStage is empty, setting to default');
      this.stageForm.patchValue({ fromStage: 'Applied' });
    }
    
    // Ensure availableStages has content
    if (this.availableStages.length === 0) {
      console.warn('availableStages is empty, setting to all stages');
      this.availableStages = [...HIRING_STAGES];
    }
    
    // Set a default toStage if none is selected and we have available stages
    const toStage = this.stageForm.get('toStage')?.value;
    if (!toStage && this.availableStages.length > 0) {
      const defaultToStage = this.availableStages[0];
      console.log(`Setting default toStage to: ${defaultToStage}`);
      this.stageForm.patchValue({ toStage: defaultToStage });
    }
    
    // Ensure movedBy has a default value if empty
    const movedBy = this.stageForm.get('movedBy')?.value;
    if (!movedBy || movedBy.trim() === '') {
      console.warn('movedBy is empty, setting to default');
      this.stageForm.patchValue({ movedBy: 'System User' });
    }
    
    console.log('Form validity check complete');
  }

  validateStageProgression(): void {
    const fromStage = this.stageForm.get('fromStage')?.value;
    const toStage = this.stageForm.get('toStage')?.value;
    
    if (fromStage && toStage && fromStage === toStage) {
      this.stageForm.get('toStage')?.setErrors({ sameStage: true });
    } else {
      this.stageForm.get('toStage')?.setErrors(null);
    }
  }

  onCancel(): void {
    if (this.stageForm.dirty) {
      if (confirm('You have unsaved changes. Are you sure you want to cancel?')) {
        this.dialogRef.close();
      }
    } else {
      this.dialogRef.close();
    }
  }

  onSave(): void {
    console.log('Form validity:', this.stageForm.valid);
    console.log('Form errors:', this.stageForm.errors);
    console.log('Form value:', this.stageForm.value);
    
    // Ensure form is properly populated before submission
    this.ensureFormValidity();
    
    // Validate required fields manually
    const formValue = this.stageForm.value;
    const validationErrors: string[] = [];
    
    if (!formValue.fromStage || formValue.fromStage.trim() === '') {
      validationErrors.push('Current Stage is required');
    }
    
    if (!formValue.toStage || formValue.toStage.trim() === '') {
      validationErrors.push('Next Stage is required');
    }
    
    if (!formValue.movedBy || formValue.movedBy.trim() === '') {
      validationErrors.push('Moved By is required');
    }
    
    if (validationErrors.length > 0) {
      this.showError(`Please fix the following errors: ${validationErrors.join(', ')}`);
      this.stageForm.markAllAsTouched();
      return;
    }
    
    if (this.stageForm.valid && !this.isLoading) {
      this.isLoading = true;
      
      const newHistory: CreateStageHistoryDto = {
        applicationId: this.data.applicationId,
        fromStage: this.normalizeStageName(formValue.fromStage.trim()),
        toStage: this.normalizeStageName(formValue.toStage.trim()),
        movedBy: formValue.movedBy.trim(),
        reason: formValue.reason?.trim() || undefined,
        notes: formValue.notes?.trim() || undefined
      };

      console.log('Sending stage history data:', newHistory);
      console.log('Application ID:', this.data.applicationId);
      console.log('From Stage:', formValue.fromStage);
      console.log('To Stage:', formValue.toStage);
      console.log('Moved By:', formValue.movedBy);

      this.stageHistoryService.addStageHistory(newHistory)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.isLoading = false;
            this.showSuccess('Stage history added successfully!');
            this.dialogRef.close(true);
          },
          error: (err: Error) => {
            console.error('Error adding stage history:', err);
            this.isLoading = false;
            this.showError(err.message || 'Failed to add stage history. Please try again.');
          }
        });
    } else {
      console.log('Form is invalid, marking as touched');
      this.stageForm.markAllAsTouched();
      
      // Show validation errors
      const errors = this.getFormErrors();
      if (errors.length > 0) {
        this.showError(`Please fix the following errors: ${errors.join(', ')}`);
      }
    }
  }

  private getFormErrors(): string[] {
    const errors: string[] = [];
    
    Object.keys(this.stageForm.controls).forEach(key => {
      const control = this.stageForm.get(key);
      if (control?.errors && control.touched) {
        if (control.errors['required']) {
          errors.push(`${this.getFieldLabel(key)} is required`);
        }
        if (control.errors['minlength']) {
          errors.push(`${this.getFieldLabel(key)} must be at least ${control.errors['minlength'].requiredLength} characters`);
        }
        if (control.errors['maxlength']) {
          errors.push(`${this.getFieldLabel(key)} must not exceed ${control.errors['maxlength'].requiredLength} characters`);
        }
        if (control.errors['sameStage']) {
          errors.push('From and To stages cannot be the same');
        }
      }
    });
    
    return errors;
  }

  getStageDescription(stage: string): string {
    const descriptions: { [key: string]: string } = {
      'Applied': 'Candidate has submitted their application',
      'Phone Screen': 'Initial phone conversation with recruiter',
      'Technical Interview': 'Technical skills assessment',
      'Onsite Interview': 'In-person interview with team members',
      'Reference Check': 'Verifying candidate references',
      'Offer': 'Job offer extended to candidate',
      'Hired': 'Candidate accepted and started',
      'Rejected': 'Application not selected',
      'Withdrawn': 'Candidate withdrew application'
    };
    return descriptions[stage] || stage;
  }

  getStageIcon(stage: string): string {
    const stageIcons: { [key: string]: string } = {
      'Applied': 'assignment',
      'Phone Screen': 'phone',
      'Technical Interview': 'code',
      'Onsite Interview': 'people',
      'Reference Check': 'verified_user',
      'Offer': 'card_giftcard',
      'Hired': 'check_circle',
      'Rejected': 'cancel',
      'Withdrawn': 'exit_to_app'
    };
    return stageIcons[stage] || 'fiber_manual_record';
  }

  getStageColor(stage: string): string {
    const stageColors: { [key: string]: string } = {
      'Applied': 'text-blue-600',
      'Phone Screen': 'text-indigo-600',
      'Technical Interview': 'text-purple-600',
      'Onsite Interview': 'text-pink-600',
      'Reference Check': 'text-yellow-600',
      'Offer': 'text-green-600',
      'Hired': 'text-emerald-600',
      'Rejected': 'text-red-600',
      'Withdrawn': 'text-gray-600'
    };
    return stageColors[stage] || 'text-gray-600';
  }

  getStageDotClass(stage: string): string {
    if (!stage) return 'bg-gray-400';
    const stageLower = stage.toLowerCase();
    if (stageLower.includes('interview')) return 'bg-purple-500';
    if (stageLower.includes('offer')) return 'bg-yellow-500';
    if (stageLower.includes('hired')) return 'bg-emerald-500';
    if (stageLower.includes('rejected')) return 'bg-red-500';
    if (stageLower.includes('withdrawn')) return 'bg-gray-500';
    return 'bg-blue-500'; // Default for 'Applied' or unknown
  }

  getFormError(controlName: string): string {
    const control = this.stageForm.get(controlName);
    if (control?.errors && control.touched) {
      if (control.errors['required']) {
        return `${this.getFieldLabel(controlName)} is required`;
      }
      if (control.errors['minlength']) {
        return `${this.getFieldLabel(controlName)} must be at least ${control.errors['minlength'].requiredLength} characters`;
      }
      if (control.errors['maxlength']) {
        return `${this.getFieldLabel(controlName)} must not exceed ${control.errors['maxlength'].requiredLength} characters`;
      }
      if (control.errors['pattern']) {
        if (controlName === 'movedBy') {
          return 'Moved By can only contain letters, spaces, hyphens, and periods';
        }
        return `${this.getFieldLabel(controlName)} contains invalid characters`;
      }
      if (control.errors['sameStage']) {
        return 'From and To stages cannot be the same';
      }
    }
    return '';
  }

  private getFieldLabel(controlName: string): string {
    const labels: { [key: string]: string } = {
      'fromStage': 'Current Stage',
      'toStage': 'Next Stage',
      'movedBy': 'Moved By',
      'reason': 'Reason',
      'notes': 'Notes'
    };
    return labels[controlName] || controlName;
  }

  private showSuccess(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['success-snackbar']
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['error-snackbar']
    });
  }
}
