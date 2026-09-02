using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class SameTreeBenchmarks
{
    private const int LeftChildValue = 2;
    private const int RightChildValue = 3;

    private BinaryTreeNode<int> _a=null!; private BinaryTreeNode<int> _b=null!; [GlobalSetup] public void Setup(){_a=Tree();_b=Tree();}
    [Benchmark(Baseline=true)] public bool RecursiveCompare()=>Same(_a,_b);
    [Benchmark] public bool BinaryTreeNodeCompare()=>Same(_a,_b);
    private static bool Same(BinaryTreeNode<int>? p,BinaryTreeNode<int>? q)=>p is null||q is null?p is null&&q is null:p.Value==q.Value&&Same(p.Left,q.Left)&&Same(p.Right,q.Right);
    private static BinaryTreeNode<int> Tree()=>new(1){Left=new(LeftChildValue),Right=new(RightChildValue)};
}
