import { Component, Output, EventEmitter, Input} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Musclegroup } from '../model/musclegroup';
import { ExerciseSelector } from "../exercise-selector/exercise-selector";
import { Exercises } from '../model/exercises';

@Component({
  selector: 'app-musclegroup-selector',
  standalone: true,
  imports: [CommonModule, ExerciseSelector],
  templateUrl: './musclegroup-selector.html',
  styleUrl: './musclegroup-selector.css',

})
export class MuscleGroupSelectorComponent {
  @Output() exerciseAdded = new EventEmitter<any>();  // Exercise from ExerciseSelector
  @Output() overlayClosed = new EventEmitter<void>();

  muscleGroups: Musclegroup[] = [];
  selectedMuscleGroupId: number | null = null;
  showExercises = false;
  loading = false;
  isOverlayOpen = false;
  allExercises: Exercises[] = [];

  async openOverlay(): Promise<void> {
    this.isOverlayOpen = true;
    this.loading = true;
    document.body.style.overflow = 'hidden';
    
    try {
      // Load from your SQL MuscleGroups table
      const response = await fetch('/api/musclegroups');  // Add this endpoint
      this.muscleGroups = await response.json();
    } catch (error) {
      console.error('Failed to load muscle groups:', error);
      // Fallback to derived from exercises if no /api/musclegroups
      await this.loadFromExercises();
    } finally {
      this.loading = false;
    }
  }

  private async loadFromExercises(): Promise<void> {
    try {
      const response = await fetch('/api/musclegroups');  // Direct API call
      this.muscleGroups = await response.json();
    } catch (error) {
      console.error('Failed to load muscle groups:', error);
      this.muscleGroups = [];
    }
  }
  

  selectMuscleGroup(groupId: number): void {
    this.selectedMuscleGroupId = groupId;
    this.showExercises = true;
  }

  backToGroups(): void {
    this.showExercises = false;
    this.selectedMuscleGroupId = null;
  }

  onExerciseSelected(exercise: any): void {
    this.exerciseAdded.emit(exercise);
    this.closeOverlay();
  }

  closeOverlay(): void {
    this.isOverlayOpen = false;
    this.showExercises = false;
    document.body.style.overflow = 'auto';
    this.overlayClosed.emit();
  }
}
