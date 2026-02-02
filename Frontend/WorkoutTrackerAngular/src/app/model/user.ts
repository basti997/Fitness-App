export interface User {
    // src/app/models/user.model.ts
    id: number;           // primary key
    userName: string;          // unique, not null
    eMail: string;             // unique, not null
    passwordHash: string;     // not null, hashed password
    createdAt: string;        // timestamp with time zone, ISO string
  
}