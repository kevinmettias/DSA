using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.CountGoodNodesInBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountGoodNodesInBinaryTreeSolution's, the same methods
// CountGoodNodesInBinaryTreeTests proves correct - a plain recursive DFS threading
// the running maximum through call-stack parameters against this repo's own
// TopDownTraversal threading it through ITopDownHooks.Descend. The measured tree is
// built once in [GlobalSetup] so only the counting walk is charged to either arm.
[MemoryDiagnoser]
public class CountGoodNodesInBinaryTreeBenchmarks
{
    private const int RandomSeed = 1448; // LC problem number
    private const int CompleteTreeBranchingFactor = 2;
    private const int RightChildIndexOffset = 2;

    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _root = BuildCompleteTree(NodeCount, random);
    }

    // Workload sizing only: a complete tree of the requested size whose values are
    // drawn from a fixed seed, so how many nodes turn out to be good is stable across
    // runs without being trivially all of them.
    private static BinaryTreeNode<int> BuildCompleteTree(int nodeCount, Random random)
    {
        var nodes = new BinaryTreeNode<int>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            nodes[i] = new BinaryTreeNode<int>(random.Next(0, nodeCount));
        }

        LinkChildren(nodes, nodeCount);

        return nodes[0];
    }

    // Wires each index's array-implicit children per the standard complete-binary-tree
    // indexing formula (left = 2i+1, right = 2i+2).
    private static void LinkChildren(BinaryTreeNode<int>[] nodes, int nodeCount)
    {
        for (var i = 0; i < nodeCount; i++)
        {
            var left = (CompleteTreeBranchingFactor * i) + 1;
            var right = (CompleteTreeBranchingFactor * i) + RightChildIndexOffset;

            if (left < nodeCount)
            {
                nodes[i].Left = nodes[left];
            }

            if (right < nodeCount)
            {
                nodes[i].Right = nodes[right];
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int RecursiveDfs() => CountGoodNodesInBinaryTreeSolution.CountGoodNodesByRecursiveDfs(_root);

    [Benchmark]
    public int TopDownTraversalCount() =>
        CountGoodNodesInBinaryTreeSolution.CountGoodNodesByTopDownTraversal(_root);
}
