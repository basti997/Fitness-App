import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Workout } from '../model/workout';
import { MuscleGroupSelectorComponent } from '../musclegroup-selector/musclegroup-selector';

@Component({
  selector: 'app-workout-card',
  imports: [CommonModule, FormsModule, MuscleGroupSelectorComponent],
  templateUrl: './workout-card.html',
  styleUrl: './workout-card.css',
})
export class WorkoutCard {
onOverlayClosed() {
throw new Error('Method not implemented.');
}
  // null = no active workout
  @Input() workout: Workout | null = null;

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

  @ViewChild(MuscleGroupSelectorComponent) muscleGroupSelector!: MuscleGroupSelectorComponent;

addExercises(): void {
  this.muscleGroupSelector.openOverlay();
}

onExerciseAdded(exercise: any): void {
  // Add to current workout
  console.log('Exercise added:', exercise);
  // Save to WorkoutSets table
}

}
