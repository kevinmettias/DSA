using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LastDayWhereYouCanStillCross;

// LeetCode 1970. Last Day Where You Can Still Cross: crossability is monotone in
// day - once flooding disconnects the top row from the bottom row, flooding more
// cells never reconnects them - so "is day d blocked" is the same monotone
// [false...false, true...true] sequence this repo's own BinarySearch.LowerBound
// already locates in KokoEatingBananasTests/SplitArrayLargestSumTests. Here the
// per-day feasibility check is DepthFirstSearch.Traverse (NumberOfIslandsTests'
// own grid-neighbors idiom) from a virtual node above the grid connected to every
// still-dry top-row cell, testing whether any bottom-row cell is reachable.
public sealed partial class LastDayWhereYouCanStillCrossTests
{
    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Theory]
    [MemberData(nameof(Examples))]
    public void LatestDayToCross_LeetCodeExamples_ReturnsLastCrossableDay(int row, int col, int[][] cells, int expected)
    {
        var actual = LatestDayToCross(row, col, cells);
        Assert.Equal(expected, actual);
    }

    public static IEnumerable<object[]> Examples()
    {
        yield return [2, 2, new[] { new[] { 1, 1 }, new[] { 2, 1 }, new[] { 1, 2 }, new[] { 2, 2 } }, 2];
        yield return [2, 2, new[] { new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 1 }, new[] { 2, 2 } }, 1];
        yield return
        [
            3, 3,
            new[]
            {
                new[] { 1, 2 }, new[] { 2, 1 }, new[] { 3, 3 }, new[] { 2, 2 }, new[] { 1, 1 },
                new[] { 1, 3 }, new[] { 2, 3 }, new[] { 3, 2 }, new[] { 3, 1 },
            },
            3,
        ];
    }

    private static int LatestDayToCross(int row, int col, int[][] cells)
    {
        var floodDay = new int[row, col];

        for (var i = 0; i < cells.Length; i++)
        {
            floodDay[cells[i][0] - 1, cells[i][1] - 1] = i + 1;
        }

        var sequence = new IsBlockedSequence(row, col, floodDay);
        return BinarySearch.LowerBound(sequence, true) - 1;
    }

    // (-1, -1) is a virtual node above the grid, wired to every top-row cell that is
    // still land on this day - collapsing "start a search from each dry top-row cell"
    // into a single Traverse call instead of one per column.
    private static bool CanCross(int row, int col, int[,] floodDay, int day)
        => DepthFirstSearch.Traverse((Row: -1, Col: -1), cell => Neighbors(cell, new GridBounds(row, col), floodDay, day))
            .Any(cell => cell.Row == row - 1);

    private static IEnumerable<(int Row, int Col)> Neighbors(
        (int Row, int Col) cell, GridBounds bounds, int[,] floodDay, int day)
    {
        if (cell.Row == -1)
        {
            for (var c = 0; c < bounds.Cols; c++)
            {
                if (floodDay[0, c] > day)
                {
                    yield return (0, c);
                }
            }

            yield break;
        }

        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: cell.Row + dRow, Col: cell.Col + dCol);

            if (next.Row >= 0 && next.Row < bounds.Rows && next.Col >= 0 && next.Col < bounds.Cols
                && floodDay[next.Row, next.Col] > day)
            {
                yield return next;
            }
        }
    }

    private readonly record struct GridBounds(int Rows, int Cols);

    private readonly struct IsBlockedSequence(int row, int col, int[,] floodDay) : IRandomAccessSequence<bool>
    {
        public int Length => (row * col) + 1;

        public bool Get(int index) => !CanCross(row, col, floodDay, index);
    }
}
