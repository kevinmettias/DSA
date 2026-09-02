namespace DSAExperimentation.LeetCode.MaximumNumberOfMovesToKillAllPawns;

// The precomputed input MaxMovesByReduceGraphMinimax's hoisted overload takes:
// pairwise knight-move distances between the knight's start (index 0) and every
// pawn (index 1..PawnCount), so a benchmark's [GlobalSetup] can charge every BFS
// to setup and leave only the minimax recursion to the measured call.
internal sealed record KnightDistances(int[,] Distances, int PawnCount);
