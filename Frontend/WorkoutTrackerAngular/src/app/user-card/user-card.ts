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
export class UserCard {
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
  Id: 0,
  Username: '',
  Email: '',
  PasswordHash: '',
  CreatedAt: ''
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
    Id: this.nextUserId,
    Username: '',
    Email: '',
    PasswordHash: '',
    CreatedAt: new Date().toISOString()
    };
    } else {
    // First time: also start fresh
    this.user = {
    Id: this.nextUserId,
    Username: '',
    Email: '',
    PasswordHash: '',
    CreatedAt: new Date().toISOString()
    };
    }
    this.showUserPopup = true;
    }

    saveUser() {
      if (!this.user.Username || !this.user.Email) {
        alert('Username and Email required!');
        return;
    }
      const user = {
        Username: this.user.Username,      
        Email: this.user.Email,
        PasswordHash: this.user.PasswordHash || 'hash123'
      };
    
      // ✅ CALL YOUR SERVICE → Backend!
      this.userService.createUser(user as any).subscribe({
        next: (response) => {
          console.log('✅ Backend saved user!', response);
          
          // Update local state
          this.isLoggedIn = true;
          this.lastSavedUser = { ...this.user, Id: 0 }; // Backend gives Id next load
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
