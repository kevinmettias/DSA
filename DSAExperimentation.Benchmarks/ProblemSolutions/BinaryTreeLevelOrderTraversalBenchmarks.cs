using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class BinaryTreeLevelOrderTraversalBenchmarks
{
    private BinaryTreeNode<int> _root = null!; [GlobalSetup] public void Setup()=>_root=Tree();
    [Benchmark(Baseline=true)] public int QueueLevels(){var q=new Queue<BinaryTreeNode<int>>();q.Enqueue(_root);var count=0;while(q.Count>0){var n=q.Count;for(var i=0;i<n;i++){var node=q.Dequeue();count++;if(node.Left is not null)q.Enqueue(node.Left);if(node.Right is not null)q.Enqueue(node.Right);}}return count;}
    [Benchmark] public int LevelGroupedTraversal(){LevelHooks.Count.Value=0;LevelGroupedBreadthFirstTraversal.Walk<BinaryTreeNode<int>,BinaryTreeTopology<int>,BinaryTreeChildren<int>,NaturalChildOrder<BinaryTreeNode<int>,BinaryTreeChildren<int>>,BinaryTreeChildren<int>,LevelHooks>(_root);return LevelHooks.Count.Value;}
    private readonly struct LevelHooks:ILevelGroupedHooks<BinaryTreeNode<int>>{public static readonly AsyncLocal<int> Count=new();public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level,int depth)=>Count.Value+=level.Count;}
    private static BinaryTreeNode<int> Tree()=>new(3){Left=new(9),Right=new(20){Left=new(15),Right=new(7)}};
}
