using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Step-By-Step Directions From a Binary Tree Node to Another (LC 2096): both
// benchmarks solve the identical problem for the leftmost and rightmost leaf of a
// balanced tree (an LCA of the root, the worst case for either approach).
// DirectPathSearch hand-rolls the textbook solution - one bespoke recursive
// find-path-to-node call per endpoint, O(n) total. LowestCommonAncestorWithRootTo
// LeafPaths instead composes two existing repo primitives (LowestCommonAncestor,
// already proven by LowestCommonAncestorOfBstTests, plus AllRootToLeafPaths,
// already proven by PathSumII/PathSumIII/KthAncestorOfATreeNode): correct, and
// reuses tested engines instead of new bespoke recursion, but AllRootToLeafPaths
// collects EVERY leaf path under the LCA rather than just the two paths this
// problem needs, so it is expected to do strictly more work - the same "a
// generic primitive proves feasibility, not necessarily minimal cost" trade-off
// ReconstructItineraryBenchmarks documents for its own repo-primitive side.
[MemoryDiagnoser]
public class StepByStepDirectionsFromABinaryTreeNodeToAnotherBenchmarks
{
    private const string TargetNotReachableMessage = "target is not reachable from the given root.";

    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;
    private BinaryTreeNode<int> _start = null!;
    private BinaryTreeNode<int> _dest = null!;

    [GlobalSetup]
    public void Setup()
    {
        _root = BinaryTrees.Balanced(NodeCount);
        _start = LeftmostLeaf(_root);
        _dest = RightmostLeaf(_root);
    }

    private static BinaryTreeNode<int> LeftmostLeaf(BinaryTreeNode<int> node)
    {
        while (node.Left is not null || node.Right is not null)
        {
            node = node.Left ?? node.Right!;
        }

        return node;
    }

    private static BinaryTreeNode<int> RightmostLeaf(BinaryTreeNode<int> node)
    {
        while (node.Left is not null || node.Right is not null)
        {
            node = node.Right ?? node.Left!;
        }

        return node;
    }

    [Benchmark(Baseline = true)]
    public string DirectPathSearch()
    {
        var pathToStart = FindPath(_root, _start);
        var pathToDest = FindPath(_root, _dest);

        return BuildDirections(pathToStart, pathToDest);
    }

    private static List<BinaryTreeNode<int>> FindPath(BinaryTreeNode<int> root, BinaryTreeNode<int> target)
    {
        var path = new List<BinaryTreeNode<int>>();
        TryFindPath(root, target, path);
        return path;
    }

    private static bool TryFindPath(BinaryTreeNode<int> node, BinaryTreeNode<int> target, List<BinaryTreeNode<int>> path)
    {
        path.Add(node);

        if (node == target)
        {
            return true;
        }

        if ((node.Left is not null && TryFindPath(node.Left, target, path)) ||
            (node.Right is not null && TryFindPath(node.Right, target, path)))
        {
            return true;
        }

        path.RemoveAt(path.Count - 1);
        return false;
    }

    private static string BuildDirections(List<BinaryTreeNode<int>> pathToStart, List<BinaryTreeNode<int>> pathToDest)
    {
        var commonLength = 0;

        while (commonLength < pathToStart.Count && commonLength < pathToDest.Count
               && pathToStart[commonLength] == pathToDest[commonLength])
        {
            commonLength++;
        }

        var up = new string('U', pathToStart.Count - commonLength);
        var down = new char[pathToDest.Count - commonLength];

        for (var i = 0; i < down.Length; i++)
        {
            var parent = pathToDest[commonLength + i - 1];
            down[i] = parent.Left == pathToDest[commonLength + i] ? 'L' : 'R';
        }

        return up + new string(down);
    }

    [Benchmark]
    public string LowestCommonAncestorWithRootToLeafPaths()
    {
        var lca = LowestCommonAncestor.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(_root, _start, _dest);

        var leafPaths = AllRootToLeafPaths.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(lca!);

        var pathToStart = PathTo(leafPaths, _start);
        var pathToDest = PathTo(leafPaths, _dest);

        var up = new string('U', pathToStart.Length - 1);
        var down = new char[pathToDest.Length - 1];

        for (var i = 0; i < down.Length; i++)
        {
            down[i] = pathToDest[i].Left == pathToDest[i + 1] ? 'L' : 'R';
        }

        return up + new string(down);
    }

    private static BinaryTreeNode<int>[] PathTo(List<BinaryTreeNode<int>[]> leafPaths, BinaryTreeNode<int> target)
    {
        foreach (var path in leafPaths)
        {
            var index = Array.IndexOf(path, target);

            if (index >= 0)
            {
                return path[..(index + 1)];
            }
        }

        throw new InvalidOperationException(TargetNotReachableMessage);
    }
}
