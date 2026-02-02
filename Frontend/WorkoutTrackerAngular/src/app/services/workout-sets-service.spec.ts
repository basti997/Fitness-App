import { TestBed } from '@angular/core/testing';

import { WorkoutSetsService } from './workout-sets-service';

describe('WorkoutSetsService', () => {
  let service: WorkoutSetsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WorkoutSetsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
