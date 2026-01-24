import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MuscleGroupSelectorComponent } from './musclegroup-selector';

describe('MusclegroupSelector', () => {
  let component: MuscleGroupSelectorComponent;
  let fixture: ComponentFixture<MuscleGroupSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MuscleGroupSelectorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MuscleGroupSelectorComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
