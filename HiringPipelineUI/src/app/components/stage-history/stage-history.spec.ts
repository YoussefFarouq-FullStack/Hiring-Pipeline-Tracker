import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StageHistory } from './stage-history';

describe('StageHistory', () => {
  let component: StageHistory;
  let fixture: ComponentFixture<StageHistory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StageHistory]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StageHistory);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
