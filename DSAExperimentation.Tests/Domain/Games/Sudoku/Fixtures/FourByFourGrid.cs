using DSAExperimentation.Domain.Games.Sudoku;
using DSAExperimentation.Domain.Games.Sudoku.Regions;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku.Fixtures;

// The 4x4 children's Sudoku, test-only: boxes of 2x2 on a board four a side, the rule and the boxes
// on an order no preset takes.
internal readonly struct FourByFourGrid : ISudokuGrid, IBoxPartition<FourByFourGrid>
{
    public static int BoxRows => 2;

    public static int BoxColumns => 2;

    public static char EmptyCell => '.';
}
