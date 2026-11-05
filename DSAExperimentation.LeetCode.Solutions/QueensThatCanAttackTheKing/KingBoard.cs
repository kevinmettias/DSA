using DSAExperimentation.Domain.Games.Chess;

namespace DSAExperimentation.LeetCode.Solutions.QueensThatCanAttackTheKing;

// Where the king stands and how far the board extends around it - the origin and
// the bounds of every ray LC 1222 casts.
//
// It exists so a caller that already knows both - a benchmark that placed the king
// itself in [GlobalSetup] on chess's 8x8 board - can hand them to either strategy
// instead of restating "king plus two loose ints" at every call. The same role RookSquare
// plays for LC 999, and per §17.3 it lives beside the solution rather than in
// Domain/Games/Chess: LC 1222 is the only problem with a king whose square is the query
// origin.
internal readonly record struct KingBoard(int KingRow, int KingCol, int BoardSize)
{
    public static KingBoard Standard(int[] king) => new(king[0], king[1], ChessRules.BoardSize);

    public bool IsInBounds(int row, int col) =>
        row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
}
