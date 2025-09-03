import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of } from 'rxjs';
import { ApplicationsComponent } from './applications';
import { ApplicationService } from '../../services/application.service';

describe('ApplicationsComponent', () => {
  let component: ApplicationsComponent;
  let fixture: ComponentFixture<ApplicationsComponent>;
  let applicationService: jasmine.SpyObj<ApplicationService>;
  let dialog: jasmine.SpyObj<MatDialog>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;

  const mockApplications = [
    {
      applicationId: 1,
      candidateId: 1,
      requisitionId: 1,
      currentStage: 'Applied',
      status: 'Active',
      createdAt: '2024-01-15T10:00:00Z',
      updatedAt: '2024-01-15T10:00:00Z',
      candidate: {
        candidateId: 1,
        firstName: 'John',
        lastName: 'Doe',
        email: 'john.doe@email.com',
        phone: '+1-555-0123',
        status: 'Active',
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      },
      requisition: {
        requisitionId: 1,
        title: 'Senior Developer',
        department: 'Engineering',
        status: 'Active',
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      }
    },
    {
      applicationId: 2,
      candidateId: 2,
      requisitionId: 1,
      currentStage: 'Screening',
      status: 'Active',
      createdAt: '2024-01-10T09:00:00Z',
      updatedAt: '2024-01-15T10:00:00Z',
      candidate: {
        candidateId: 2,
        firstName: 'Jane',
        lastName: 'Smith',
        email: 'jane.smith@email.com',
        phone: '+1-555-0124',
        status: 'Active',
        createdAt: '2024-01-02T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      },
      requisition: {
        requisitionId: 1,
        title: 'Senior Developer',
        department: 'Engineering',
        status: 'Active',
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      }
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

  beforeEach(async () => {
    const applicationSpy = jasmine.createSpyObj('ApplicationService', ['getApplications', 'getCandidates', 'getRequisitions']);
    const dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    const snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    applicationSpy.getApplications.and.returnValue(of(mockApplications));
    applicationSpy.getCandidates.and.returnValue(of(mockCandidates));
    applicationSpy.getRequisitions.and.returnValue(of(mockRequisitions));

    await TestBed.configureTestingModule({
      imports: [ApplicationsComponent, FormsModule],
      providers: [
        { provide: ApplicationService, useValue: applicationSpy },
        { provide: MatDialog, useValue: dialogSpy },
        { provide: MatSnackBar, useValue: snackBarSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ApplicationsComponent);
    component = fixture.componentInstance;
    applicationService = TestBed.inject(ApplicationService) as jasmine.SpyObj<ApplicationService>;
    dialog = TestBed.inject(MatDialog) as jasmine.SpyObj<MatDialog>;
    snackBar = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load applications on init', () => {
    component.ngOnInit();
    expect(applicationService.getApplications).toHaveBeenCalled();
    expect(component.applications).toEqual(mockApplications);
    expect(component.filteredApplications).toEqual(mockApplications);
  });

  it('should load related data (candidates and requisitions)', () => {
    component.ngOnInit();
    expect(applicationService.getCandidates).toHaveBeenCalled();
    expect(applicationService.getRequisitions).toHaveBeenCalled();
    expect(component.candidates).toEqual(mockCandidates);
    expect(component.requisitions).toEqual(mockRequisitions);
  });

  it('should merge related data correctly', () => {
    component.applications = mockApplications;
    component.candidates = mockCandidates;
    component.requisitions = mockRequisitions;
    
    // Use the public method that triggers mergeRelatedData
    component.filterApplications();
    
    expect(component.applications[0].candidate).toBeDefined();
    expect(component.applications[0].requisition).toBeDefined();
    expect(component.applications[0].candidate?.firstName).toBe('John');
    expect(component.applications[0].requisition?.title).toBe('Senior Developer');
  });

  it('should filter applications based on search term', () => {
    component.applications = mockApplications;
    component.searchTerm = 'John';
    component.filterApplications();
    
    expect(component.filteredApplications.length).toBe(1);
    expect(component.filteredApplications[0].candidate?.firstName).toBe('John');
  });

  it('should filter by status correctly', () => {
    component.applications = mockApplications;
    component.selectedStat = 'Applied';
    component.filterApplications();
    
    expect(component.filteredApplications.length).toBe(1);
    expect(component.filteredApplications[0].currentStage).toBe('Applied');
  });

  it('should handle pagination correctly', () => {
    component.applications = Array(20).fill(mockApplications[0]);
    component.currentPage = 1;
    component.pageSize = 6;
    component.updatePagination();
    
    expect(component.totalPages).toBe(4);
    expect(component.paginatedApplications.length).toBe(6);
  });

  it('should navigate to next page', () => {
    component.currentPage = 1;
    component.totalPages = 3;
    component.nextPage();
    
    expect(component.currentPage).toBe(2);
  });

  it('should navigate to previous page', () => {
    component.currentPage = 2;
    component.previousPage();
    
    expect(component.currentPage).toBe(1);
  });

  it('should go to specific page', () => {
    component.totalPages = 5;
    component.goToPage(3);
    
    expect(component.currentPage).toBe(3);
  });

  it('should get page numbers correctly', () => {
    component.currentPage = 1;
    component.totalPages = 5;
    const pageNumbers = component.getPageNumbers();
    
    expect(pageNumbers).toEqual([1, 2, 3, 4, 5]);
  });

  it('should open application dialog', () => {
    component.openDialog();
    expect(dialog.open).toHaveBeenCalled();
  });

  it('should open stage history dialog', () => {
    component.viewStageHistory(1, 'Applied');
    expect(dialog.open).toHaveBeenCalled();
  });

  it('should handle loading state correctly', () => {
    expect(component.isLoading).toBeFalsy();
    component.loadApplications();
    expect(component.isLoading).toBeTruthy();
  });

  it('should handle error state correctly', () => {
    applicationService.getApplications.and.returnValue(of([]));
    component.loadApplications();
    expect(component.hasError).toBeFalsy();
  });

  it('should handle error state correctly', () => {
    applicationService.getApplications.and.returnValue(of([]));
    component.loadApplications();
    expect(component.hasError).toBeFalsy();
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

  it('should format date correctly', () => {
    const formattedDate = component.formatDate('2024-01-15T10:00:00Z');
    expect(formattedDate).toContain('Jan 15');
  });

  it('should get stage badge class correctly', () => {
    const badgeClass = component.getStageBadgeClass('Applied');
    expect(badgeClass).toContain('bg-');
  });

  it('should handle empty data gracefully', () => {
    applicationService.getApplications.and.returnValue(of([]));
    applicationService.getCandidates.and.returnValue(of([]));
    applicationService.getRequisitions.and.returnValue(of([]));

    component.ngOnInit();
    
    expect(component.applications.length).toBe(0);
    expect(component.candidates.length).toBe(0);
    expect(component.requisitions.length).toBe(0);
  });

  it('should provide fallback values for helper functions', () => {
    component.candidates = [];
    component.requisitions = [];
    
    const candidateName = component.getCandidateName(999);
    const requisitionTitle = component.getRequisitionTitle(999);
    
    expect(candidateName).toBe('Unknown Candidate');
    expect(requisitionTitle).toBe('Unknown Position');
  });
});
