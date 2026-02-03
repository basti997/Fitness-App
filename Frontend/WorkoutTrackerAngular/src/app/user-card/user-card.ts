import { User } from '../model/user';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../services/user-service';

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-card.html',
  styleUrls: ['./user-card.css'],
})
export class UserCard implements OnInit{
  constructor(private userService: UserService){}
  users: User [] = [];
  ngOnInit(): void{
    this.userService.getUsers().subscribe(
      users => {
        console.log('API response:', users);
        this.users = users;
      },
      error => {
        console.error('API error:', error);
      }
    );
  }
  showUserPopup = false;
  @Output() userChange = new EventEmitter<User | null>();
  isLoggedIn = false;

  private nextUserId = 1; // starts at 1, will increase on each new user

  // lastSavedUser is what is shown in the card
  lastSavedUser: User | null = null;

  user: User = {
    id: 0,
    userName: '',
    eMail: '',
    passwordHash: '',
    createdAt: ''
  };

  get loginButtonLabel(): string {
    return this.isLoggedIn ? '✏️ Change Profile' : '🔐 Login';
  }

  openLogin() {
    this.user = {
      id: this.nextUserId,
      userName: '',
      eMail: '',
      passwordHash: '',
      createdAt: new Date().toISOString()
    };
    this.showUserPopup = true;
  }

  saveUser() {
    const userToCreate = {
      Username: this.user.userName,      
      Email: this.user.eMail,
      PasswordHash: this.user.passwordHash
    };
    
    this.userService.createUser(userToCreate as any).subscribe({
      next: (response: any) => {
        console.log('Backend saved user!', response);
        
        // If backend returned an id, use it
        const createdId = response?.id ?? 0;
        this.isLoggedIn = true;
        this.lastSavedUser = { ...this.user, id: createdId };
        if (!createdId) {
          // fallback: keep local id but warn
          this.lastSavedUser.id = this.nextUserId;
        }
        this.nextUserId++;
        this.showUserPopup = false;
        this.userChange.emit(this.lastSavedUser);
        
        // Refresh list from backend
        this.ngOnInit();
      },
      error: (error) => {
        console.error('Backend error:', error);
        alert('Save failed: ' + (error?.message ?? error));
      }
    });
  }
    
  cancel() {
    if (this.lastSavedUser) {
      this.user = { ...this.lastSavedUser };
      this.isLoggedIn = true;
    }
    this.showUserPopup = false;
  }
}