import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Musclegroup } from '../model/musclegroup';

@Injectable({
  providedIn: 'root',
})
export class MuscleGroupService {

  baseUrl = 'http://localhost:5038/api';
  constructor(private http: HttpClient){}
    
    getMuscleGroups(): Observable<Musclegroup[]> {
      return this.http.get<Musclegroup[]>(`${this.baseUrl}/musclegroup`);
    }
    getMuscleGroup(id: number): Observable<Musclegroup> {
      return this.http.get<Musclegroup>(`${this.baseUrl}/musclegroup/${id}`);
    }
    createMuscleGroup(musclegroup: Musclegroup): Observable<any> {
      return this.http.post(`${this.baseUrl}/musclegroup`, musclegroup);
    }
    updateMuscleGroup(musclegroup: Musclegroup): Observable<any> {
      return this.http.put(`${this.baseUrl}/musclegroup`, musclegroup);
    }
    deleteMuscleGroup(id: number): Observable<any> {
      return this.http.delete(`${this.baseUrl}/musclegroup/${id}`);
    }
  
}