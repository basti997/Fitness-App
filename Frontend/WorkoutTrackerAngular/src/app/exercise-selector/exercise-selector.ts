import { Component, OnInit, Output, EventEmitter, Input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
// import { OverlayModule } from '@angular/cdk/overlay';
import { Exercises } from '../model/exercises';
import { ExercisesService } from '../services/exercises-service';
import { MuscleGroupService } from '../services/muscle-group-service';

@Component({
  selector: 'app-exercise-selector',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './exercise-selector.html',
  styleUrl: './exercise-selector.css',
})
export class ExerciseSelector implements OnInit {
  constructor(private exerciseService: ExercisesService){}

  @Input() initialMuscleGroupId: number | null = null;
  @Output() exerciseAdded = new EventEmitter<Exercises>();
  @Output() overlayClosed = new EventEmitter<void>();

  // Use signals for auto-reactivity
  exercises = signal<Exercises[]>([]);
  selectedExercise: Exercises | null = null;
  loadingExercises = false;
  selectedMuscleGroupId: number | null = null;

  ngOnInit(): void {

    if (this.initialMuscleGroupId !== null) {
      this.selectMuscleGroup(this.initialMuscleGroupId);
    }
  }

  selectMuscleGroup(groupId: number): void { 
    this.loadingExercises = true;
    this.selectedMuscleGroupId = groupId;

    this.exerciseService.getExercisesByMuscleGroup(groupId).subscribe({
      next: (exercises) => {
        console.log('Filtered exercises:', exercises);  // Debug log
        this.exercises.set(exercises);
        this.loadingExercises = false;
      },
      error: (err) => {
        console.error('Exercises by group failed:', err);
        this.exercises.set([]);
        this.loadingExercises = false;
      }
    });
  }

  // Update selectExercise to use signal if needed elsewhere
  selectExercise(exercise: Exercises): void {
    this.selectedExercise = exercise;
  }
  

  addExercise(): void {
    if (this.selectedExercise) {
      this.exerciseAdded.emit(this.selectedExercise);
    }
  }
  close(): void {
    this.overlayClosed.emit();
  }
}