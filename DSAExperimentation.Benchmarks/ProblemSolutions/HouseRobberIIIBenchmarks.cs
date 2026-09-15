using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.HouseRobberIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HouseRobberIIISolution's, the same methods
// HouseRobberIIITests proves correct - a hand-rolled post-order recursion vs. this
// repo's generic TreeFold engine closed over RobFoldAlgebra, over a random full
// binary tree of the given depth.
[MemoryDiagnoser]
public class HouseRobberIIIBenchmarks
{
    private const int Seed = 1;

    private BinaryTreeNode<int> _root = null!;

    [Params(10, 15)]
    public int Depth { get; set; }

    [GlobalSetup]
    public void Setup() => _root = HouseRobberIIIWorkloads.RandomFullTree(Depth, Seed);

    [Benchmark(Baseline = true)]
    public int RecursivePair() => HouseRobberIIISolution.RobByRecursivePair(_root);

    [Benchmark]
    public int TreeFoldAlgebra() => HouseRobberIIISolution.RobByTreeFoldAlgebra(_root);
}
