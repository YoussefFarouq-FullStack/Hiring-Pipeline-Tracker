import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of, throwError } from 'rxjs';
import { StageHistoryDialogComponent } from './stage-history-dialog';
import { StageHistoryService } from '../../../services/stage-history.service';

describe('StageHistoryDialogComponent', () => {
  let component: StageHistoryDialogComponent;
  let fixture: ComponentFixture<StageHistoryDialogComponent>;
  let stageHistoryService: jasmine.SpyObj<StageHistoryService>;
  let dialogRef: jasmine.SpyObj<MatDialogRef<StageHistoryDialogComponent>>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;

  const mockDialogData = {
    applicationId: 1,
    currentStage: 'Applied'
  };

  const mockStageHistory = {
    stageHistoryId: 1,
    applicationId: 1,
    fromStage: 'Applied',
    toStage: 'Screening',
    movedBy: 'Recruiter',
    movedAt: '2024-01-15T10:00:00Z',
    reason: 'Initial screening passed',
    notes: 'Strong technical background'
  };

  beforeEach(async () => {
    const stageHistorySpy = jasmine.createSpyObj('StageHistoryService', ['addStageHistory', 'updateStageHistory']);
    const dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);
    const snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    stageHistorySpy.addStageHistory.and.returnValue(of(mockStageHistory));
    stageHistorySpy.updateStageHistory.and.returnValue(of(mockStageHistory));

    await TestBed.configureTestingModule({
      imports: [StageHistoryDialogComponent, ReactiveFormsModule],
      providers: [
        { provide: StageHistoryService, useValue: stageHistorySpy },
        { provide: MatDialogRef, useValue: dialogRefSpy },
        { provide: MAT_DIALOG_DATA, useValue: mockDialogData },
        { provide: MatSnackBar, useValue: snackBarSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(StageHistoryDialogComponent);
    component = fixture.componentInstance;
    stageHistoryService = TestBed.inject(StageHistoryService) as jasmine.SpyObj<StageHistoryService>;
    dialogRef = TestBed.inject(MatDialogRef) as jasmine.SpyObj<MatDialogRef<StageHistoryDialogComponent>>;
    snackBar = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with correct data', () => {
    expect(component.data.applicationId).toBe(1);
    expect(component.data.currentStage).toBe('Applied');
  });

  it('should set up form with correct initial values', () => {
    component.ngOnInit();
    expect(component.stageForm.get('fromStage')?.value).toBe('Applied');
    expect(component.stageForm.get('applicationId')?.value).toBe(1);
  });

  it('should update available stages when fromStage changes', () => {
    component.ngOnInit();
    component.stageForm.patchValue({ fromStage: 'Applied' });
    component.updateAvailableStages();
    
    expect(component.availableStages).toContain('Phone Screen');
    expect(component.availableStages).toContain('Rejected');
    expect(component.availableStages).toContain('Withdrawn');
  });

  it('should validate form correctly', () => {
    component.ngOnInit();
    
    // Test required fields
    expect(component.stageForm.valid).toBeFalsy();
    
    // Fill required fields
    component.stageForm.patchValue({
      fromStage: 'Applied',
      toStage: 'Phone Screen',
      movedBy: 'Recruiter'
    });
    
    expect(component.stageForm.valid).toBeTruthy();
  });

  it('should validate movedBy field format', () => {
    component.ngOnInit();
    
    // Test invalid format
    component.stageForm.patchValue({ movedBy: 'Recruiter123' });
    expect(component.stageForm.get('movedBy')?.valid).toBeFalsy();
    
    // Test valid format
    component.stageForm.patchValue({ movedBy: 'John Doe' });
    expect(component.stageForm.get('movedBy')?.valid).toBeTruthy();
  });

  it('should validate field lengths', () => {
    component.ngOnInit();
    
    // Test reason length
    component.stageForm.patchValue({ reason: 'a'.repeat(201) });
    expect(component.stageForm.get('reason')?.valid).toBeFalsy();
    
    // Test notes length
    component.stageForm.patchValue({ notes: 'a'.repeat(501) });
    expect(component.stageForm.get('notes')?.valid).toBeFalsy();
  });

  it('should create stage history on save', () => {
    component.ngOnInit();
    component.stageForm.patchValue({
      fromStage: 'Applied',
      toStage: 'Phone Screen',
      movedBy: 'Recruiter',
      reason: 'Initial screening passed',
      notes: 'Strong technical background'
    });

    component.onSave();
    
    expect(stageHistoryService.addStageHistory).toHaveBeenCalledWith({
      applicationId: 1,
      fromStage: 'Applied',
      toStage: 'Phone Screen',
      movedBy: 'Recruiter',
      reason: 'Initial screening passed',
      notes: 'Strong technical background'
    });
  });

  it('should handle save success', () => {
    component.ngOnInit();
    component.stageForm.patchValue({
      fromStage: 'Applied',
      toStage: 'Phone Screen',
      movedBy: 'Recruiter'
    });

    component.onSave();
    
    expect(snackBar.open).toHaveBeenCalledWith(
      'Stage history created successfully!',
      'Close',
      jasmine.any(Object)
    );
    expect(dialogRef.close).toHaveBeenCalledWith(true);
  });

  it('should handle save error', () => {
    stageHistoryService.addStageHistory.and.returnValue(throwError(() => new Error('Test error')));
    
    component.ngOnInit();
    component.stageForm.patchValue({
      fromStage: 'Applied',
      toStage: 'Phone Screen',
      movedBy: 'Recruiter'
    });

    component.onSave();
    
    expect(snackBar.open).toHaveBeenCalledWith(
      'Failed to create stage history. Please try again.',
      'Close',
      jasmine.any(Object)
    );
  });

  it('should close dialog on cancel', () => {
    component.onCancel();
    expect(dialogRef.close).toHaveBeenCalled();
  });

  it('should get form error messages correctly', () => {
    component.ngOnInit();
    
    const fromStageControl = component.stageForm.get('fromStage');
    fromStageControl?.markAsTouched();
    fromStageControl?.setErrors({ required: true });
    
    const errorMessage = component.getFormError('fromStage');
    expect(errorMessage).toBe('Current stage is required');
  });

  it('should get stage description correctly', () => {
    const description = component.getStageDescription('Applied');
    expect(description).toContain('Candidate has submitted their application');
  });

  it('should handle loading state during save', () => {
    component.ngOnInit();
    component.stageForm.patchValue({
      fromStage: 'Applied',
      toStage: 'Phone Screen',
      movedBy: 'Recruiter'
    });

    component.onSave();
    expect(component.isLoading).toBeTruthy();
  });

  it('should disable save button when form is invalid', () => {
    component.ngOnInit();
    expect(component.stageForm.invalid).toBeTruthy();
  });

  it('should enable save button when form is valid', () => {
    component.ngOnInit();
    component.stageForm.patchValue({
      fromStage: 'Applied',
      toStage: 'Phone Screen',
      movedBy: 'Recruiter'
    });
    expect(component.stageForm.valid).toBeTruthy();
  });

  it('should handle stage progression rules correctly', () => {
    component.ngOnInit();
    
    // Test valid progression
    component.stageForm.patchValue({ fromStage: 'Applied' });
    component.updateAvailableStages();
    expect(component.availableStages).toContain('Phone Screen');
    
    // Test invalid progression
    expect(component.availableStages).not.toContain('Hired');
  });

  it('should ensure form validity on init', () => {
    component.ngOnInit();
    expect(component.stageForm.get('fromStage')?.value).toBe('Applied');
    expect(component.stageForm.get('applicationId')?.value).toBe(1);
  });
});
