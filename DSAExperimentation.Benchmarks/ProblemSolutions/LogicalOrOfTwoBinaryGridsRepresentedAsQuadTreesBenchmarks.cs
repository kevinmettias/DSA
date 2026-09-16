using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructQuadTree;
using DSAExperimentation.LeetCode.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are
// LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution's, the same methods
// LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesTests proves correct. [Benchmark]
// methods return object? rather than the internal QuadTreeNode - the same
// accommodation ConstructQuadTreeBenchmarks and friends make (a public [Benchmark]
// method cannot expose an internal return type even to a friend assembly, CS0050).
// grid1 splits top/bottom, grid2 splits left/right, so every top-level quadrant of
// the OR result is genuinely mixed and neither input tree nor the merged grid
// collapses trivially at the root - this is what lets DirectRecursiveMerge's
// output-sensitive cost (tracking the combined input node count, not the grid area)
// show a growing margin over the two materializing arms as Size grows while the two
// trees stay coarse.
[MemoryDiagnoser]
public class LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarks
{
    private QuadTreeNode _tree1 = null!;

    private QuadTreeNode _tree2 = null!;
    [Params(16, 128)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var grid1 = new int[Size][];
        var grid2 = new int[Size][];

        for (var row = 0; row < Size; row++)
        {
            grid1[row] = new int[Size];
            grid2[row] = new int[Size];

            for (var col = 0; col < Size; col++)
            {
                // grid1 splits top/bottom, grid2 splits left/right - every top-level
                // quadrant of the OR result is genuinely mixed, so neither input tree
                // nor the merged grid collapses trivially at the root.
                grid1[row][col] = IsTopHalf(row) ? 0 : 1;
                grid2[row][col] = IsLeftHalf(col) ? 0 : 1;
            }
        }

        _tree1 = ConstructQuadTreeSolution.BuildByBruteForceCellScan(grid1);
        _tree2 = ConstructQuadTreeSolution.BuildByBruteForceCellScan(grid2);
    }

    // A quad-tree region splits into 2x2 quadrants, so each dimension halves at every recursion
    // level - AlgorithmConstants.HalvingFactor, the shared divisor every middle-splitting call site
    // in the repo reads rather than holding a private copy of 2 under a name of its own.
    private bool IsTopHalf(int row) => row < Size / AlgorithmConstants.HalvingFactor;

    private bool IsLeftHalf(int col) => col < Size / AlgorithmConstants.HalvingFactor;

    [Benchmark(Baseline = true)]
    public object? BruteForceGridMaterialize() =>
        LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByBruteForceGridMaterialize(_tree1, _tree2, Size);

    [Benchmark]
    public object? FenwickGridMaterialize() =>
        LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByFenwickGridMaterialize(_tree1, _tree2, Size);

    [Benchmark]
    public object? DirectRecursiveMerge() =>
        LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByDirectRecursiveMerge(_tree1, _tree2);
}
