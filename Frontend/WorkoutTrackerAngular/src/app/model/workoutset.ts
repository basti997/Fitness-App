export interface WorkoutSet {
  id: number;          // primary key (matches backend JSON "id")
  workoutId: number;
  exerciseId: number;
  setNumber: number;
  weight: number;
  reps: number;

  done?: boolean;
  editing?: boolean;
}