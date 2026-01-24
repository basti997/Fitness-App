import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
// import { OverlayModule } from '@angular/cdk/overlay';
import { Musclegroup } from '../model/musclegroup';
import { Exercises } from '../model/exercises';

@Component({
  selector: 'app-exercise-selector',
  imports: [CommonModule],
  templateUrl: './exercise-selector.html',
  styleUrl: './exercise-selector.css',
})
export class ExerciseSelector implements OnInit {
  @Input() initialMuscleGroupId: number | null = null;
  getSelectedGroupName() {
throw new Error('Method not implemented.');
}
  @Output() exerciseAdded = new EventEmitter<Exercises>();
  @Output() overlayClosed = new EventEmitter<void>();

  muscleGroups: Musclegroup[] = [];
  exercises: Exercises[] = [];
  allExercises: Exercises[] = [];
  selectedMuscleGroupId: number | null = null;
  selectedExercise: Exercises | null = null;
  isOverlayOpen = false;
  loading = false;

  async ngOnInit(): Promise<void> {
    // Auto-load initial group if provided (from MuscleGroupSelector)
    if (this.initialMuscleGroupId) {
      await this.loadMuscleGroups();
      await this.selectMuscleGroup(this.initialMuscleGroupId);
    }
  }

  async openOverlay(): Promise<void> {
    this.isOverlayOpen = true;
    this.loading = true;
    document.body.style.overflow = 'hidden';
    
    try {
      await this.loadMuscleGroups();
    } catch (error) {
      console.error('Failed to load muscle groups:', error);
    } finally {
      this.loading = false;
    }
  }

  private async loadMuscleGroups(): Promise<void> {
    try {
      const response = await fetch('/api/musclegroups');  // Direct MuscleGroups endpoint
      this.muscleGroups = await response.json();
    } catch (error) {
      console.error('Failed to load muscle groups:', error);
      this.muscleGroups = [];  // Empty fallback
    }
  }
  

  async selectMuscleGroup(groupId: number): Promise<void> {
    this.loading = true;
    this.selectedMuscleGroupId = groupId;
    
    try {
      // YOUR API: GET api/exercise/byMuscleGroup/{id}
      const response = await fetch(`/api/exercise/byMuscleGroup/${groupId}`);
      this.exercises = await response.json();
    } catch (error) {
      console.error('Failed to load exercises:', error);
      this.exercises = [];
    } finally {
      this.loading = false;
    }
  }

  selectExercise(exercise: Exercises): void {
    this.selectedExercise = exercise;
  }

  addExercise(): void {
    if (this.selectedExercise) {
      this.exerciseAdded.emit(this.selectedExercise);
      this.closeOverlay();
    }
  }

  closeOverlay(): void {
    this.isOverlayOpen = false;
    this.selectedMuscleGroupId = null;
    this.selectedExercise = null;
    this.exercises = [];
    document.body.style.overflow = 'auto';
    this.overlayClosed.emit();
  }
}