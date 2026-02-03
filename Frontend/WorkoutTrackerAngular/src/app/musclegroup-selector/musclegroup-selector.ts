import { Component, Output, EventEmitter, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Musclegroup } from '../model/musclegroup';
import { ExerciseSelector } from "../exercise-selector/exercise-selector";
import { Exercises } from '../model/exercises';
import { MuscleGroupService } from '../services/muscle-group-service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-musclegroup-selector',
  standalone: true,
  imports: [CommonModule, ExerciseSelector],
  templateUrl: './musclegroup-selector.html',
  styleUrls: ['./musclegroup-selector.css'],
})
export class MuscleGroupSelector implements OnInit{
  constructor(private MuscleGroupService: MuscleGroupService){}
    muscleGroups: Musclegroup[] = [] // Initialized with an empty array
    ngOnInit(): void{
      this.MuscleGroupService.getMuscleGroups().subscribe(
        muscleGroups => {
          console.log('API response:', muscleGroups);
          this.muscleGroups = muscleGroups;
        },
        error => {
          console.error('API error:', error);
        }
      );
    }
  
  @Output() exerciseAdded = new EventEmitter<any>();  // Exercise from ExerciseSelector
  @Output() overlayClosed = new EventEmitter<void>();

  selectedMuscleGroupId: number | null = null;
  showExercises = false;
  loading = false;
  isOverlayOpen = false;
  allExercises: Exercises[] = [];

  openOverlay(): void {  // No async needed
    this.isOverlayOpen = true;
    this.muscleGroups = this.muscleGroups;  // cached
    document.body.style.overflow = 'hidden';
  }

  // If you want to reload on demand, use the Angular service instead of fetch:
  private async loadFromExercises(): Promise<void> {
    try {
      const muscleGroups = await firstValueFrom(this.MuscleGroupService.getMuscleGroups());
      this.muscleGroups = muscleGroups;
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