namespace DSAExperimentation.Domain.Mathematics.Lattices;

// One move on a grid of rows and columns, as how far it goes down the rows and across the columns:
// a step of the square lattice the grid's cells sit on. It is a fact about the grid, general to no
// one game or domain (the owner's ruling, 2026-11-03), so chess's pieces, a flood fill and a
// baseline all read it here.
// The moves the grid tables are made of are named, so a reader sees Up rather than decoding (-1, 0):
// row numbers grow downward, so Up is -1 row. The L-shaped jumps are named for the two squares of
// their long leg and the one of their short leg: UpUpLeft is two rows up and one column left, which
// is Up + Up + Left.
//
// The lattice's algebra (IPlanarLatticeStep): steps add, a step's opposite is its negation, and the
// square's symmetries are the quarter turns and a mirror, so four quarter turns make a whole one. A
// quarter turn clockwise as the grid is drawn, rows growing downward, takes (dr, dc) to (dc, -dr), so
// Up becomes Right; the mirror flips left and right, so Left becomes Right and Up stays Up.
/// <summary>A move on a grid: how many rows it goes down, and how many columns right.</summary>
/// <param name="DeltaRow">How many rows the move goes down; negative goes up.</param>
/// <param name="DeltaCol">How many columns the move goes right; negative goes left.</param>
public readonly record struct GridStep(int DeltaRow, int DeltaCol) : IPlanarLatticeStep<GridStep>
{
    // The square lattice's smallest rotation is a quarter turn.
    internal const int QuarterTurnsPerTurn = 4;

    /// <inheritdoc/>
    public static int RotationOrder => QuarterTurnsPerTurn;

    /// <summary>Gets the step that stays put: (0, 0), which adds nothing.</summary>
    public static GridStep AdditiveIdentity { get; } = new(0, 0);

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

    /// <summary>Takes one step and then another: the steps' rows and columns added.</summary>
    /// <param name="left">The first step.</param>
    /// <param name="right">The step taken after it.</param>
    /// <returns>The single step that goes as far as both.</returns>
    public static GridStep operator +(GridStep left, GridStep right) =>
        new(left.DeltaRow + right.DeltaRow, left.DeltaCol + right.DeltaCol);

    /// <summary>The opposite step: as far, the other way.</summary>
    /// <param name="step">The step to reverse.</param>
    /// <returns>The step that undoes it.</returns>
    public static GridStep operator -(GridStep step) => new(-step.DeltaRow, -step.DeltaCol);

    /// <inheritdoc/>
    public static GridStep Rotate(GridStep step) => new(step.DeltaCol, -step.DeltaRow);

    /// <inheritdoc/>
    public static GridStep Reflect(GridStep step) => new(step.DeltaRow, -step.DeltaCol);
}
