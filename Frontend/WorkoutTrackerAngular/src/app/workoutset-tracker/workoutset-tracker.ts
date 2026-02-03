import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WorkoutSet } from '../model/workoutset';
import { WorkoutSetsService } from '../services/workout-sets-service';
import { Workout } from '../model/workout';
import { User } from '../model/user';
import { WorkoutSetsService } from '../services/workout-sets-service';

@Component({
  selector: 'app-workoutset-tracker',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './workoutset-tracker.html',
  styleUrls: ['./workoutset-tracker.css'],
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
  @Output() addExercise = new EventEmitter<void>();
  @Output() finishWorkout = new EventEmitter<void>();

  sets: WorkoutSet[] = [];
  editingSetId: number | null = null;
  editingSetNumber: number | null = null;

  // Inputs for creating a set
  currentWeight = 0;
  currentReps = 0;

    // ✅ SINGLE ngOnInit - proper logic
  ngOnInit(): void {
    if (this.workout?.id) {
      this.loadWorkoutSets(this.workout.id);
    }
  }

  // Load all sets for the workout then filter by exerciseId in the subscription (template uses filtered `sets`)
  private loadWorkoutSets(workoutId: number): void {
    this.workoutsetService.getSetsByWorkout(workoutId).subscribe({
      next: (workoutsets: any[]) => {
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
    console.log('WorkoutsetTracker debug:', {
      workout: this.workout,
      exerciseId: this.exerciseId,
      sets: this.sets
    });
  }

  // Template calls onAddExercise() but the Output is named addExercise — provide a wrapper
  onAddExercise(): void {
    this.addExercise.emit();
  }
}