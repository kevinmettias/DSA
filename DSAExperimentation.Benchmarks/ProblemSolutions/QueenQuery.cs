namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// One LC 3001 query as drawn for a benchmark batch: the three squares of the
// board the pieces stand on, each pair kept under the piece that occupies it -
// the white rook, the white bishop and the black queen. LC 3001's own signature
// hands them over as six adjacent ints, where nothing but position says which
// pair belongs to which piece.
internal readonly record struct QueenQuery(
    int RookRow, int RookCol, int BishopRow, int BishopCol, int QueenRow, int QueenCol);
