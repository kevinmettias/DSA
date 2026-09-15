namespace DSAExperimentation.LeetCode.MinimumMovesToCaptureTheQueen;

// One square of the 8x8 board the rook, the bishop and the queen stand on.
// LC 3001 hands all three pieces over as six adjacent ints, and at that call site
// nothing but position says which pair belongs to which piece - the same silent
// swap RookSquare already names for LC 999. A square is one concept, so it is one
// argument that says so.
internal readonly record struct ChessSquare(int Row, int Col);
