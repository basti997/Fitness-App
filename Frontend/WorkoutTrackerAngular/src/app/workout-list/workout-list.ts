import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Workout } from '../model/workout';

@Component({
  selector: 'app-workout-list',
  imports: [CommonModule],
  templateUrl: './workout-list.html',
  styleUrl: './workout-list.css',
})
export class WorkoutList {
@Input() workouts: Workout[] = [
  
];
}