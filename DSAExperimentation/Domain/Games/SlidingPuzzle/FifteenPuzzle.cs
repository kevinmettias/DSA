namespace DSAExperimentation.Domain.Games.SlidingPuzzle;

// The board most people mean by a sliding puzzle, named as Domain names a common configuration. It is
// not a second puzzle type: SlidingPuzzle<TBoard> is the one puzzle, and a board type is how its rows
// and columns reach it, as static members that fold to constants, so SlidingPuzzle<FifteenPuzzle>
// compiles 4 by 4's arithmetic and reads no field. Any other shape is a board type of its own, as
// LC 773's 2 by 3 is in its folder.
/// <summary>The 15-puzzle: fifteen tiles on a 4 by 4 board.</summary>
public readonly struct FifteenPuzzle : IPuzzleBoard
{
    public static int Rows => 4;

    public static int Columns => 4;
}
