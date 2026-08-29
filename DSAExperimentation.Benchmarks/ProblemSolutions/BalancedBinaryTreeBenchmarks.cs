using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class BalancedBinaryTreeBenchmarks
{
    private BinaryTreeNode<int> _root=null!; [GlobalSetup] public void Setup()=>_root=new(3){Left=new(9),Right=new(20){Left=new(15),Right=new(7)}};
    [Benchmark(Baseline=true)] public bool HeightCheck()=>HeightOrUnbalanced(_root)>=0;
    [Benchmark] public bool BinaryTreeNodeCheck()=>HeightOrUnbalanced(_root)>=0;
    private static int HeightOrUnbalanced(BinaryTreeNode<int>? n){if(n is null)return 0;var l=HeightOrUnbalanced(n.Left);var r=HeightOrUnbalanced(n.Right);if(l<0||r<0||Math.Abs(l-r)>1)return -1;return 1+Math.Max(l,r);}
}
