namespace DSAExperimentation.DataStructures.Graph.Grids;

// One move on a grid of rows and columns, as how far it goes down the rows and across the columns.
// The moves the grid tables are made of are named, so a reader sees Up rather than decoding (-1, 0):
// row numbers grow downward, so Up is -1 row. The L-shaped jumps are named for the two squares of
// their long leg and the one of their short leg: UpUpLeft is two rows up and one column left.
/// <summary>A move on a grid: how many rows it goes down, and how many columns right.</summary>
/// <param name="DeltaRow">How many rows the move goes down; negative goes up.</param>
/// <param name="DeltaCol">How many columns the move goes right; negative goes left.</param>
public readonly record struct GridStep(int DeltaRow, int DeltaCol)
{
    /// <summary>One row up: (-1, 0).</summary>
    public static GridStep Up { get; } = new(-1, 0);

    /// <summary>One row down: (1, 0).</summary>
    public static GridStep Down { get; } = new(1, 0);

    /// <summary>One column left: (0, -1).</summary>
    public static GridStep Left { get; } = new(0, -1);

    /// <summary>One column right: (0, 1).</summary>
    public static GridStep Right { get; } = new(0, 1);

    /// <summary>One row up and one column left: (-1, -1).</summary>
    public static GridStep UpLeft { get; } = new(-1, -1);

    /// <summary>One row up and one column right: (-1, 1).</summary>
    public static GridStep UpRight { get; } = new(-1, 1);

    /// <summary>One row down and one column left: (1, -1).</summary>
    public static GridStep DownLeft { get; } = new(1, -1);

    /// <summary>One row down and one column right: (1, 1).</summary>
    public static GridStep DownRight { get; } = new(1, 1);

    /// <summary>An L-shaped jump two rows up and one column left: (-2, -1).</summary>
    public static GridStep UpUpLeft { get; } = new(-2, -1);

    /// <summary>An L-shaped jump two rows up and one column right: (-2, 1).</summary>
    public static GridStep UpUpRight { get; } = new(-2, 1);

    /// <summary>An L-shaped jump one row up and two columns left: (-1, -2).</summary>
    public static GridStep UpLeftLeft { get; } = new(-1, -2);

    /// <summary>An L-shaped jump one row up and two columns right: (-1, 2).</summary>
    public static GridStep UpRightRight { get; } = new(-1, 2);

    /// <summary>An L-shaped jump one row down and two columns left: (1, -2).</summary>
    public static GridStep DownLeftLeft { get; } = new(1, -2);

    /// <summary>An L-shaped jump one row down and two columns right: (1, 2).</summary>
    public static GridStep DownRightRight { get; } = new(1, 2);

    /// <summary>An L-shaped jump two rows down and one column left: (2, -1).</summary>
    public static GridStep DownDownLeft { get; } = new(2, -1);

    /// <summary>An L-shaped jump two rows down and one column right: (2, 1).</summary>
    public static GridStep DownDownRight { get; } = new(2, 1);
}
