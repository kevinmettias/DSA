namespace DSAExperimentation.LeetCode.Solutions.CatAndMouseII;

// A position of LC 1728's game: where each animal stands, by walkable cell number, and the seat of
// the animal to move (CatAndMouseGame's MouseSeat or CatSeat).
internal readonly record struct CatAndMousePosition(int Mouse, int Cat, int Mover);
