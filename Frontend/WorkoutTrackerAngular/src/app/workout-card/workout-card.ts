import { Component, EventEmitter, Input, Output, ViewChild, OnInit } from '@angular/core';
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
export class WorkoutCard implements OnInit {
  constructor(private workoutService: WorkoutService) {}
  workouts: Workout[] = []; // Initialized with an empty array

  ngOnInit(): void {
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

  // Inputs
  @Input() workout: Workout | null = null;
  // stable display id passed from parent; if present, the UI will show this instead of workout.id
  @Input() displayId: number | string | null = null;

  @Input() user!: User | null;

  // temporary notes during this workout
  @Input() notes = '';

  // tell parent to finish and save the workout
  @Output() notesChange = new EventEmitter<string>();
  @Output() finish = new EventEmitter<void>();

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

  // exercise block handling
  exerciseBlocks: Array<{ exerciseId: number; name: string }> = [];

  onExerciseAdded(ex: any) {
    const exerciseId = ex?.exerciseId;
    const name = ex?.name ?? '';

    if (!exerciseId) return;

    const existing = this.exerciseBlocks.find(b => b.exerciseId === exerciseId);
    if (!existing) {
      this.exerciseBlocks.push({ exerciseId, name });
    } else {
      existing.name = name || existing.name;
    }
  }
}