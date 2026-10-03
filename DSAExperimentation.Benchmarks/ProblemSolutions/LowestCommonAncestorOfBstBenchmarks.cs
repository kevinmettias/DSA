using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.LowestCommonAncestorOfBst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LowestCommonAncestorOfBstSolution's, the same methods
// LowestCommonAncestorOfBstSolutionTests proves correct. The tree is built and the two queried nodes are
// found once in Setup, so the timed region is a single query against an already-ordered tree -
// which is the whole contest: the ancestry walk re-derives parentage from the node shape, while
// the BST walk reads the ordering it was handed.
public class LowestCommonAncestorOfBstBenchmarks
{
    private const int RandomSeed = 235; // LC problem number

    private BinaryTreeNode<int> _root = new(0);
    private BinaryTreeNode<int> _first = new(0);
    private BinaryTreeNode<int> _second = new(0);

    [Params(1_024, 16_384)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var values = Enumerable.Range(0, NodeCount).OrderBy(_ => random.Next()).ToArray();

        _root = BuildTree(values);

        // The two deepest insertions in the seeded permutation: neither is the root, so both arms
        // have to descend before they can answer.
        _first = FindNode(_root, values[^1]);
        _second = FindNode(_root, values[^2]);
    }

    [Benchmark(Baseline = true)]
    public int BstValueComparison() =>
        LowestCommonAncestorOfBstSolution.FindLcaByBstValueComparison(_root, _first, _second)?.Value ?? 0;

    [Benchmark]
    public int AncestryWalk() =>
        LowestCommonAncestorOfBstSolution.FindLcaByAncestryWalk(_root, _first, _second)?.Value ?? 0;

    private static BinaryTreeNode<int> BuildTree(int[] values)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        return tree.Root
            ?? throw new InvalidOperationException("A tree built from at least one value always has a root.");
    }

    private static BinaryTreeNode<int> FindNode(BinaryTreeNode<int> root, int value)
    {
        var node = root;

        while (node.Value != value)
        {
            var next = value < node.Value ? node.Left : node.Right;
            node = next ?? throw new InvalidOperationException($"Value {value} is not present in the tree.");
        }

        return node;
    }
}
