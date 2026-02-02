import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WorkoutSet } from '../model/workoutset';

@Injectable({
  providedIn: 'root',
})
export class WorkoutSetsService {

  baseUrl = 'http://localhost:5038/api';
  constructor(private http: HttpClient) {}

  // === PRIMARY ENDPOINTS (Customer Journey) ===

  createWorkoutSet(workoutSet: WorkoutSet): Observable<any> {
    return this.http.post(`${this.baseUrl}/workoutset`, workoutSet);
  }
  getSetsByWorkout(workoutId: number): Observable<WorkoutSet[]> {
    return this.http.get<WorkoutSet[]>(`${this.baseUrl}/workoutset/byWorkout/${workoutId}`);
  }
  getSetsByExerciseAndUser(userId: number, exerciseId: number): Observable<WorkoutSet[]> {
    return this.http.get<WorkoutSet[]>(`${this.baseUrl}/workoutset/byExerciseAndUser/${userId}/${exerciseId}`);
  }

  // === BASIC CRUD ===

  getWorkoutSets(): Observable<WorkoutSet[]> {
    return this.http.get<WorkoutSet[]>(`${this.baseUrl}/workoutset`);
  }
  getWorkoutSet(id: number): Observable<WorkoutSet> {
    return this.http.get<WorkoutSet>(`${this.baseUrl}/workoutset/${id}`);
  }
  createWorkoutSetLegacy(workoutSet: WorkoutSet): Observable<any> {
    return this.http.post(`${this.baseUrl}/workoutset/`, workoutSet);
  }
  updateWorkoutSet(workoutSet: WorkoutSet): Observable<any> {
    return this.http.put(`${this.baseUrl}/workoutset`, workoutSet);
  }
  deleteWorkoutSet(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/workoutset/${id}`);
  }
  
}
