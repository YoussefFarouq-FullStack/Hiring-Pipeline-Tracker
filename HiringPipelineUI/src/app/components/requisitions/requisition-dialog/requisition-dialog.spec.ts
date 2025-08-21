import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequisitionDialogComponent } from './requisition-dialog';

describe('RequisitionDialog', () => {
  let component: RequisitionDialogComponent;
  let fixture: ComponentFixture<RequisitionDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RequisitionDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RequisitionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
