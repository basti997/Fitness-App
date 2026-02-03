import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Workout } from '../model/workout';

@Injectable({
  providedIn: 'root',
})
export class WorkoutService {

  baseUrl = 'http://localhost:5038/api';
  constructor(private http: HttpClient){}
    
    getWorkouts(): Observable<Workout[]> {
      return this.http.get<Workout[]>(`${this.baseUrl}/workout`);
    }
    getWorkout(id: number): Observable<Workout> {
      return this.http.get<Workout>(`${this.baseUrl}/workout/${id}`);
    }
    createWorkout(workout: Workout): Observable<any> {
      return this.http.post(`${this.baseUrl}/workout`, workout);
    }
    updateWorkout(workout: Workout): Observable<any> {
      return this.http.put(`${this.baseUrl}/workout`, workout);
    }
    deleteWorkout(id: number): Observable<any> {
      return this.http.delete(`${this.baseUrl}/workout/${id}`);
    }
    // <-- FIXED: call backend route "byUser"
    getWorkoutsByUser(userId: number): Observable<Workout[]> {
      return this.http.get<Workout[]>(`${this.baseUrl}/workout/byUser/${userId}`);
    }
}