namespace DSAExperimentation.Tests.LeetCodeCoverage.SpiralMatrixIII;

// LeetCode 885. Spiral Matrix III: walk outward from (rStart, cStart) with
// growing step lengths (1,1,2,2,3,3,...) cycling right/down/left/up,
// recording only cells that land inside the rows x cols grid, until every
// cell has been visited once. A direction-vector index-arithmetic walk -
// the same shape Spiral Matrix/Spiral Matrix II already use - GridChildren/
// GridTopology model unordered orthogonal adjacency for graph walks, not a
// fixed clockwise turning order with growing strides, so forcing this
// through Grid/** would not be a genuine fit.
public sealed partial class SpiralMatrixIIITests
{
    [Fact]
    public void SpiralMatrixIII_SingleRow_WalksRightThenBack()
    {
        var result = SpiralWalk(rows: 1, cols: 4, rStart: 0, cStart: 0);

        Assert.Equal([[0, 0], [0, 1], [0, 2], [0, 3]], result);
    }

    [Fact]
    public void SpiralMatrixIII_FromInteriorCell_VisitsEveryCellExactlyOnce()
    {
        var result = SpiralWalk(rows: 5, cols: 6, rStart: 1, cStart: 4);

        Assert.Equal(30, result.Length);
        Assert.Equal([1, 4], result[0]);
        Assert.Equal(30, result.Select(cell => (cell[0], cell[1])).Distinct().Count());
        Assert.All(result, cell => Assert.InRange(cell[0], 0, 4));
        Assert.All(result, cell => Assert.InRange(cell[1], 0, 5));
    }

    private static int[][] SpiralWalk(int rows, int cols, int rStart, int cStart)
    {
        var total = rows * cols;
        var result = new List<int[]>();
        result.Add([rStart, cStart]);

        if (total == 1)
        {
            return result.ToArray();
        }

        int[] deltaRow = [0, 1, 0, -1];
        int[] deltaCol = [1, 0, -1, 0];
        var row = rStart;
        var col = cStart;
        var step = 1;
        var direction = 0;

        while (result.Count < total)
        {
            for (var turn = 0; turn < 2; turn++)
            {
                for (var i = 0; i < step; i++)
                {
                    row += deltaRow[direction];
                    col += deltaCol[direction];

                    if (row >= 0 && row < rows && col >= 0 && col < cols)
                    {
                        result.Add([row, col]);
                        if (result.Count == total)
                        {
                            return result.ToArray();
                        }
                    }
                }

                direction = (direction + 1) % 4;
            }

            step++;
        }

        return result.ToArray();
    }
}
