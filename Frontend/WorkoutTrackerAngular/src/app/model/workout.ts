export interface Workout {
  id: number;
  userId: number;
  workoutDate: string; // ISO string
  notes?: string;
  // persisted indicates this workout exists on the server (true) or is local-only (false)
  persisted?: boolean;
}