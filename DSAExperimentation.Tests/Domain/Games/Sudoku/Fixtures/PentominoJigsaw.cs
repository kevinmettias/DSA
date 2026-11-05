using DSAExperimentation.Domain.Games.Sudoku;
using DSAExperimentation.Domain.Games.Sudoku.Regions;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku.Fixtures;

// A 5x5 jigsaw Sudoku, test-only: five regions, each a connected pentomino, numbered row by row in
// the map below. Five is prime, so no box tiles a 5x5 board, and only a drawn map can split it.
internal readonly struct PentominoJigsaw : ISudokuGrid, IRegionMap<PentominoJigsaw>
{
    public static int Order => 5;

    public static char EmptyCell => '.';

    public static ReadOnlySpan<byte> RegionOfCell =>
    [
        0, 0, 0, 1, 1,
        0, 2, 0, 1, 1,
        2, 2, 2, 3, 1,
        4, 2, 3, 3, 3,
        4, 4, 4, 4, 3,
    ];
}
