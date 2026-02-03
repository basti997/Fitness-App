import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { User } from './model/user';
import { Workout } from './model/workout';
import { UserCard } from './user-card/user-card';
import { WorkoutCard } from './workout-card/workout-card';
import { WorkoutList } from './workout-list/workout-list';
import { WorkoutsetTracker } from "./workoutset-tracker/workoutset-tracker";
import { WorkoutService } from './services/workout-service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, UserCard, WorkoutCard, WorkoutList, WorkoutsetTracker, FormsModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  constructor(private workoutService: WorkoutService) {}

  protected readonly title = signal('WorkoutTrackerAngular');

  user: User | null = null;

  workouts: Workout[] = [];
  private nextWorkoutId = 1;

  activeWorkout: Workout | null = null;
  activeNotes = '';
  showMuscleSelector: any;

  onUserChanged(user: User | null) {
    this.user = user;
  }

  startNewWorkout() {
    if (!this.user || !this.user.id || this.user.id <= 0) {
      alert('Please create or select a user before starting a workout.');
      return;
    }

    const payload: Partial<Workout> = {
      userId: this.user.id,
      workoutDate: new Date().toISOString(),
      notes: ''
    };

    // Persist the workout first so workoutId is valid for sets
    this.workoutService.createWorkout(payload as any).subscribe({
      next: (res: any) => {
        const newId = res?.id ?? 0;
        if (newId > 0) {
          this.activeWorkout = {
            id: newId,
            userId: this.user!.id,
            workoutDate: payload.workoutDate!,
            notes: ''
          };
        } else {
          // fallback to local id if backend didn't return id (shouldn't happen with server changes)
          this.activeWorkout = {
            id: this.nextWorkoutId,
            userId: this.user!.id,
            workoutDate: payload.workoutDate!,
            notes: ''
          };
          this.nextWorkoutId++;
        }
        this.activeNotes = '';
      },
      error: (err) => {
        console.error('Failed to create workout:', err);
        alert('Failed to start workout. See console for details.');
      }
    });
  }

  onActiveNotesChange(notes: string) {
    this.activeNotes = notes;
  }

  finishActiveWorkout() {
    if (!this.activeWorkout) return;

    const finished: Workout = {
      ...this.activeWorkout,
      notes: this.activeNotes.trim()
    };

    this.workouts.push(finished);
    this.workouts.sort((a, b) => a.workoutDate < b.workoutDate ? 1 : -1);

    this.nextWorkoutId++;
    this.activeWorkout = null;
    this.activeNotes = '';
  }
}