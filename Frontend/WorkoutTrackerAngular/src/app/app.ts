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
  private nextWorkoutId = 1; // used only for allocating temp ids (if needed)

  activeWorkout: Workout | null = null;
  // stable ID shown in the UI for the active workout. Set once when workout starts.
  activeWorkoutDisplayId: number | string | null = null;

  activeNotes = '';
  showMuscleSelector: any;

  onUserChanged(user: User | null) {
    this.user = user;

    if (this.user && this.user.id && this.user.id > 0) {
      this.workoutService.getWorkoutsByUser(this.user.id).subscribe({
        next: (wks: Workout[]) => {
          this.workouts = wks.sort((a, b) => a.workoutDate < b.workoutDate ? 1 : -1);
        },
        error: (err) => {
          console.error('Failed to load workouts for user:', err);
          this.workouts = [];
        }
      });
    } else {
      this.workouts = [];
    }
  }

  // allocate a negative temporary id (if server create fails)
  private allocateTempId(): number {
    const id = -this.nextWorkoutId;
    this.nextWorkoutId++;
    return id;
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

    // Try creating on server; if it fails, allocate a negative temp id.
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
          this.activeWorkoutDisplayId = newId;
        } else {
          // fallback local negative id (distinct from server ids)
          const temp = this.allocateTempId();
          this.activeWorkout = {
            id: temp,
            userId: this.user!.id,
            workoutDate: payload.workoutDate!,
            notes: ''
          };
          this.activeWorkoutDisplayId = temp;
        }
        this.activeNotes = '';
      },
      error: (err) => {
        console.error('Failed to create workout on server; using local id:', err);
        const temp = this.allocateTempId();
        this.activeWorkout = {
          id: temp,
          userId: this.user!.id,
          workoutDate: payload.workoutDate!,
          notes: ''
        };
        this.activeWorkoutDisplayId = temp;
        this.activeNotes = '';
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

    // push to local list so history shows immediately; server persistence handled elsewhere
    this.workouts.push(finished);
    this.workouts.sort((a, b) => a.workoutDate < b.workoutDate ? 1 : -1);

    // Do NOT change nextWorkoutId here — we only increment nextWorkoutId when allocating a temp id.
    // Clear active workout and its stable display id
    this.activeWorkout = null;
    this.activeWorkoutDisplayId = null;
    this.activeNotes = '';
  }
}