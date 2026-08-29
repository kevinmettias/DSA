using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class PathSumBenchmarks
{
    private BinaryTreeNode<int> _root=null!; [GlobalSetup] public void Setup()=>_root=Tree();
    [Benchmark(Baseline=true)] public bool RecursivePathSum()=>Has(_root,22);
    [Benchmark] public bool AllRootToLeafPathsSum()=>AllRootToLeafPaths.Find<BinaryTreeNode<int>,BinaryTreeTopology<int>,BinaryTreeChildren<int>,NaturalChildOrder<BinaryTreeNode<int>,BinaryTreeChildren<int>>,BinaryTreeChildren<int>>(_root).Any(p=>p.Sum(n=>n.Value)==22);
    private static bool Has(BinaryTreeNode<int>? n,int t)=>n is not null&& (n.Left is null&&n.Right is null?n.Value==t:Has(n.Left,t-n.Value)||Has(n.Right,t-n.Value));
    private static BinaryTreeNode<int> Tree()=>new(5){Left=new(4){Left=new(11){Left=new(7),Right=new(2)}},Right=new(8){Left=new(13),Right=new(4){Right=new(1)}}};
}
