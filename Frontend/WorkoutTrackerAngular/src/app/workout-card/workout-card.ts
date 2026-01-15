import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Workout } from '../model/workout';

@Component({
  selector: 'app-workout-card',
  imports: [CommonModule, FormsModule],
  templateUrl: './workout-card.html',
  styleUrl: './workout-card.css',
})
export class WorkoutCard {
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
}
