using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Mathematics.Polyominoes;

// The twelve free pentominoes, polyominoes of five squares, each a named value under the letter
// Golomb gave it, F, I, L, N, P, T, U, V, W, X, Y and Z, the letter it looks like. Each is drawn as
// it usually is. Six of them (F, L, N, P, Y, Z) differ from their mirror images, so there are 18
// one-sided pentominoes; turned over, the twelve make all 63 fixed ones. Each preset's cells are
// fixed here, row by row, and the tests hold each one to its drawing. The table is built from the
// named pieces.
/// <summary>The twelve free pentominoes, each a named value, and the set of them.</summary>
public static class Pentominoes
{
    /// <summary>
    /// Gets the F: a column of three, with one square right of the top and one left of the middle.
    /// </summary>
    public static Polyform<GridStep, SquareCells> F { get; } =
        Piece([new(0, 1), new(0, 2), new(1, 0), new(1, 1), new(2, 1)]);

    /// <summary>Gets the I: five squares in a column.</summary>
    public static Polyform<GridStep, SquareCells> I { get; } =
        Piece([new(0, 0), new(1, 0), new(2, 0), new(3, 0), new(4, 0)]);

    /// <summary>Gets the L: four squares in a column, and one right of the bottom.</summary>
    public static Polyform<GridStep, SquareCells> L { get; } =
        Piece([new(0, 0), new(1, 0), new(2, 0), new(3, 0), new(3, 1)]);

    /// <summary>
    /// Gets the N: two squares in one column, and three in the column left of it, one lower.
    /// </summary>
    public static Polyform<GridStep, SquareCells> N { get; } =
        Piece([new(0, 1), new(1, 0), new(1, 1), new(2, 0), new(3, 0)]);

    /// <summary>Gets the P: a square of two by two, and one below its left column.</summary>
    public static Polyform<GridStep, SquareCells> P { get; } =
        Piece([new(0, 0), new(0, 1), new(1, 0), new(1, 1), new(2, 0)]);

    /// <summary>Gets the T: three squares in a row, and two below the middle.</summary>
    public static Polyform<GridStep, SquareCells> T { get; } =
        Piece([new(0, 0), new(0, 1), new(0, 2), new(1, 1), new(2, 1)]);

    /// <summary>Gets the U: three squares in a row, and one above each end.</summary>
    public static Polyform<GridStep, SquareCells> U { get; } =
        Piece([new(0, 0), new(0, 2), new(1, 0), new(1, 1), new(1, 2)]);

    /// <summary>Gets the V: two arms of three squares meeting at a corner.</summary>
    public static Polyform<GridStep, SquareCells> V { get; } =
        Piece([new(0, 0), new(1, 0), new(2, 0), new(2, 1), new(2, 2)]);

    /// <summary>Gets the W: a staircase of three steps.</summary>
    public static Polyform<GridStep, SquareCells> W { get; } =
        Piece([new(0, 0), new(1, 0), new(1, 1), new(2, 1), new(2, 2)]);

    /// <summary>Gets the X: a square and the four squares sharing its edges, a plus sign.</summary>
    public static Polyform<GridStep, SquareCells> X { get; } =
        Piece([new(0, 1), new(1, 0), new(1, 1), new(1, 2), new(2, 1)]);

    /// <summary>Gets the Y: four squares in a column, and one left of the second.</summary>
    public static Polyform<GridStep, SquareCells> Y { get; } =
        Piece([new(0, 1), new(1, 0), new(1, 1), new(2, 1), new(3, 1)]);

    /// <summary>
    /// Gets the Z: a column of three, with one square left of the top and one right of the bottom.
    /// </summary>
    public static Polyform<GridStep, SquareCells> Z { get; } =
        Piece([new(0, 0), new(0, 1), new(1, 1), new(2, 1), new(2, 2)]);

    /// <summary>Gets the twelve free pentominoes, in their letters' alphabetical order.</summary>
    public static ImmutableArray<Polyform<GridStep, SquareCells>> Free { get; } =
        [F, I, L, N, P, T, U, V, W, X, Y, Z];

    private static Polyform<GridStep, SquareCells> Piece(ReadOnlySpan<GridStep> squares) =>
        Polyform<GridStep, SquareCells>.Of(squares);
}
