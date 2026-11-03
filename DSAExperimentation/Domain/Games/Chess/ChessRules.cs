namespace DSAExperimentation.Domain.Games.Chess;

// Chess as LC 999, LC 1222, LC 2056 and LC 3001 state it: an 8x8 board. These are the problems'
// *what* (ARCHITECTURE §17.6), which every arm reads, baselines among them (§17.5); how a solution
// walks a line and what it checks at each square stay with each strategy. The pieces and the moves they make are
// ChessPiece's named values. The knight problems here are played on boards other than chess's, so
// they read a knight's jumps but not this board.
/// <summary>The chess board's size.</summary>
public static class ChessRules
{
    /// <summary>The number of rows, and of columns, on the board.</summary>
    public const int BoardSize = 8;
}
