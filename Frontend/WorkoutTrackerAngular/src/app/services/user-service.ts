import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../model/user';

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface CreateUserRequest {
  Username: string;
  Email: string;
  PasswordHash: string;
}

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

  createUser(user: CreateUserRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/user`, user);
  }

  login(credentials: LoginCredentials): Observable<any> {
    return this.http.post(`${this.baseUrl}/user/login`, credentials);
  }

  updateUser(user: User): Observable<any> {
    return this.http.put(`${this.baseUrl}/user`, user);
  }

  deleteUser(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/user/${id}`);
  }
}