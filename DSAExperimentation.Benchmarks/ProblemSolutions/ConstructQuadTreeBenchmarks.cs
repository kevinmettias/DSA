using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructQuadTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ConstructQuadTreeSolution's, the same methods
// ConstructQuadTreeTests proves correct - each builds the actual QuadTreeNode
// tree, not just a leaf count. The [Benchmark] methods return object? rather than
// QuadTreeNode itself - a public [Benchmark] method cannot expose an internal
// return type even to a friend assembly (CS0050) - the same accommodation this
// project's other node-returning benchmarks make (ReverseLinkedList, SortList,
// and friends). Confirming a region is uniform is the hot path:
// a brute-force cell scan (early-exits on the first mismatch, but must touch
// every cell to CONFIRM a genuinely uniform one) vs. one
// FenwickTree<int,SumOperation<int>> per row giving an O(rows*log(cols)) region-
// sum check instead. The Fenwick approach's per-call win does not necessarily
// pay for itself in aggregate here: unlike RangeSumQuery2DImmutable's
// independently large, caller-chosen QueryCount, a quad tree's own recursion
// never issues enough confirmation calls to amortize the Fenwick forest's own
// O(cells*log(cols)) build cost. The grid is split top/bottom (half 0, half 1)
// so all four top-level quadrants are genuinely uniform and must be fully
// confirmed either way - not chosen to be adversarial to either approach.
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

    // Returns object? rather than the internal QuadTreeNode - the same accommodation
    // this project's other node-returning benchmarks make (ReverseLinkedList,
    // SortList, and friends) since a public [Benchmark] method cannot expose an
    // internal return type even to a friend assembly (CS0050).
    [Benchmark(Baseline = true)]
    public object? BruteForceCellScan() =>
        ConstructQuadTreeSolution.BuildByBruteForceCellScan(_grid);

    [Benchmark]
    public object? RowFenwickTreeQuery() =>
        ConstructQuadTreeSolution.BuildByRowFenwickTree(_grid);
}
