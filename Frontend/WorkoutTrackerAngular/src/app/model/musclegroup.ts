export interface Musclegroup {
    // src/app/models/musclegroup.model.ts
    muscleGroupId: number;     // SERIAL PRIMARY KEY
    name: string;              // VARCHAR(100) UNIQUE, e.g., 'Chest'
}
