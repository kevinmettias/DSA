using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.Algorithms.DynamicProgramming.Fixtures;

// The grid rule, named: a cell already on the last row or column is one path from the
// end, and any other cell is the paths below it plus the paths to its right. The two
// dimensions arrive as constructor parameters rather than as constants the test and the
// fixture both reach for, so the fixture states the rule and the test states the grid.
internal sealed class DownAndRightPaths(int rows, int cols) : IRecurrence<(int Row, int Col), long>
{
    public long Replay((int Row, int Col) state, IRecurrence<(int Row, int Col), long> rest)
    {
        var (row, col) = state;

        if (row == rows - 1 || col == cols - 1)
        {
            return 1;
        }

        return rest.Replay((row + 1, col), rest) + rest.Replay((row, col + 1), rest);
    }
}
