import { User } from '../model/user';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { output } from '@angular/core';
import { UserService } from '../services/user-service';

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-card.html',
  styleUrl: './user-card.css',
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
  userChange = output<User | null>();
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
  // reset form, auto-fill timestamp
  // prepare a fresh user each time
  if (this.lastSavedUser) {
    // Start from a fresh new profile (new ID, cleared fields)
    this.user = {
    id: this.nextUserId,
    userName: '',
    eMail: '',
    passwordHash: '',
    createdAt: new Date().toISOString()
    };
    } else {
    // First time: also start fresh
    this.user = {
    id: this.nextUserId,
    userName: '',
    eMail: '',
    passwordHash: '',
    createdAt: new Date().toISOString()
    };
    }
    this.showUserPopup = true;
    }

    saveUser() {
      const userToCreate = {
        Username: this.user.userName,      
        Email: this.user.eMail,
        PasswordHash: this.user.passwordHash
      };
    
      // ✅ CALL YOUR SERVICE → Backend!
      this.userService.createUser(userToCreate as any).subscribe({
        next: (response) => {
          console.log('✅ Backend saved user!', response);
          
          // Update local state
          this.isLoggedIn = true;
          this.lastSavedUser = { ...this.user, id: 0 }; // Backend gives Id next load
          this.nextUserId++;
          this.showUserPopup = false;
          this.userChange.emit(this.lastSavedUser);
          
          // ✅ REFRESH list from backend
          this.ngOnInit();  // Reloads users
        },
        error: (error) => {
          console.error('❌ Backend error:', error);
          alert('Save failed: ' + error.message);
        }
      });
    }
    

    cancel() {
      // Restore last saved user in the card
      if (this.lastSavedUser) {
      this.user = { ...this.lastSavedUser };
      this.isLoggedIn = true;
      }
      this.showUserPopup = false;
      }
}
