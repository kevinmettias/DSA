using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class PathSumIIBenchmarks
{
    private BinaryTreeNode<int> _root=null!; [GlobalSetup] public void Setup()=>_root=new(5){Left=new(4){Left=new(11){Left=new(7),Right=new(2)}},Right=new(8){Left=new(13),Right=new(4){Left=new(5),Right=new(1)}}};
    [Benchmark(Baseline=true)] public int RecursiveCollect(){var count=0;void Search(BinaryTreeNode<int>? n,int sum){if(n is null)return;sum+=n.Value;if(n.Left is null&&n.Right is null&&sum==22)count++;Search(n.Left,sum);Search(n.Right,sum);}Search(_root,0);return count;}
    [Benchmark] public int AllRootToLeafPathsCollect()=>AllRootToLeafPaths.Find<BinaryTreeNode<int>,BinaryTreeTopology<int>,BinaryTreeChildren<int>,NaturalChildOrder<BinaryTreeNode<int>,BinaryTreeChildren<int>>,BinaryTreeChildren<int>>(_root).Count(p=>p.Sum(n=>n.Value)==22);
}
