import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkoutsetTracker } from './workoutset-tracker';

describe('WorkoutsetTracker', () => {
  let component: WorkoutsetTracker;
  let fixture: ComponentFixture<WorkoutsetTracker>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkoutsetTracker]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkoutsetTracker);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
