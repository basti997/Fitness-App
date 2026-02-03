import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WorkoutSet } from '../model/workoutset';
import { WorkoutSetsService } from '../services/workout-sets-service';
import { Workout } from '../model/workout';
import { User } from '../model/user';

@Component({
  selector: 'app-workoutset-tracker',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './workoutset-tracker.html',
  styleUrls: ['./workoutset-tracker.css'],
})
export class WorkoutsetTracker implements OnInit {
  @Input() workout: Workout | null = null;
  @Input() user: User | null = null;
  @Input() exerciseName = '';
  @Input() exerciseId: number | null = null;

  // Optional input shown in the template
  @Input() todaysBestText: string | null = null;

  @Output() setsChanged = new EventEmitter<WorkoutSet[]>();
  @Output() addExercise = new EventEmitter<void>();
  @Output() finishWorkout = new EventEmitter<void>();

  sets: WorkoutSet[] = [];
  editingSetId: number | null = null;
  editingSetNumber: number | null = null;

  // Inputs for creating a set
  currentWeight = 0;
  currentReps = 0;

  // Inputs for editing an existing set
  editWeight = 0;
  editReps = 0;

  constructor(private workoutsetService: WorkoutSetsService) {}

  ngOnInit(): void {
    if (this.workout?.id) {
      this.loadWorkoutSets(this.workout.id);
    }
  }

  // Load all sets for the workout then filter by exerciseId in the subscription (template uses filtered `sets`)
  private loadWorkoutSets(workoutId: number): void {
    this.workoutsetService.getSetsByWorkout(workoutId).subscribe({
      next: (workoutsets: WorkoutSet[]) => {
        const exId = this.exerciseId;
        this.sets = exId == null ? [] : workoutsets.filter(s => s.exerciseId === exId);
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
      // Basic guard: require numbers and a parent workout
      console.warn('Cannot add set: missing inputs or workout.');
      return;
    }

    if (this.exerciseId === null || this.exerciseId <= 0) {
      console.error('No exercise selected -> cannot save set. exerciseId=', this.exerciseId);
      return;
    }

    const nextNumber = this.sets.reduce((m, s) => Math.max(m, s.setNumber), 0) + 1;

    const newSet: WorkoutSet = {
      setNumber: nextNumber,
      weight: this.currentWeight,
      reps: this.currentReps,
      done: true,
      workoutId: this.workout.id,
      exerciseId: this.exerciseId,
      setId: 0, // backend will assign an id
    };

    // Optimistically update UI
    this.sets.push(newSet);
    this.setsChanged.emit(this.sets);

    this.workoutsetService.createWorkoutSet(newSet).subscribe({
      next: (res) => {
        // After success, reload from server to get real ids and ordering
        if (this.workout) this.loadWorkoutSets(this.workout.id);
      },
      error: (error) => {
        console.error('Save failed:', error);
        // rollback optimistic change
        this.sets = this.sets.filter(s => s !== newSet);
        this.setsChanged.emit(this.sets);
      }
    });

    // Keep weight/reps for quick repeated entries (user preferred)
  }

  // Edit flow
  startEdit(set: WorkoutSet): void {
    if (!set || !set.setId) return;
    this.editingSetId = set.setId;
    this.editingSetNumber = set.setNumber;
    this.editWeight = set.weight;
    this.editReps = set.reps;
  }

  saveEdit(set: WorkoutSet): void {
    if (!this.editingSetId || !this.workout) return;

    // Apply edits locally
    const idx = this.sets.findIndex(s => s.setId === this.editingSetId);
    if (idx === -1) {
      this.cancelEdit();
      return;
    }

    const updated: WorkoutSet = {
      ...this.sets[idx],
      weight: this.editWeight,
      reps: this.editReps,
      setNumber: this.editingSetNumber ?? this.sets[idx].setNumber,
    };

    // Optimistically apply
    this.sets[idx] = updated;
    this.setsChanged.emit(this.sets);

    this.workoutsetService.updateWorkoutSet(updated).subscribe({
      next: () => {
        // After update, reload to ensure server canonical state
        this.loadWorkoutSets(this.workout!.id);
        this.editingSetId = null;
      },
      error: (error) => {
        console.error('Update failed:', error);
        // Reload to restore server state
        this.loadWorkoutSets(this.workout!.id);
        this.editingSetId = null;
      }
    });
  }

  cancelEdit(): void {
    this.editingSetId = null;
    this.editWeight = 0;
    this.editReps = 0;
    this.editingSetNumber = null;
  }

  deleteSet(set: WorkoutSet): void {
    if (!set || !set.setId) {
      // If it's an unsaved local set, just remove it
      this.sets = this.sets.filter(s => s !== set);
      this.setsChanged.emit(this.sets);
      return;
    }

    this.workoutsetService.deleteWorkoutSet(set.setId).subscribe({
      next: () => {
        this.sets = this.sets.filter(s => s.setId !== set.setId);
        this.setsChanged.emit(this.sets);
      },
      error: (error) => {
        console.error('Delete failed:', error);
      }
    });
  }

  // Template button helpers
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