import { User } from '../model/user';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { output } from '@angular/core';

@Component({
  selector: 'app-user-card',
  imports: [CommonModule, FormsModule],
  templateUrl: './user-card.html',
  styleUrl: './user-card.css',
})
export class UserCard {
  showUserPopup = false;
  userChange = output<User | null>();
  isLoggedIn = false;

  private nextUserId = 1; // starts at 1, will increase on each new user

  // lastSavedUser is what is shown in the card
  lastSavedUser: User | null = null;

  user: User = {
  user_id: 0,
  username: '',
  email: '',
  password_hash: '',
  created_at: ''
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
    user_id: this.nextUserId,
    username: '',
    email: '',
    password_hash: '',
    created_at: new Date().toISOString()
    };
    } else {
    // First time: also start fresh
    this.user = {
    user_id: this.nextUserId,
    username: '',
    email: '',
    password_hash: '',
    created_at: new Date().toISOString()
    };
    }
    this.showUserPopup = true;
    }

  saveUser() {
    if (!this.user.username || !this.user.email) return;
      
    this.isLoggedIn = true;
    this.lastSavedUser = { ...this.user };   // remember what was saved
    this.nextUserId++;
    this.showUserPopup = false;
    this.userChange.emit(this.lastSavedUser);
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
