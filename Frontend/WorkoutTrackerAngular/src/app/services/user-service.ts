import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ApiUser {
  id?: number;
  username?: string;
  email?: string;
  createdAt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  // Set this to the actual backend URL + base path
  private backendOrigin = 'http://localhost:5038';
  private baseUrl = `${this.backendOrigin}/api/user`;

  constructor(private http: HttpClient) {}

  getUsers(): Observable<ApiUser[]> {
    return this.http.get<ApiUser[]>(this.baseUrl);
  }

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/login`, credentials);
  }

  createUser(createPayload: any): Observable<any> {
    return this.http.post<any>(this.baseUrl, createPayload);
  }

  getById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/${id}`);
  }
}