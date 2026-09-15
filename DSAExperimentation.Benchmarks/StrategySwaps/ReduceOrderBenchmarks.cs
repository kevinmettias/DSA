using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// Compares this repo's own IReduceOrderStrategy implementations (BreadthFirst vs
// DepthFirst) on a balanced tree with DistanceMapReduceAlgebra - a tree's unique
// ancestry makes each node's depth structurally fixed regardless of visit order,
// so (unlike most IReduceAlgebra choices, per that interface's own doc comment)
// this specific algebra is order-insensitive: same result either way, so the
// comparison is purely about traversal mechanics (explicit queue vs recursion).
[MemoryDiagnoser]
public class ReduceOrderBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(10_000, 200_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int BreadthFirst() => RunReduce<BreadthFirstReduceOrder<BinaryTreeNode<int>>>();

    [Benchmark]
    public int DepthFirst() => RunReduce<DepthFirstReduceOrder<BinaryTreeNode<int>>>();

    private int RunReduce<TOrderStrategy>()
        where TOrderStrategy : struct, IReduceOrderStrategy<BinaryTreeNode<int>>
        => Reduce.Tree<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            TOrderStrategy, DistanceMapReduceAlgebra<BinaryTreeNode<int>>, Dictionary<BinaryTreeNode<int>, int>>(
            _root).Count;
}
