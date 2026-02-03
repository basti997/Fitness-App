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

  @Input() todaysBestText: string | null = null;

  @Output() setsChanged = new EventEmitter<WorkoutSet[]>();
  @Output() addExercise = new EventEmitter<void>();
  @Output() finishWorkout = new EventEmitter<void>();

  sets: WorkoutSet[] = [];
  editingSetId: number | null = null;
  editingSetNumber: number | null = null;

  currentWeight = 0;
  currentReps = 0;

  editWeight = 0;
  editReps = 0;

  isSaving = false;

  constructor(private workoutsetService: WorkoutSetsService) {}

  ngOnInit(): void {
    if (this.workout?.id) {
      this.loadWorkoutSets(this.workout.id);
    }
  }

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
    if (!this.currentWeight || !this.currentReps || !this.workout) return;
    if (this.exerciseId === null || this.exerciseId <= 0) {
      console.error('No exercise selected -> cannot save set. exerciseId=', this.exerciseId);
      return;
    }

    const nextNumber = this.sets.reduce((m, s) => Math.max(m, s.setNumber), 0) + 1;

    const newSet: WorkoutSet = {
      id: 0, // server will assign
      setNumber: nextNumber,
      weight: this.currentWeight,
      reps: this.currentReps,
      done: true,
      workoutId: this.workout.id,
      exerciseId: this.exerciseId
    };

    this.sets.push(newSet);
    this.setsChanged.emit(this.sets);

    this.isSaving = true;
    this.workoutsetService.createWorkoutSet(newSet).subscribe({
      next: () => {
        if (this.workout) this.loadWorkoutSets(this.workout.id);
        this.isSaving = false;
      },
      error: (error) => {
        console.error('Save failed:', error);
        this.sets = this.sets.filter(s => s !== newSet);
        this.setsChanged.emit(this.sets);
        this.isSaving = false;
      }
    });
  }

  startEdit(set: WorkoutSet): void {
    if (!set || !set.id) return;
    this.editingSetId = set.id;
    this.editingSetNumber = set.setNumber;
    this.editWeight = set.weight;
    this.editReps = set.reps;
  }

  saveEdit(set: WorkoutSet): void {
    if (!this.editingSetId || !this.workout) return;
    if (!set.id || set.id <= 0) return;

    const payload: WorkoutSet = {
      id: set.id,
      workoutId: set.workoutId,
      exerciseId: set.exerciseId,
      setNumber: this.editingSetNumber ?? set.setNumber,
      weight: this.editWeight,
      reps: this.editReps,
      done: true
    };

    const idx = this.sets.findIndex(s => s.id === payload.id);
    if (idx !== -1) {
      this.sets[idx] = { ...this.sets[idx], ...payload };
      this.setsChanged.emit(this.sets);
    }

    this.workoutsetService.updateWorkoutSet(payload).subscribe({
      next: () => {
        if (this.workout) this.loadWorkoutSets(this.workout.id);
        this.editingSetId = null;
      },
      error: (error) => {
        console.error('Update failed:', error);
        if (this.workout) this.loadWorkoutSets(this.workout.id);
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
    if (!set) return;

    if (!set.id || set.id <= 0) {
      this.sets = this.sets.filter(s => s !== set);
      this.setsChanged.emit(this.sets);
      return;
    }

    this.workoutsetService.deleteWorkoutSet(set.id).subscribe({
      next: () => {
        this.sets = this.sets.filter(s => s.id !== set.id);
        this.setsChanged.emit(this.sets);
      },
      error: (error) => {
        console.error('Delete failed:', error);
        alert('Failed to delete set.');
      }
    });
  }

  debugClick(): void {
    console.log('WorkoutsetTracker debug:', { workout: this.workout, exerciseId: this.exerciseId, sets: this.sets });
  }

  onAddExercise(): void {
    this.addExercise.emit();
  }
}