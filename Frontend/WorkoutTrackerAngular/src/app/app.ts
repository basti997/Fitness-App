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

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, UserCard, WorkoutCard, WorkoutList, WorkoutsetTracker, FormsModule],   // put imports here, not above
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
addExerciseToWorkout($event: Event) {
throw new Error('Method not implemented.');
}
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
if (!this.user) return;

this.activeWorkout = {
workout_id: this.nextWorkoutId,
user_id: this.user.id,
workout_date: new Date().toISOString(),
notes: ''
};
this.activeNotes = '';
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
this.workouts.sort((a, b) => a.workout_date < b.workout_date ? 1 : -1);

this.nextWorkoutId++;
this.activeWorkout = null;
this.activeNotes = '';
}
}