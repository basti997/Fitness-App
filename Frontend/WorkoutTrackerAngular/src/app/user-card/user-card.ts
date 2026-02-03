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

  private nextUserId = 1;
  lastSavedUser: User | null = null;

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
    this.userService.getUsers().subscribe(
      (result: any) => {
        this.users = Array.isArray(result) ? result : (result || []);
      },
      (err: any) => {
        console.error('Failed to load users:', err);
        this.users = [];
      }
    );
  }

  get loginButtonLabel(): string {
    return this.isLoggedIn ? '✏️ Change Profile' : '🔐 Login';
  }

  openLogin(): void {
    this.user = {
      id: this.nextUserId,
      userName: (this.lastSavedUser && this.lastSavedUser.userName) || '',
      eMail: (this.lastSavedUser && (this.lastSavedUser as any).eMail) || '',
      password: '',
      createdAt: new Date().toISOString()
    };
    this.showUserPopup = true;
  }

  private isValidEmail(email: string): boolean {
    var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test((email || '').trim());
  }

  saveUser(): void {
    var email = (this.user.eMail || '').trim();
    var password = this.user.password || '';

    if (!this.isValidEmail(email)) {
      alert('Please enter a valid email address.');
      return;
    }
    if (!password || password.length < 6) {
      alert('Please enter a password with at least 6 characters.');
      return;
    }

    var credentials = { email: email, password: password };

    if (typeof (this.userService as any).login !== 'function') {
      alert('Login not available. Check UserService.');
      console.error('UserService missing login():', this.userService);
      return;
    }

    this.userService.login(credentials).subscribe(
      (res: any) => {
        var createdId = (res && res.id) ? Number(res.id) : this.nextUserId;
        var userName = (res && (res.userName || res.username)) ? (res.userName || res.username) : this.user.userName;
        var createdAt = (res && res.createdAt) ? String(res.createdAt) : new Date().toISOString();

        var saved: User = {
          id: createdId,
          userName: userName,
          eMail: email,
          createdAt: createdAt
        } as any;

        this.isLoggedIn = true;
        this.lastSavedUser = saved;
        this.userChange.emit(this.lastSavedUser);
        this.showUserPopup = false;
        this.nextUserId = Math.max(this.nextUserId, (this.lastSavedUser && this.lastSavedUser.id) ? (this.lastSavedUser.id + 1) : this.nextUserId);
        this.loadUsers();
      },
      (err: any) => {
        if (err instanceof HttpErrorResponse) {
          if (err.status === 404) {
            var username = this.user.userName || email.split('@')[0];
            var userToCreate: any = { Username: username, Email: email, Password: password };

            this.userService.createUser(userToCreate).subscribe(
              (createRes: any) => {
                var createdId = (createRes && createRes.id) ? Number(createRes.id) : this.nextUserId;
                var createdAt = (createRes && createRes.createdAt) ? String(createRes.createdAt) : new Date().toISOString();

                var saved: User = {
                  id: createdId,
                  userName: username,
                  eMail: email,
                  createdAt: createdAt
                } as any;

                this.isLoggedIn = true;
                this.lastSavedUser = saved;
                this.userChange.emit(this.lastSavedUser);
                this.nextUserId = Math.max(this.nextUserId, saved.id + 1);
                this.showUserPopup = false;
                this.loadUsers();
              },
              (createErr: any) => {
                console.error('Create user failed:', createErr);
                alert('Failed to create user. See console.');
              }
            );
            return;
          } else if (err.status === 401) {
            alert('Invalid password. Please try again.');
            return;
          }
        }
        console.error('Login error:', err);
        alert('Login failed. See console for details.');
      }
    );
  }

  cancel(): void {
    if (this.lastSavedUser) {
      this.user = {
        id: this.lastSavedUser.id,
        userName: this.lastSavedUser.userName || '',
        eMail: (this.lastSavedUser as any).eMail || '',
        password: '',
        createdAt: (this.lastSavedUser as any).createdAt || ''
      };
      this.isLoggedIn = true;
    } else {
      this.user = { id: 0, userName: '', eMail: '', password: '', createdAt: '' };
      this.isLoggedIn = false;
    }
    this.showUserPopup = false;
  }
}