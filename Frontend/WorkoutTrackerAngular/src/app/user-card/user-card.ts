import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { User } from '../model/user';
import { UserService } from '../services/user-service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-card.html',
  styleUrls: ['./user-card.css'],
})
export class UserCard implements OnInit {
  constructor(private userService: UserService) {}

  users: User[] = [];
  showUserPopup = false;
  @Output() userChange = new EventEmitter<User | null>();
  isLoggedIn = false;

  private nextUserId = 1; // fallback local id
  lastSavedUser: User | null = null; // <-- explicitly nullable

  // Form model used by template
  user: { id: number; userName: string; eMail: string; password: string; createdAt: string } = {
    id: 0,
    userName: '',
    eMail: '',
    password: '',
    createdAt: ''
  };

  ngOnInit(): void {
    this.loadUsers();
  }

  private loadUsers(): void {
    this.userService.getUsers().subscribe({
      next: (users: User[]) => this.users = users ?? [],
      error: (err: any) => console.error('Failed to load users:', err)
    });
  }

  get loginButtonLabel(): string {
    return this.isLoggedIn ? '✏️ Change Profile' : '🔐 Login';
  }

  openLogin(): void {
    // Populate the form safely using lastSavedUser if available
    this.user = {
      id: this.nextUserId,
      userName: this.lastSavedUser?.userName ?? '',
      eMail: this.lastSavedUser?.eMail ?? '',
      password: '',
      createdAt: new Date().toISOString()
    };
    this.showUserPopup = true;
  }

  private isValidEmail(email: string): boolean {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test((email ?? '').trim());
  }

  saveUser(): void {
    const email = (this.user.eMail ?? '').trim();
    const password = this.user.password ?? '';

    if (!this.isValidEmail(email)) {
      alert('Please enter a valid email address.');
      return;
    }
    if (!password || password.length < 6) {
      alert('Please enter a password with at least 6 characters.');
      return;
    }

    const credentials = { email, password };

    if (typeof (this.userService as any).login !== 'function') {
      alert('Login not available. Check UserService.');
      console.error('UserService missing login():', this.userService);
      return;
    }

    this.userService.login(credentials).subscribe({
      next: (res: any) => {
        // Defensive handling of server response
        const createdId = Number(res?.id) || this.nextUserId;
        const userName = (res?.userName ?? res?.username ?? this.user.userName ?? '').toString();
        const createdAt = res?.createdAt ? String(res.createdAt) : new Date().toISOString();

        // Build a sanitized User object (avoid storing password)
        const saved: User = {
          id: createdId,
          userName,
          eMail: email,
          // If your frontend User type includes other fields, add them here as needed.
        } as any;

        this.isLoggedIn = true;
        this.lastSavedUser = saved;
        this.userChange.emit(this.lastSavedUser);
        this.showUserPopup = false;
        this.nextUserId = Math.max(this.nextUserId, (this.lastSavedUser?.id ?? 0) + 1);
        this.loadUsers();
      },
      error: (err: any) => {
        if (err instanceof HttpErrorResponse) {
          if (err.status === 404) {
            // Create the user if not found
            const userToCreate = {
              Username: this.user.userName || email.split('@')[0],
              Email: email,
              PasswordHash: password
            };
            this.userService.createUser(userToCreate as any).subscribe({
              next: (createRes: any) => {
                const createdId = Number(createRes?.id) || this.nextUserId;
                const saved: User = {
                  id: createdId,
                  userName: userToCreate.Username,
                  eMail: email
                } as any;

                this.isLoggedIn = true;
                this.lastSavedUser = saved;
                this.userChange.emit(this.lastSavedUser);
                this.nextUserId++;
                this.showUserPopup = false;
                this.loadUsers();
              },
              error: (createErr: any) => {
                console.error('Create user failed:', createErr);
                alert('Failed to create user. See console.');
              }
            });
            return;
          } else if (err.status === 401) {
            alert('Invalid password. Please try again.');
            return;
          }
        }
        console.error('Login error:', err);
        alert('Login failed. See console for details.');
      }
    });
  }

  cancel(): void {
    if (this.lastSavedUser) {
      // Access lastSavedUser only inside guarded block
      this.user = {
        id: this.lastSavedUser.id,
        userName: this.lastSavedUser.userName ?? '',
        eMail: (this.lastSavedUser as any).eMail ?? '',
        password: '',
        createdAt: (this.lastSavedUser as any).createdAt ?? ''
      };
      this.isLoggedIn = true;
    } else {
      this.user = { id: 0, userName: '', eMail: '', password: '', createdAt: '' };
      this.isLoggedIn = false;
    }
    this.showUserPopup = false;
  }
}