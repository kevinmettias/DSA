using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ValidateBinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is ValidateBinarySearchTreeSolution's, the same
// method ValidateBinarySearchTreeSolutionTests proves correct. The previous class
// carried RecursiveBounds and BinaryTreeNodeBounds as two [Benchmark] arms
// that both called the same private IsWithinBounds helper - one strategy under two
// names, not two - so only the survivor remains.
//
// The tree is a random-shaped search tree: the values 0 to NodeCount - 1, in a
// seeded order, inserted into this repo's own BinarySearchTree. Its ordering
// invariant makes the tree valid, so the bounds check cannot stop at a violation
// and visits every node. NodeCount stops at LC 98's 10,000-node cap.
public class ValidateBinarySearchTreeBenchmarks
{
    private const int RandomSeed = 98; // LC problem number

    private BinaryTreeNode<int>? _root;

    [Params(100, 1_000, 10_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in SeededSequences.ShuffledZeroTo(NodeCount, RandomSeed))
        {
            tree.Insert(value);
        }

        _root = tree.Root;
    }

    [Benchmark]
    public bool IsValidByBoundsRecursion() => ValidateBinarySearchTreeSolution.IsValidByBoundsRecursion(_root);
}
