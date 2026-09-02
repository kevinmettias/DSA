using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class BinaryTreeLevelOrderTraversalIIBenchmarks
{
    private const int RootValue = 3;
    private const int LeftValue = 9;
    private const int RightValue = 20;
    private const int RightLeftValue = 15;
    private const int RightRightValue = 7;

    private BinaryTreeNode<int> _root = null!; [GlobalSetup] public void Setup()=>_root=new BinaryTreeNode<int>(RootValue){Left=new(LeftValue),Right=new(RightValue){Left=new(RightLeftValue),Right=new(RightRightValue)}};
    [Benchmark(Baseline=true)] public int QueueThenReverse(){var levels=new List<int>();var q=new Queue<BinaryTreeNode<int>>();q.Enqueue(_root);while(q.Count>0){levels.Add(q.Count);for(var i=q.Count;i>0;i--){var n=q.Dequeue();if(n.Left is not null)q.Enqueue(n.Left);if(n.Right is not null)q.Enqueue(n.Right);}}levels.Reverse();return levels.Sum();}
    [Benchmark] public int LevelGroupedThenReverse(){LevelHooks.Counts.Value=[];LevelGroupedBreadthFirstTraversal.Walk<BinaryTreeNode<int>,BinaryTreeTopology<int>,BinaryTreeChildren<int>,NaturalChildOrder<BinaryTreeNode<int>,BinaryTreeChildren<int>>,BinaryTreeChildren<int>,LevelHooks>(_root);var levels=LevelHooks.Counts.Value!;levels.Reverse();return levels.Sum();}
    private readonly struct LevelHooks:ILevelGroupedHooks<BinaryTreeNode<int>>{public static readonly AsyncLocal<List<int>> Counts=new();public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level,int depth)=>Counts.Value!.Add(level.Count);}
}
