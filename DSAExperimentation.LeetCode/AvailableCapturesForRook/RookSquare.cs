namespace DSAExperimentation.LeetCode.AvailableCapturesForRook;

// The rook's square on the board, located once. It exists so a caller that
// already knows where the rook is - a benchmark that placed it itself in
// [GlobalSetup] - can hand it to either strategy instead of paying for the
// O(rows*cols) scan that finds it, and so those hoisted overloads read as
// "board plus rook" rather than "board plus two loose ints". LC 999 is the only
// problem with a board whose single rook is the query origin, so per §17.3 this
// lives beside the solution rather than in Domain/.
internal readonly record struct RookSquare(int Row, int Col);
