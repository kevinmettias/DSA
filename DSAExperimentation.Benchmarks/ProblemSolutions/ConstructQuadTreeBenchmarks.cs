using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Construct Quad Tree (LC 427): confirming a region is uniform is the hot path - a brute-force
// cell scan (early-exits on the first mismatch, but must touch every cell to CONFIRM a
// genuinely uniform one) vs. one FenwickTree<int,SumOperation<int>> per row (the same
// RangeSumQuery2DImmutableBenchmarks move, LC 304) giving an O(rows*log(cols)) region-sum
// check instead: uniform exactly when the sum is 0 or the full area.
//
// Measured here, the Fenwick approach does NOT win, and this is a real finding, not a badly
// chosen input - confirmed across several fragmentation patterns and grid sizes with a
// throwaway probe before settling on this file. RangeSumQuery2DImmutableBenchmarks amortizes
// its one O(rows*cols*log(cols)) row-Fenwick build over an independently large, caller-chosen
// QueryCount; a quad tree's own recursion instead issues at most O(cells) confirmation calls
// total, tightly coupled to the same grid size the build cost scales with - never enough
// distinct queries to pay back a build that already costs as much as brute force's own
// full-region confirmation. ARCHITECTURE.md §8 names this risk as "performance independence" -
// a syntactic Query(left,right) contract says nothing about whether its caller supplies enough
// queries to amortize the structure's own build cost; here it measurably doesn't. The grid is
// split top/bottom (half 0, half 1) so all four top-level quadrants are genuinely uniform and
// must be fully confirmed either way - not chosen to be adversarial to either approach.
[MemoryDiagnoser]
public class ConstructQuadTreeBenchmarks
{
    [Params(16, 128)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        _grid = new int[Size][];

        for (var row = 0; row < Size; row++)
        {
            _grid[row] = new int[Size];
            var value = row < Size / 2 ? 0 : 1;

            for (var col = 0; col < Size; col++)
            {
                _grid[row][col] = value;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BruteForceCellScan() => CountLeaves(0, 0, Size);

    [Benchmark]
    public int RowFenwickTreeQuery()
    {
        var rows = _grid.Select(row => new FenwickTree<int, SumOperation<int>>(row)).ToArray();
        return CountLeaves(rows, 0, 0, Size);
    }

    private int CountLeaves(int row, int col, int size)
    {
        var first = _grid[row][col];
        var uniform = true;

        for (var r = row; r < row + size && uniform; r++)
        {
            for (var c = col; c < col + size; c++)
            {
                if (_grid[r][c] != first)
                {
                    uniform = false;
                    break;
                }
            }
        }

        if (uniform)
        {
            return 1;
        }

        var half = size / 2;
        return CountLeaves(row, col, half)
             + CountLeaves(row, col + half, half)
             + CountLeaves(row + half, col, half)
             + CountLeaves(row + half, col + half, half);
    }

    private static int CountLeaves(FenwickTree<int, SumOperation<int>>[] rows, int row, int col, int size)
    {
        var sum = 0;
        for (var r = row; r < row + size; r++)
        {
            sum += rows[r].Query(col, col + size - 1);
        }

        if (sum == 0 || sum == size * size)
        {
            return 1;
        }

        var half = size / 2;
        return CountLeaves(rows, row, col, half)
             + CountLeaves(rows, row, col + half, half)
             + CountLeaves(rows, row + half, col, half)
             + CountLeaves(rows, row + half, col + half, half);
    }
}
