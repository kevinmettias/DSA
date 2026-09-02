using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class BalancedBinaryTreeBenchmarks
{
    private const int RootValue = 3;
    private const int LeftValue = 9;
    private const int RightValue = 20;
    private const int RightLeftValue = 15;
    private const int RightRightValue = 7;

    private BinaryTreeNode<int> _root=null!; [GlobalSetup] public void Setup()=>_root=new(RootValue){Left=new(LeftValue),Right=new(RightValue){Left=new(RightLeftValue),Right=new(RightRightValue)}};
    [Benchmark(Baseline=true)] public bool HeightCheck()=>HeightOrUnbalanced(_root)>=0;
    [Benchmark] public bool BinaryTreeNodeCheck()=>HeightOrUnbalanced(_root)>=0;
    private static int HeightOrUnbalanced(BinaryTreeNode<int>? n){if(n is null)return 0;var l=HeightOrUnbalanced(n.Left);var r=HeightOrUnbalanced(n.Right);if(l<0||r<0||Math.Abs(l-r)>1)return -1;return 1+Math.Max(l,r);}
}
