namespace DSAExperimentation.Domain.Games.SlidingPuzzle;

/// <summary>The 8-puzzle: eight tiles on a 3 by 3 board.</summary>
public readonly struct EightPuzzle : IPuzzleBoard
{
    public static int Rows => 3;

    public static int Columns => 3;
}
