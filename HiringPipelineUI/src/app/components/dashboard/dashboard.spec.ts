import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChangeDetectorRef } from '@angular/core';
import { of } from 'rxjs';
import { DashboardComponent } from './dashboard';
import { ApplicationService } from '../../services/application.service';
import { CandidateService } from '../../services/candidate.service';
import { RequisitionService } from '../../services/requisition.service';
import { StageHistoryService } from '../../services/stage-history.service';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let applicationService: jasmine.SpyObj<ApplicationService>;
  let candidateService: jasmine.SpyObj<CandidateService>;
  let requisitionService: jasmine.SpyObj<RequisitionService>;
  let stageHistoryService: jasmine.SpyObj<StageHistoryService>;
  let changeDetectorRef: jasmine.SpyObj<ChangeDetectorRef>;

  const mockApplications = [
    {
      applicationId: 1,
      candidateId: 1,
      requisitionId: 1,
      currentStage: 'Applied',
      status: 'Active',
      createdAt: '2024-01-15T10:00:00Z',
      updatedAt: '2024-01-15T10:00:00Z'
    },
    {
      applicationId: 2,
      candidateId: 2,
      requisitionId: 1,
      currentStage: 'Hired',
      status: 'Active',
      createdAt: '2024-01-10T09:00:00Z',
      updatedAt: '2024-01-20T14:00:00Z'
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
    },
    {
      candidateId: 2,
      firstName: 'Jane',
      lastName: 'Smith',
      email: 'jane.smith@email.com',
      phone: '+1-555-0124',
      status: 'Active',
      createdAt: '2024-01-02T00:00:00Z',
      updatedAt: '2024-01-02T00:00:00Z'
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
    },
    {
      requisitionId: 2,
      title: 'Product Manager',
      department: 'Product',
      status: 'Active',
      createdAt: '2024-01-05T00:00:00Z',
      updatedAt: '2024-01-05T00:00:00Z'
    }
  ];

  const mockStageHistory = [
    {
      stageHistoryId: 1,
      applicationId: 1,
      fromStage: 'Applied',
      toStage: 'Screening',
      movedBy: 'Recruiter',
      movedAt: '2024-01-15T10:00:00Z',
      reason: 'Initial screening passed',
      notes: 'Strong technical background'
    },
    {
      stageHistoryId: 2,
      applicationId: 2,
      fromStage: 'Offer',
      toStage: 'Hired',
      movedBy: 'HR Manager',
      movedAt: '2024-01-20T14:00:00Z',
      reason: 'Offer accepted',
      notes: 'Candidate accepted the offer'
    }
  ];

  beforeEach(async () => {
    const applicationSpy = jasmine.createSpyObj('ApplicationService', ['getApplications']);
    const candidateSpy = jasmine.createSpyObj('CandidateService', ['getCandidates']);
    const requisitionSpy = jasmine.createSpyObj('RequisitionService', ['getRequisitions']);
    const stageHistorySpy = jasmine.createSpyObj('StageHistoryService', ['getStageHistory']);
    const changeDetectorSpy = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);

    applicationSpy.getApplications.and.returnValue(of(mockApplications));
    candidateSpy.getCandidates.and.returnValue(of(mockCandidates));
    requisitionSpy.getRequisitions.and.returnValue(of(mockRequisitions));
    stageHistorySpy.getStageHistory.and.returnValue(of(mockStageHistory));

    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        { provide: ApplicationService, useValue: applicationSpy },
        { provide: CandidateService, useValue: candidateSpy },
        { provide: RequisitionService, useValue: requisitionSpy },
        { provide: StageHistoryService, useValue: stageHistorySpy },
        { provide: ChangeDetectorRef, useValue: changeDetectorSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    applicationService = TestBed.inject(ApplicationService) as jasmine.SpyObj<ApplicationService>;
    candidateService = TestBed.inject(CandidateService) as jasmine.SpyObj<CandidateService>;
    requisitionService = TestBed.inject(RequisitionService) as jasmine.SpyObj<RequisitionService>;
    stageHistoryService = TestBed.inject(StageHistoryService) as jasmine.SpyObj<StageHistoryService>;
    changeDetectorRef = TestBed.inject(ChangeDetectorRef) as jasmine.SpyObj<ChangeDetectorRef>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load dashboard data on init', () => {
    component.ngOnInit();
    expect(applicationService.getApplications).toHaveBeenCalled();
    expect(candidateService.getCandidates).toHaveBeenCalled();
    expect(requisitionService.getRequisitions).toHaveBeenCalled();
    expect(stageHistoryService.getStageHistory).toHaveBeenCalled();
  });

  it('should calculate metrics correctly', () => {
    component.applications = mockApplications;
    component.candidates = mockCandidates;
    component.requisitions = mockRequisitions;

    expect(component.totalCandidates).toBe(2);
    expect(component.activeRequisitions).toBe(2);
    expect(component.totalHiredCount).toBe(1);
    expect(component.totalAppliedCount).toBe(2);
  });

  it('should calculate stage distribution correctly', () => {
    component.applications = mockApplications;
    const distribution = component.stageDistribution;
    
    expect(distribution.length).toBeGreaterThan(0);
    expect(distribution.find(item => item.stage === 'Applied')).toBeDefined();
    expect(distribution.find(item => item.stage === 'Hired')).toBeDefined();
  });

  it('should calculate hiring velocity correctly', () => {
    component.applications = mockApplications;
    const velocity = component.hiringVelocity;
    
    expect(velocity.length).toBeGreaterThan(0);
    expect(velocity[0].month).toBeDefined();
    expect(velocity[0].hired).toBeDefined();
    expect(velocity[0].applied).toBeDefined();
  });

  it('should load recent activity correctly', () => {
    component.ngOnInit();
    expect(component.recentHistory.length).toBeGreaterThan(0);
    expect(component.recentApplications.length).toBeGreaterThan(0);
    expect(component.recentRequisitions.length).toBeGreaterThan(0);
  });

  it('should get candidate name correctly', () => {
    component.candidates = mockCandidates;
    const candidateName = component.getCandidateName(1);
    expect(candidateName).toBe('John Doe');
  });

  it('should get requisition title correctly', () => {
    component.requisitions = mockRequisitions;
    const requisitionTitle = component.getRequisitionTitle(1);
    expect(requisitionTitle).toBe('Senior Developer');
  });

  it('should get time ago correctly', () => {
    const timeAgo = component.getTimeAgo('2024-01-15T10:00:00Z');
    expect(timeAgo).toContain('ago');
  });

  it('should get stage dot class correctly', () => {
    const dotClass = component.getStageDotClass('Applied');
    expect(dotClass).toContain('bg-');
  });

  it('should get stage badge class correctly', () => {
    const badgeClass = component.getStageBadgeClass('Applied');
    expect(badgeClass).toContain('bg-');
  });

  it('should handle loading state correctly', () => {
    expect(component.isLoading).toBeFalsy();
    component.ngOnInit();
    expect(component.isLoading).toBeTruthy();
  });

  it('should handle error state correctly', () => {
    applicationService.getApplications.and.returnValue(of([]));
    component.ngOnInit();
    expect(component.hasError).toBeFalsy();
  });

  it('should call change detector after data loading', () => {
    component.ngOnInit();
    expect(changeDetectorRef.detectChanges).toHaveBeenCalled();
  });

  it('should track by functions work correctly', () => {
    const stageItem = { stage: 'Applied', count: 1, percentage: 50, color: 'blue' };
    const monthItem = { month: 'Jan', hired: 1, applied: 2 };
    const historyItem = mockStageHistory[0];

    expect(component.trackByStage(0, stageItem)).toBe('Applied');
    expect(component.trackByMonth(0, monthItem)).toBe('Jan');
    expect(component.trackByHistory(0, historyItem)).toBe(1);
  });

  it('should track by application and requisition correctly', () => {
    const applicationItem = mockApplications[0];
    const requisitionItem = mockRequisitions[0];

    expect(component.trackByApplication(0, applicationItem)).toBe(1);
    expect(component.trackByRequisition(0, requisitionItem)).toBe(1);
  });

  it('should handle empty data gracefully', () => {
    applicationService.getApplications.and.returnValue(of([]));
    candidateService.getCandidates.and.returnValue(of([]));
    requisitionService.getRequisitions.and.returnValue(of([]));
    stageHistoryService.getStageHistory.and.returnValue(of([]));

    component.ngOnInit();
    
    expect(component.totalCandidates).toBe(0);
    expect(component.activeRequisitions).toBe(0);
    expect(component.totalHiredCount).toBe(0);
  });

  it('should provide fallback values for helper functions', () => {
    component.candidates = [];
    component.requisitions = [];
    
    const candidateName = component.getCandidateName(999);
    const requisitionTitle = component.getRequisitionTitle(999);
    const timeAgo = component.getTimeAgo('invalid-date');
    
    expect(candidateName).toBe('Unknown Candidate');
    expect(requisitionTitle).toBe('Unknown Position');
    expect(timeAgo).toBe('Unknown time');
  });
});
