import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { WorkoutSet } from '../model/workoutset';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Workout } from '../model/workout';
import { User } from '../model/user';
import { WorkoutSetsService } from '../services/workout-sets-service';

@Component({
  selector: 'app-workoutset-tracker',
  standalone: true,
  imports: [CommonModule, FormsModule ],
  templateUrl: './workoutset-tracker.html',
  styleUrl: './workoutset-tracker.css',
})
export class WorkoutsetTracker implements OnInit{
console: any;
  constructor(private workoutsetService: WorkoutSetsService){}
    @Input() exerciseName = '';           // e.g. "Bench Press"
    @Input() todaysBestText = '';        // e.g. "Today's best: 80kg x 8 reps"
    @Input() user!: User | null;      
    @Input() workout!: Workout | null;
    @Input() exerciseId: number | null = null;

    @Output() setsChanged = new EventEmitter<WorkoutSet[]>();
    @Output() addExercise = new EventEmitter<void>();   // tell parent to open exercise selector
    @Output() finishWorkout = new EventEmitter<void>(); // parent Workout component handles saving


    sets: WorkoutSet[] = [];
    editingSetId: number | null = null;
    editingSetNumber: number | null = null;
    currentWeight = 0;
    currentReps = 0;

    // ✅ SINGLE ngOnInit - proper logic
  ngOnInit(): void {
    // Load sets for THIS specific workout (not all!)
    if (this.workout?.workout_id) {
      this.loadWorkoutSets(this.workout.workout_id);
    }
  }

  // ✅ Load sets for specific workout
  private loadWorkoutSets(workoutId: number): void {
    this.workoutsetService.getSetsByWorkout(workoutId).subscribe({
      next: (workoutsets: WorkoutSet[]) => {
        const exId = this.exerciseId;
        this.sets = (exId == null) ? [] : workoutsets.filter(s => s.exerciseId === exId);
        this.setsChanged.emit(this.sets);
      },
      error: (error) => {
        console.error('Failed to load workout sets:', error);
        this.sets = [];
        this.setsChanged.emit(this.sets);
      }
    });
  }
  
    addSet(): void {
    if (!this.currentWeight || !this.currentReps || !this.workout) {
      return;
    }

    // CHANGE 1: Added a guard so we never send exerciseId <= 0 / null to the backend.
    // Reason: backend POST rejects when ExerciseId <= 0 ("WorkoutSet info not correct"). [file:170]
    if (this.exerciseId === null || this.exerciseId <= 0) {
      console.error('No exercise selected -> cannot save set. exerciseId=', this.exerciseId);
      return;
    }

    const nextNumber =
    this.sets.reduce((m, s) => Math.max(m, s.setNumber), 0) + 1;

    const newSet: WorkoutSet = {
      setNumber: nextNumber,
      weight: this.currentWeight,
      reps: this.currentReps,
      done: true,
      workoutId: this.workout.workout_id, // ✅ Link to parent workout

      // CHANGE 2: Use the real exerciseId (guaranteed > 0 due to guard above).
      // This replaces the previous buggy "exerciseId: 0" which always triggered the backend 400. [file:170]
      exerciseId: this.exerciseId,

      setId: 0,
    };
    console.log('POST payload', newSet);

    this.sets.push(newSet);
    this.setsChanged.emit(this.sets);

    this.workoutsetService.createWorkoutSet(newSet).subscribe({
      next: () => {
        console.log('✅ Set saved!');
        const workoutId = this.workout!.workout_id;
        this.loadWorkoutSets(workoutId);
      },
      error: (error) => {
        console.error('❌ Save failed:', error);
        this.sets.pop();
        this.setsChanged.emit(this.sets);
      },
    });

    // Reset inputs
    this.currentWeight = this.currentWeight;  // Keep weight
    this.currentReps = this.currentReps;
  }
  
    // editSet(set: WorkoutSet): void {
    //   // Load values back into inputs and mark as not done until saved again
    //   this.editingSetId = set.setId;
    //   this.editingSetNumber = set.setNumber;
    //   this.currentWeight = set.weight;
    //   this.currentReps = set.reps;
    //   set.done = false;
    // }
  
    deleteSet(set: WorkoutSet): void {
      if (!set.setId) return;  // Can't delete unsaved
  
      this.workoutsetService.deleteWorkoutSet(set.setId).subscribe({
        next: () => {
          this.sets = this.sets.filter(s => s.setId !== set.setId);
          this.setsChanged.emit(this.sets);
        },
        error: console.error
      });
  }

  editWeight = 0;
editReps = 0;

startEdit(set: WorkoutSet): void {
  if (!set.setId || set.setId <= 0) return;
  this.editingSetId = set.setId;
  this.editWeight = set.weight;
  this.editReps = set.reps;
}

cancelEdit(): void {
  this.editingSetId = null;
}

saveEdit(set: WorkoutSet): void {
  if (!this.workout?.workout_id) return;
  if (!set.setId || set.setId <= 0) return;

  const payload: WorkoutSet = {
    setId: set.setId,
    workoutId: set.workoutId,
    exerciseId: set.exerciseId,
    setNumber: set.setNumber,
    weight: this.editWeight,
    reps: this.editReps,
    done: true,
  };

  this.workoutsetService.updateWorkoutSet(payload).subscribe({
    next: () => {
      this.editingSetId = null;
      this.loadWorkoutSets(this.workout!.workout_id);
    },
    error: (e) => console.error(e),
  });
  }

  debugClick(): void {
    alert('click works');
  }
  
  
    onAddExercise(): void { this.addExercise.emit(); }
    onFinishWorkout(): void { this.finishWorkout.emit(); }
}
