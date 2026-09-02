using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToReconstructATree;

// LeetCode 1719. Number Of Ways To Reconstruct A Tree: the "candidate parent" technique -
// pairs is the FULL ancestor/descendant relation of some unknown tree, so build it as an
// adjacency map (HashMap<node, HashMap<neighbor,bool>>, the same map-of-set shape
// AccountsMergeTests uses for emailsByRoot), sort nodes by degree with this repo's own
// MergeSort (AccountsMergeTests' "sort with this repo's MergeSort" convention), and for
// every node but one arbitrarily designated root, its parent is the neighbor with the
// smallest degree still >= its own degree - transitivity of ancestor/descendant means
// adjacency[node] minus that parent must be a subset of adjacency[parent], and a tie for
// that minimal qualifying degree (including a tie with the node's own degree) means more
// than one tree reproduces the same relation.
public sealed partial class NumberOfWaysToReconstructATreeTests
{
    [Fact]
    public void CheckWays_StarShapedTree_ReturnsExactlyOne()
    {
        int[][] pairs = [[1, 2], [2, 3]];

        Assert.Equal(1, CheckWays(pairs));
    }

    [Fact]
    public void CheckWays_FullyConnectedTriple_ReturnsMoreThanOne()
    {
        int[][] pairs = [[1, 2], [2, 3], [1, 3]];

        Assert.Equal(2, CheckWays(pairs));
    }

    [Fact]
    public void CheckWays_DegreesCannotProduceARoot_ReturnsZero()
    {
        int[][] pairs = [[1, 2], [2, 3], [2, 4], [1, 5]];

        Assert.Equal(0, CheckWays(pairs));
    }

    [Fact]
    public void CheckWays_TwoNodesBothHaveMaxDegree_StillDetectsAmbiguity()
    {
        // 1 -> 4 -> {2, 3} reproduces the same relation as 4 -> 1 -> {2, 3}: both 1 and 4
        // end up related to everyone (degree n-1) despite only one of them being the
        // actual root of either tree.
        int[][] pairs = [[1, 2], [1, 3], [1, 4], [2, 4], [3, 4]];

        Assert.Equal(2, CheckWays(pairs));
    }

    private static int CheckWays(int[][] pairs)
    {
        var adjacency = BuildAdjacency(pairs);
        var nodes = SortNodesByDegree(adjacency);

        if (Degree(adjacency, nodes[^1]) != adjacency.Count - 1)
        {
            return 0;
        }

        return EvaluateAllCandidateParents(adjacency, nodes);
    }

    private static HashMap<int, HashMap<int, bool>> BuildAdjacency(int[][] pairs)
    {
        var adjacency = new HashMap<int, HashMap<int, bool>>();

        foreach (var pair in pairs)
        {
            AddEdge(adjacency, pair[0], pair[1]);
            AddEdge(adjacency, pair[1], pair[0]);
        }

        return adjacency;
    }

    private static int[] SortNodesByDegree(HashMap<int, HashMap<int, bool>> adjacency)
    {
        var nodes = adjacency.Keys.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(
            new ArrayIndexedSequence<int>(nodes),
            Comparer<int>.Create((a, b) =>
            {
                var degreeA = Degree(adjacency, a);
                var degreeB = Degree(adjacency, b);
                return degreeA.CompareTo(degreeB);
            }));

        return nodes;
    }

    // nodes[^1] is arbitrarily designated the root and skipped; every other node
    // (including any further node that also happens to have degree n - 1) is
    // validated against its own candidate parent.
    private static int EvaluateAllCandidateParents(HashMap<int, HashMap<int, bool>> adjacency, int[] nodes)
    {
        var result = 1;

        for (var i = 0; i < nodes.Length - 1; i++)
        {
            var outcome = EvaluateCandidateParent(adjacency, nodes[i]);

            if (outcome is null)
            {
                return 0;
            }

            if (outcome.Value)
            {
                result = 2;
            }
        }

        return result;
    }

    // Returns null when no valid parent exists for this node (the pairs are
    // inconsistent with any tree), true when the parent choice is ambiguous
    // (ties the root-parent designation), false when the parent is unambiguous.
    private static bool? EvaluateCandidateParent(HashMap<int, HashMap<int, bool>> adjacency, int node)
    {
        var ownDegree = Degree(adjacency, node);
        adjacency.TryGetValue(node, out var neighbors);

        var (parent, minQualifyingDegree, countAtMinDegree) = FindCandidateParent(adjacency, neighbors!, ownDegree);

        if (parent == -1)
        {
            return null;
        }

        adjacency.TryGetValue(parent, out var parentNeighbors);
        foreach (var other in neighbors!.Keys)
        {
            if (other != parent && !parentNeighbors!.HasKey(other))
            {
                return null;
            }
        }

        return minQualifyingDegree == ownDegree || countAtMinDegree > 1;
    }

    private static (int Parent, int MinQualifyingDegree, int CountAtMinDegree) FindCandidateParent(
        HashMap<int, HashMap<int, bool>> adjacency, HashMap<int, bool> neighbors, int ownDegree)
    {
        var minQualifyingDegree = FindMinQualifyingDegree(adjacency, neighbors, ownDegree);
        var (parent, countAtMinDegree) = CountNeighborsAtDegree(adjacency, neighbors, minQualifyingDegree);

        return (parent, minQualifyingDegree, countAtMinDegree);
    }

    // Smallest neighbor degree that is still >= ownDegree, or int.MaxValue if none qualify.
    private static int FindMinQualifyingDegree(HashMap<int, HashMap<int, bool>> adjacency, HashMap<int, bool> neighbors, int ownDegree)
    {
        var minQualifyingDegree = int.MaxValue;

        foreach (var neighbor in neighbors.Keys)
        {
            var neighborDegree = Degree(adjacency, neighbor);
            if (neighborDegree >= ownDegree && neighborDegree < minQualifyingDegree)
            {
                minQualifyingDegree = neighborDegree;
            }
        }

        return minQualifyingDegree;
    }

    // First neighbor (in iteration order) at exactly the given degree, plus how many share it.
    private static (int Parent, int Count) CountNeighborsAtDegree(HashMap<int, HashMap<int, bool>> adjacency, HashMap<int, bool> neighbors, int degree)
    {
        var parent = -1;
        var count = 0;

        foreach (var neighbor in neighbors.Keys)
        {
            if (Degree(adjacency, neighbor) != degree)
            {
                continue;
            }

            if (parent == -1)
            {
                parent = neighbor;
            }

            count++;
        }

        return (parent, count);
    }

    private static void AddEdge(HashMap<int, HashMap<int, bool>> adjacency, int from, int to)
    {
        if (!adjacency.TryGetValue(from, out var neighbors))
        {
            neighbors = new HashMap<int, bool>();
            adjacency.Set(from, neighbors);
        }

        neighbors.Set(to, true);
    }

    private static int Degree(HashMap<int, HashMap<int, bool>> adjacency, int node)
    {
        adjacency.TryGetValue(node, out var neighbors);
        return neighbors!.Count;
    }
}
