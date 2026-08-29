using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class ValidateBinarySearchTreeBenchmarks
{
    private BinaryTreeNode<int> _root=null!; [GlobalSetup] public void Setup()=>_root=new BinaryTreeNode<int>(2){Left=new(1),Right=new(3)};
    [Benchmark(Baseline=true)] public bool RecursiveBounds()=>Validate(_root,null,null);
    [Benchmark] public bool BinaryTreeNodeBounds()=>Validate(_root,null,null);
    private static bool Validate(BinaryTreeNode<int>? n,int? min,int? max)=>n is null||((min is null||n.Value>min)&&(max is null||n.Value<max)&&Validate(n.Left,min,n.Value)&&Validate(n.Right,n.Value,max));
}
