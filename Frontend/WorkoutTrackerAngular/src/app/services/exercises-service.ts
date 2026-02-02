import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Exercises } from '../model/exercises';

@Injectable({
  providedIn: 'root',
})
export class ExercisesService {

  baseUrl = 'http://localhost:5038/api';
    constructor(private http: HttpClient){}
      
    getExercises(): Observable<Exercises[]> {
      return this.http.get<Exercises[]>(`${this.baseUrl}/exercise`);
    }
    getExercise(id: number): Observable<Exercises> {
      return this.http.get<Exercises>(`${this.baseUrl}/exercise/${id}`);
    }
    createExercise(exercise: Exercises): Observable<any> {
      return this.http.post(`${this.baseUrl}/exercise`, exercise);
    }
    updateExercise(exercise: Exercises): Observable<any> {
      return this.http.put(`${this.baseUrl}/exercise`, exercise);
    }
    deleteExercise(id: number): Observable<any> {
      return this.http.delete(`${this.baseUrl}/exercise/${id}`);
    }
    getExercisesByMuscleGroup(muscleGroupId: number): Observable<Exercises[]> {
      return this.http.get<Exercises[]>(`${this.baseUrl}/exercise/byMuscleGroup/${muscleGroupId}`);
    }
  
}
