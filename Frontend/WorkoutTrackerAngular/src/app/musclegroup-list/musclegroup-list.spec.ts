import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MusclegroupList } from './musclegroup-list';

describe('MusclegroupList', () => {
  let component: MusclegroupList;
  let fixture: ComponentFixture<MusclegroupList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MusclegroupList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MusclegroupList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
