using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Strictly Increasing Cells in a Matrix (LC 2713): MemoizedRowColumnScan is the
// textbook DAG-DP baseline - dp(row,col) recurses into every strictly-smaller cell in
// its own row and column, memoized per cell - O(rows*cols*(rows+cols)) since each of
// the rows*cols cells rescans a whole row plus a whole column. SortedBatchDp is
// MaximumStrictlyIncreasingCellsInAMatrixTests' approach: MergeSort every cell by value
// once, then fold each equal-value batch's dp into running rowBest/colBest arrays -
// O(rows*cols*log(rows*cols)) total, with no per-cell row/column rescan at all.
[MemoryDiagnoser]
public class MaximumStrictlyIncreasingCellsInAMatrixBenchmarks
{
    private const int RandomSeed = 2713; // LC problem number
    private const int ValueRange = 1_000;

    [Params(8, 20)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _matrix = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _matrix[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _matrix[r][c] = random.Next(1, ValueRange);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int MemoizedRowColumnScan()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var memo = new int[rows, cols];
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                best = Math.Max(best, ComputeBest(r, c, memo));
            }
        }

        return best;
    }

    private int ComputeBest(int row, int col, int[,] memo)
    {
        if (memo[row, col] != 0)
        {
            return memo[row, col];
        }

        var best = 1;

        for (var c = 0; c < _matrix[0].Length; c++)
        {
            if (_matrix[row][c] < _matrix[row][col])
            {
                best = Math.Max(best, 1 + ComputeBest(row, c, memo));
            }
        }

        for (var r = 0; r < _matrix.Length; r++)
        {
            if (_matrix[r][col] < _matrix[row][col])
            {
                best = Math.Max(best, 1 + ComputeBest(r, col, memo));
            }
        }

        memo[row, col] = best;
        return best;
    }

    [Benchmark]
    public int SortedBatchDp()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var cells = BuildCells(rows, cols);

        SortCellsByValue(cells);

        var rowBest = new int[rows];
        var colBest = new int[cols];
        var index = 0;

        while (index < cells.Length)
        {
            index = ProcessBatch(cells, index, rowBest, colBest);
        }

        return Math.Max(rowBest.Max(), colBest.Max());
    }

    private (int Value, int Row, int Col)[] BuildCells(int rows, int cols)
    {
        var cells = new (int Value, int Row, int Col)[rows * cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                cells[(r * cols) + c] = (_matrix[r][c], r, c);
            }
        }

        return cells;
    }

    private static void SortCellsByValue((int Value, int Row, int Col)[] cells)
    {
        var byValue = Comparer<(int Value, int Row, int Col)>.Create((a, b) => a.Value.CompareTo(b.Value));
        MergeSort.Sort<(int Value, int Row, int Col), ArrayIndexedSequence<(int Value, int Row, int Col)>>(
            new ArrayIndexedSequence<(int Value, int Row, int Col)>(cells), byValue);
    }

    private static int ProcessBatch((int Value, int Row, int Col)[] cells, int index, int[] rowBest, int[] colBest)
    {
        var end = index;

        while (end < cells.Length && cells[end].Value == cells[index].Value)
        {
            end++;
        }

        var dp = new int[end - index];

        for (var i = index; i < end; i++)
        {
            var (_, row, col) = cells[i];
            dp[i - index] = 1 + Math.Max(rowBest[row], colBest[col]);
        }

        for (var i = index; i < end; i++)
        {
            var (_, row, col) = cells[i];
            rowBest[row] = Math.Max(rowBest[row], dp[i - index]);
            colBest[col] = Math.Max(colBest[col], dp[i - index]);
        }

        return end;
    }
}
