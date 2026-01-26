import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { WorkoutSet } from '../model/workoutset';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Workout } from '../model/workout';
import { User } from '../model/user';

@Component({
  selector: 'app-workoutset-tracker',
  standalone: true,
  imports: [CommonModule, FormsModule ],
  templateUrl: './workoutset-tracker.html',
  styleUrl: './workoutset-tracker.css',
})
export class WorkoutsetTracker {
    @Input() exerciseName = '';           // e.g. "Bench Press"
    @Input() todaysBestText = '';        // e.g. "Today's best: 80kg x 8 reps"
    @Output() setsChanged = new EventEmitter<WorkoutSet[]>();
    @Output() addExercise = new EventEmitter<void>();   // tell parent to open exercise selector
    @Output() finishWorkout = new EventEmitter<void>(); // parent Workout component handles saving
    @Input() user!: User | null;      
    @Input() workout!: Workout | null;
    sets: WorkoutSet[] = [];
    currentWeight = 0;
    currentReps = 0;
  
    ngOnInit(): void {
      // Automatically show first set input
      if (this.sets.length === 0) {
        this.currentWeight = 0;
        this.currentReps = 0;
      }
    }
  
    addSet(): void {
      if (!this.currentWeight || !this.currentReps) {
        return;
      }
  
      const newSet: WorkoutSet = {
        setNumber: this.sets.length + 1,
        weight: this.currentWeight,
        reps: this.currentReps,
        done: true,
        workoutId: 0,
        exerciseId: 0
      };
  
      this.sets.push(newSet);
      this.setsChanged.emit(this.sets);
  
      // Reset for next set (auto-show new empty row)
      this.currentWeight = this.currentWeight; // keep last weight as convenience
      this.currentReps = 0;
    }
  
    editSet(set: WorkoutSet): void {
      // Load values back into inputs and mark as not done until saved again
      this.currentWeight = set.weight;
      this.currentReps = set.reps;
      set.done = false;
    }
  
    deleteSet(set: WorkoutSet): void {
      this.sets = this.sets.filter(s => s.setNumber !== set.setNumber)
                           .map((s, index) => ({ ...s, setNumber: index + 1 }));
      this.setsChanged.emit(this.sets);
    }
  
    onAddExercise(): void {
      this.addExercise.emit();
    }
  
    onFinishWorkout(): void {
      this.finishWorkout.emit();
    }

}
