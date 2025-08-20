import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequisitionDialog } from './requisition-dialog';

describe('RequisitionDialog', () => {
  let component: RequisitionDialog;
  let fixture: ComponentFixture<RequisitionDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RequisitionDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RequisitionDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
