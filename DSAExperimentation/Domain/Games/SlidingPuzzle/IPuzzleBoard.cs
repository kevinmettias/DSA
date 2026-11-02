namespace DSAExperimentation.Domain.Games.SlidingPuzzle;

// A sliding puzzle is fixed by its board's shape: so many rows of so many cells, one cell empty and
// the rest numbered tiles. The 8-puzzle is 3 by 3 and the 15-puzzle 4 by 4; LeetCode 773 plays a 2 by
// 3. How a tile slides, what "solved" means and which arrangements can be solved follow from the
// shape, and are SlidingPuzzle's to state.
/// <summary>The shape of a sliding puzzle's board: its rows and columns.</summary>
public interface IPuzzleBoard
{
    /// <summary>How many rows the board has, at least 2.</summary>
    static abstract int Rows { get; }

    /// <summary>How many cells each row has, at least 2.</summary>
    static abstract int Columns { get; }
}
