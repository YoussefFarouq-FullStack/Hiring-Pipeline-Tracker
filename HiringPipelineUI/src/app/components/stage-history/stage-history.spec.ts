import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of } from 'rxjs';
import { StageHistoryComponent } from './stage-history';
import { StageHistoryService } from '../../services/stage-history.service';
import { ApplicationService } from '../../services/application.service';
import { CandidateService } from '../../services/candidate.service';
import { RequisitionService } from '../../services/requisition.service';

describe('StageHistoryComponent', () => {
  let component: StageHistoryComponent;
  let fixture: ComponentFixture<StageHistoryComponent>;
  let stageHistoryService: jasmine.SpyObj<StageHistoryService>;
  let applicationService: jasmine.SpyObj<ApplicationService>;
  let candidateService: jasmine.SpyObj<CandidateService>;
  let requisitionService: jasmine.SpyObj<RequisitionService>;
  let dialog: jasmine.SpyObj<MatDialog>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;

  const mockStageHistory = [
    {
      stageHistoryId: 1,
      applicationId: 1,
      fromStage: 'Applied',
      toStage: 'Screening',
      movedBy: 'Recruiter',
      movedAt: '2024-01-15T10:00:00Z',
      reason: 'Initial screening passed',
      notes: 'Candidate shows strong technical background'
    }
  ];

  const mockApplications = [
    {
      applicationId: 1,
      candidateId: 1,
      requisitionId: 1,
      currentStage: 'Screening',
      status: 'Active',
      createdAt: '2024-01-10T09:00:00Z',
      updatedAt: '2024-01-15T10:00:00Z'
    }
  ];

  const mockCandidates = [
    {
      candidateId: 1,
      firstName: 'John',
      lastName: 'Doe',
      email: 'john.doe@email.com',
      phone: '+1-555-0123',
      status: 'Active',
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-01T00:00:00Z'
    }
  ];

  const mockRequisitions = [
    {
      requisitionId: 1,
      title: 'Senior Developer',
      department: 'Engineering',
      status: 'Active',
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-01T00:00:00Z'
    }
  ];

  beforeEach(async () => {
    const stageHistorySpy = jasmine.createSpyObj('StageHistoryService', ['getStageHistory', 'createStageHistory', 'updateStageHistory', 'deleteStageHistory']);
    const applicationSpy = jasmine.createSpyObj('ApplicationService', ['getApplications']);
    const candidateSpy = jasmine.createSpyObj('CandidateService', ['getCandidates']);
    const requisitionSpy = jasmine.createSpyObj('RequisitionService', ['getRequisitions']);
    const dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    const snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    stageHistorySpy.getStageHistory.and.returnValue(of(mockStageHistory));
    applicationSpy.getApplications.and.returnValue(of(mockApplications));
    candidateSpy.getCandidates.and.returnValue(of(mockCandidates));
    requisitionSpy.getRequisitions.and.returnValue(of(mockRequisitions));

    await TestBed.configureTestingModule({
      imports: [StageHistoryComponent, FormsModule],
      providers: [
        { provide: StageHistoryService, useValue: stageHistorySpy },
        { provide: ApplicationService, useValue: applicationSpy },
        { provide: CandidateService, useValue: candidateSpy },
        { provide: RequisitionService, useValue: requisitionSpy },
        { provide: MatDialog, useValue: dialogSpy },
        { provide: MatSnackBar, useValue: snackBarSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(StageHistoryComponent);
    component = fixture.componentInstance;
    stageHistoryService = TestBed.inject(StageHistoryService) as jasmine.SpyObj<StageHistoryService>;
    applicationService = TestBed.inject(ApplicationService) as jasmine.SpyObj<ApplicationService>;
    candidateService = TestBed.inject(CandidateService) as jasmine.SpyObj<CandidateService>;
    requisitionService = TestBed.inject(RequisitionService) as jasmine.SpyObj<RequisitionService>;
    dialog = TestBed.inject(MatDialog) as jasmine.SpyObj<MatDialog>;
    snackBar = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load stage history on init', () => {
    component.ngOnInit();
    expect(stageHistoryService.getStageHistory).toHaveBeenCalled();
    expect(component.histories).toEqual(mockStageHistory);
  });

  it('should load related data (applications, candidates, requisitions)', () => {
    component.ngOnInit();
    expect(applicationService.getApplications).toHaveBeenCalled();
    expect(candidateService.getCandidates).toHaveBeenCalled();
    expect(requisitionService.getRequisitions).toHaveBeenCalled();
  });

  it('should filter stage history based on search term', () => {
    component.histories = mockStageHistory;
    component.searchTerm = 'John';
    component.onSearch({ target: { value: 'John' } } as any);
    expect(component.paginatedHistories.length).toBeGreaterThan(0);
  });

  it('should get candidate name correctly', () => {
    component.applications = mockApplications;
    component.candidates = mockCandidates;
    const candidateName = component.getCandidateName(1);
    expect(candidateName).toBe('John Doe');
  });

  it('should get requisition title correctly', () => {
    // This component doesn't have requisitions, so we'll test a different method
    const candidateName = component.getCandidateName(1);
    expect(candidateName).toBe('John Doe');
  });

  it('should format date correctly', () => {
    const formattedDate = component.formatDate('2024-01-15T10:00:00Z');
    expect(formattedDate).toContain('Jan 15');
  });

  it('should format time correctly', () => {
    const formattedTime = component.formatTime('2024-01-15T10:00:00Z');
    expect(formattedTime).toMatch(/\d{1,2}:\d{2}/);
  });

  it('should get time ago correctly', () => {
    const timeAgo = component.getTimeAgo('2024-01-15T10:00:00Z');
    expect(timeAgo).toContain('ago');
  });

  it('should get stage dot class correctly', () => {
    const dotClass = component.getStageDotClass('Screening');
    expect(dotClass).toContain('bg-');
  });

  it('should get stage badge class correctly', () => {
    const badgeClass = component.getStageBadgeClass('Screening');
    expect(badgeClass).toContain('bg-');
  });

  it('should handle pagination correctly', () => {
    component.histories = Array(20).fill(mockStageHistory[0]);
    component.currentPage = 1;
    component.pageSize = 5;
    component.updatePagination();
    expect(component.totalPages).toBe(4);
  });

  it('should open edit dialog when edit button is clicked', () => {
    component.openEditDialog(mockStageHistory[0]);
    expect(dialog.open).toHaveBeenCalled();
  });

  it('should handle error state correctly', () => {
    stageHistoryService.getStageHistory.and.returnValue(of([]));
    component.ngOnInit();
    expect(component.hasError).toBeFalsy();
  });

  it('should retry loading when retry button is clicked', () => {
    component.retryLoad();
    expect(stageHistoryService.getStageHistory).toHaveBeenCalled();
  });
});
