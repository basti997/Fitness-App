export interface User {
    // src/app/models/user.model.ts
    user_id: number;           // primary key
    username: string;          // unique, not null
    email: string;             // unique, not null
    password_hash: string;     // not null, hashed password
    created_at: string;        // timestamp with time zone, ISO string
  
}