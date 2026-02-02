export interface WorkoutSet {
        // set_id SERIAL PRIMARY KEY → Auto-generated, optional for new sets
    setId: number;  
    workoutId: number;
    exerciseId: number;
    setNumber: number;
    weight: number;
    reps: number;

    done?: boolean;
    editing?: boolean;
 
}
