namespace DSAExperimentation.Domain.Games.SlidingPuzzle;

/// <summary>The 15-puzzle: fifteen tiles on a 4 by 4 board.</summary>
public readonly struct FifteenPuzzle : IPuzzleBoard
{
    public static int Rows => 4;

    public static int Columns => 4;
}
