using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeMaximumPathSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is BinaryTreeMaximumPathSumSolution's, the same
// method BinaryTreeMaximumPathSumTests proves correct. Pre-migration this class
// was an untested compile-smoke placeholder (`Baseline() => 1`,
// `PrimitiveComposed() => 1`) rather than a second strategy to reconcile.
[MemoryDiagnoser]
public class BinaryTreeMaximumPathSumBenchmarks
{
    // LeetCode 124's own second example tree: the best path (15 -> 20 -> 7)
    // bends through both children of a node that is not the root, so it
    // exercises the running-best branch rather than only the return value.
    private const int RootValue = -10;
    private const int LeftValue = 9;
    private const int RightValue = 20;
    private const int RightLeftValue = 15;
    private const int RightRightValue = 7;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() =>
        _root = new(RootValue)
        {
            Left = new(LeftValue),
            Right = new(RightValue) { Left = new(RightLeftValue), Right = new(RightRightValue) },
        };

    [Benchmark(Baseline = true)]
    public int GainRecursion() => BinaryTreeMaximumPathSumSolution.MaxPathSumByGainRecursion(_root);
}
