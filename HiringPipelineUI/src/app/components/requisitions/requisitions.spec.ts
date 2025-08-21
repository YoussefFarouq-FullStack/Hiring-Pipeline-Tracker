import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { RequisitionsComponent } from './requisitions';
import { RequisitionService } from '../../services/requisition.service';

describe('RequisitionsComponent', () => {
  let component: RequisitionsComponent;
  let fixture: ComponentFixture<RequisitionsComponent>;

  const requisitionServiceMock: Partial<RequisitionService> = {
    getRequisitions: () => of([]),
    deleteRequisition: () => of(void 0),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RequisitionsComponent],
      providers: [
        { provide: RequisitionService, useValue: requisitionServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(RequisitionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
