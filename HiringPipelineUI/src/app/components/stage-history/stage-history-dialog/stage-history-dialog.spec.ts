import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StageHistoryDialog } from './stage-history-dialog';

describe('StageHistoryDialog', () => {
  let component: StageHistoryDialog;
  let fixture: ComponentFixture<StageHistoryDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StageHistoryDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StageHistoryDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
