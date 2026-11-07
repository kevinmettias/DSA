using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Fixtures;

// A set of squares drawn as text, the way a book draws a polyomino: one string per row, '#' for a
// square and any other character for none. A preset is held to its drawing, so a test reads as the
// picture of the piece.
internal static class Drawing
{
    private const char Square = '#';

    // The squares drawn, row by row, the top left character at the origin.
    public static GridStep[] Squares(params string[] rows) =>
        [.. rows.SelectMany((line, row) => line
            .Select((mark, col) => (Mark: mark, Square: new GridStep(row, col)))
            .Where(drawn => drawn.Mark == Square)
            .Select(drawn => drawn.Square))];

    // The polyomino drawn.
    public static Polyform<GridStep, SquareCells> Polyomino(params string[] rows) =>
        Polyform<GridStep, SquareCells>.Of(Squares(rows));
}
