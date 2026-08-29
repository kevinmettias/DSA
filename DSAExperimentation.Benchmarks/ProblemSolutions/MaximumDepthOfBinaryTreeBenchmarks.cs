using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class MaximumDepthOfBinaryTreeBenchmarks
{
    private BinaryTreeNode<int> _root=null!; [GlobalSetup] public void Setup()=>_root=new(3){Left=new(9),Right=new(20){Left=new(15),Right=new(7)}};
    [Benchmark(Baseline=true)] public int RecursiveHeight()=>Height(_root);
    [Benchmark] public int TreeMetricsHeight()=>TreeMetrics.Height<BinaryTreeNode<int>,BinaryTreeTopology<int>,BinaryTreeChildren<int>,NaturalChildOrder<BinaryTreeNode<int>,BinaryTreeChildren<int>>,BinaryTreeChildren<int>>(_root);
    private static int Height(BinaryTreeNode<int>? n)=>n is null?0:1+Math.Max(Height(n.Left),Height(n.Right));
}
