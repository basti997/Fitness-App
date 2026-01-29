export interface User {
    // src/app/models/user.model.ts
    Id: number;           // primary key
    Username: string;          // unique, not null
    Email: string;             // unique, not null
    PasswordHash: string;     // not null, hashed password
    CreatedAt: string;        // timestamp with time zone, ISO string
  
}