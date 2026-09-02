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
        => new SpiralWalker(rows, cols, rStart, cStart).Run();

    // Walk state for SpiralWalk, extracted so the growing-stride clockwise walk
    // (while total not reached -> two turns per stride length -> step cells per
    // turn -> in-bounds check) reads as one level of nesting per method instead
    // of one combined function nesting four loops deep.
    private sealed class SpiralWalker(int rows, int cols, int rStart, int cStart)
    {
        private static readonly int[] DeltaRow = [0, 1, 0, -1];
        private static readonly int[] DeltaCol = [1, 0, -1, 0];

        private readonly int _total = rows * cols;
        private readonly List<int[]> _result = [[rStart, cStart]];
        private int _row = rStart;
        private int _col = cStart;
        private int _direction;
        private int _step = 1;

        public int[][] Run()
        {
            if (_total == 1)
            {
                return _result.ToArray();
            }

            while (_result.Count < _total)
            {
                if (RunTurns())
                {
                    return _result.ToArray();
                }

                _step++;
            }

            return _result.ToArray();
        }

        private bool RunTurns()
        {
            for (var turn = 0; turn < 2; turn++)
            {
                if (AdvanceRun())
                {
                    return true;
                }

                _direction = (_direction + 1) % 4;
            }

            return false;
        }

        private bool AdvanceRun()
        {
            for (var i = 0; i < _step; i++)
            {
                _row += DeltaRow[_direction];
                _col += DeltaCol[_direction];

                if (!IsInBounds())
                {
                    continue;
                }

                _result.Add([_row, _col]);
                if (_result.Count == _total)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsInBounds() => _row >= 0 && _row < rows && _col >= 0 && _col < cols;
    }
}
