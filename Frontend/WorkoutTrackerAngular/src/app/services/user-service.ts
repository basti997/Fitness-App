import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../model/user';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  baseUrl = 'http://localhost:5038/api';
  constructor(private http: HttpClient){}
    
    getUsers(): Observable<User[]> {
      return this.http.get<User[]>(`${this.baseUrl}/user`);
    }
    getUser(id: number): Observable<User> {
      return this.http.get<User>(`${this.baseUrl}/user/${id}`);
    }
    createUser(user: User): Observable<any> {
      return this.http.post(`${this.baseUrl}/user`, user);
    }
    updateUser(user: User): Observable<any> {
      return this.http.put(`${this.baseUrl}/user`, user);
    }
    deleteUser(id: number): Observable<any> {
      return this.http.delete(`${this.baseUrl}/user/${id}`);
    }
  }