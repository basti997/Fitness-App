import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { WorkoutSet } from '../model/workoutset';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Workout } from '../model/workout';
import { User } from '../model/user';
import { WorkoutSetsService } from '../services/workout-sets-service';
import { OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-workoutset-tracker',
  standalone: true,
  imports: [CommonModule, FormsModule ],
  templateUrl: './workoutset-tracker.html',
  styleUrl: './workoutset-tracker.css',
})
export class WorkoutsetTracker implements OnInit, OnChanges{
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


  ngOnChanges(changes: SimpleChanges): void {
    const workoutChanged = !!changes['workout'];
    const exerciseChanged = !!changes['exerciseId'];
  
    if ((workoutChanged || exerciseChanged) && this.workout?.workout_id && this.exerciseId) {
      this.loadWorkoutSets(this.workout.workout_id);
    }
  }

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
      next: (workoutsets: any[]) => {
        const exId = this.exerciseId;
  
        const normalized: WorkoutSet[] = workoutsets.map(s => ({
          ...s,
          id: s.setId ?? s.set_id ?? s.id,
          workoutId: s.workoutId ?? s.workout_id,
          exerciseId: s.exerciseId ?? s.exercise_id,
        }));
  
        this.sets = (exId == null) ? [] : normalized.filter(s => s.exerciseId === exId);
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

    id: 0,
  };
  console.log('POST payload', newSet);

  this.sets = [...this.sets, newSet];
this.setsChanged.emit(this.sets);

  this.workoutsetService.createWorkoutSet(newSet).subscribe({
    next: () => this.loadWorkoutSets(this.workout!.workout_id),
    error: (e) => console.error(e),
  });

  // Reset inputs
  this.currentWeight = this.currentWeight;  // Keep weight
  this.currentReps = this.currentReps;
  }
  
  deleteSet(set: WorkoutSet): void {
    if (!set.id) return;  // Can't delete unsaved
    const id = set.id;

    this.workoutsetService.deleteWorkoutSet(set.id).subscribe({
      next: () => {
        this.sets = this.sets.filter(s => s.id !== set.id);
        this.setsChanged.emit(this.sets);
      },
      error: console.error
    });
  }

  editWeight = 0;
  editReps = 0;

startEdit(set: WorkoutSet): void {
  if (!set.id || set.id <= 0) return;
  this.editingSetId = set.id;
  this.editWeight = set.weight;
  this.editReps = set.reps;
}

cancelEdit(): void {
  this.editingSetId = null;
}

saveEdit(set: WorkoutSet): void {
  if (!this.workout?.workout_id) return;
  if (!set.id || set.id <= 0) return;

  const id = set.id;
  this.editingSetId = null;

  const payload: WorkoutSet = {
    id: set.id,
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
