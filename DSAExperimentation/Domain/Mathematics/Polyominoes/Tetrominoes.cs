using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Mathematics.Polyominoes;

// The tetrominoes, polyominoes of four squares, each a named value under the letter it looks like.
// Golomb named the five free ones I, O, T, S and L. Turned over, S is Z and L is J, which Tetris
// keeps as pieces of their own, since its pieces turn but are never turned over: its seven are the
// one-sided tetrominoes. Each piece is drawn in the orientation it enters a Tetris well in, in the
// Tetris Guideline (an L with its foot up on the right, a J with it up on the left), so J is L
// mirrored exactly and Z is S mirrored. How a piece turns in the well, about which centre and with
// which kicks off the walls, is Tetris's own rule and is not stated here. Each preset's cells are
// fixed here, row by row, and the tests hold each one to its drawing. The tables are built from
// the named pieces.
/// <summary>The tetrominoes, each a named value, and the free and one-sided sets.</summary>
public static class Tetrominoes
{
    /// <summary>Gets the I: four squares in a row.</summary>
    public static Polyform<GridStep, SquareCells> I { get; } =
        Piece([new(0, 0), new(0, 1), new(0, 2), new(0, 3)]);

    /// <summary>Gets the O: a square of two by two.</summary>
    public static Polyform<GridStep, SquareCells> O { get; } =
        Piece([new(0, 0), new(0, 1), new(1, 0), new(1, 1)]);

    /// <summary>Gets the T: three squares in a row, and one above the middle.</summary>
    public static Polyform<GridStep, SquareCells> T { get; } =
        Piece([new(0, 1), new(1, 0), new(1, 1), new(1, 2)]);

    /// <summary>Gets the S: two squares on top, and two below them a square to the left.</summary>
    public static Polyform<GridStep, SquareCells> S { get; } =
        Piece([new(0, 1), new(0, 2), new(1, 0), new(1, 1)]);

    /// <summary>
    /// Gets the Z, the S turned over: two squares on top, and two below them a square to the right.
    /// </summary>
    public static Polyform<GridStep, SquareCells> Z { get; } =
        Piece([new(0, 0), new(0, 1), new(1, 1), new(1, 2)]);

    /// <summary>
    /// Gets the J, the L turned over: three squares in a row, and one above the left end.
    /// </summary>
    public static Polyform<GridStep, SquareCells> J { get; } =
        Piece([new(0, 0), new(1, 0), new(1, 1), new(1, 2)]);

    /// <summary>Gets the L: three squares in a row, and one above the right end.</summary>
    public static Polyform<GridStep, SquareCells> L { get; } =
        Piece([new(0, 2), new(1, 0), new(1, 1), new(1, 2)]);

    /// <summary>Gets the five free tetrominoes, Golomb's I, O, T, S and L.</summary>
    public static ImmutableArray<Polyform<GridStep, SquareCells>> Free { get; } = [I, O, T, S, L];

    /// <summary>Gets the seven one-sided tetrominoes, Tetris's I, O, T, S, Z, J and L.</summary>
    public static ImmutableArray<Polyform<GridStep, SquareCells>> OneSided { get; } =
        [I, O, T, S, Z, J, L];

    private static Polyform<GridStep, SquareCells> Piece(ReadOnlySpan<GridStep> squares) =>
        Polyform<GridStep, SquareCells>.Of(squares);
}
