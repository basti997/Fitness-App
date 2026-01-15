export interface Workout {
    workout_id: number;     // primary key, SERIAL
    user_id: number;        // foreign key to users
    workout_date: string;   // ISO string for TIMESTAMP WITH TIME ZONE
    notes: string | null;   // nullable TEXT column
}
