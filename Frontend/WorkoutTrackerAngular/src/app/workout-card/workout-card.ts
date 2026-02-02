import { Component, EventEmitter, Input, Output, ViewChild, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Workout } from '../model/workout';
import { Musclegroup } from '../model/musclegroup';
import { MuscleGroupSelector } from '../musclegroup-selector/musclegroup-selector';
import { WorkoutService } from '../services/workout-service';
import { WorkoutsetTracker } from "../workoutset-tracker/workoutset-tracker";
import { User } from '../model/user';
import { Exercises } from '../model/exercises';

@Component({
  selector: 'app-workout-card',
  standalone: true,
  imports: [CommonModule, FormsModule, MuscleGroupSelector, WorkoutsetTracker],
  templateUrl: './workout-card.html',
  styleUrl: './workout-card.css',
})

export class WorkoutCard implements OnInit{
  constructor(private workoutService: WorkoutService){}
  workouts: Workout[] = [] // Initialized with an empty array
  ngOnInit(): void{
    this.workoutService.getWorkouts().subscribe(
      workouts => {
        console.log('API response:', workouts);
        this.workouts = workouts;
      },
      error => {
        console.error('API error:', error);
      }
    );
  }

// onOverlayClosed() {
// throw new Error('Method not implemented.');
// }
  // null = no active workout
  @Input() workout: Workout | null = null;
  @Input() user!: User | null;
  //@Input() exerciseID!: Exercises | null;

  // temporary notes during this workout
  @Input() notes = '';

  // tell parent to finish and save the workout
  @Output() notesChange = new EventEmitter<string>();
  @Output() finish = new EventEmitter<void>();
  // selectedExerciseId: number | null = null;
  // selectedExerciseName = '';

  onNotesChange(value: string) {
  this.notesChange.emit(value);
  }

  onFinishClicked() {
    this.finish.emit();
  }

  @ViewChild(MuscleGroupSelector) muscleGroupSelector!: MuscleGroupSelector;

addExercises(): void {
  this.muscleGroupSelector.openOverlay();
}

// workout.ts (component behind workout.html)
// Add this near your other fields:
exerciseBlocks: Array<{ exerciseId: number; name: string }> = [];

// Replace/implement your existing handler:
onExerciseAdded(ex: any) {
  const exerciseId = ex?.exerciseId;
  const name = ex?.name ?? '';

  if (!exerciseId) return;

  const existing = this.exerciseBlocks.find(b => b.exerciseId === exerciseId);
  if (!existing) {
    this.exerciseBlocks.push({ exerciseId, name });
  } else {
    // optional: keep name in sync if it changes
    existing.name = name || existing.name;
  }
}

}


// <app-workoutset
//   *ngIf="selectedExercise"
//   [exerciseName]="selectedExercise.name"
//   [todaysBestText]="selectedExerciseBestText"
//   (setsChanged)="onSetsChanged($event)"
//   (addExercise)="openExerciseSelector()"
//   (finishWorkout)="onFinishClicked()">
// </app-workoutset>