using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number Of Ways To Reconstruct A Tree (LC 1719): building the adjacency map with a plain
// BCL Dictionary<int, HashSet<int>> and sorting node ids with Array.Sort/LINQ vs. this
// repo's own HashMap<int, HashMap<int,bool>> (the AccountsMergeTests map-of-set shape) plus
// its own MergeSort over ArrayIndexedSequence for the degree ordering. Pairs are generated
// from a "star of chains" shape (a root connected to several disjoint chains hanging off
// it) so every non-root node clears the initial degree check and forces both strategies
// through the full candidate-parent/subset-check walk instead of short-circuiting early.
[MemoryDiagnoser]
public class NumberOfWaysToReconstructATreeBenchmarks
{
    // LC 1719's own return code for "more than one valid reconstruction exists".
    private const int AmbiguousReconstructionCount = 2;

    [Params(50, 2_000)]
    public int NodeCount;

    private int[][] _pairs = null!;

    // Root (node 0) with ChainCount disjoint chains hanging off it - every node ends up
    // related to every node on its own chain plus the root, and to no node on any other
    // chain, giving both strategies a real (non-degree-mismatch-short-circuited) walk
    // through the full candidate-parent/subset-check logic for every non-root node.
    // Pairs are built from parent links so every node is paired with every one of its
    // own ancestors (root down to itself), matching the "pairs is the FULL ancestor/
    // descendant relation" precondition.
    [GlobalSetup]
    public void Setup()
    {
        const int chainCount = 5;
        var parent = BuildRootedChainParents(NodeCount, chainCount);
        _pairs = BuildAncestorPairs(parent);
    }

    private static int[] BuildRootedChainParents(int nodeCount, int chainCount)
    {
        var parent = new int[nodeCount];
        for (var i = 0; i < nodeCount; i++)
        {
            parent[i] = -1;
        }

        var chainStart = 1;
        for (var c = 0; c < chainCount && chainStart < nodeCount; c++)
        {
            chainStart = AppendChain(parent, nodeCount, chainCount, chainStart);
        }

        return parent;
    }

    private static int[][] BuildAncestorPairs(int[] parent)
    {
        var fullPairs = new List<int[]>();
        for (var node = 1; node < parent.Length; node++)
        {
            for (var ancestor = parent[node]; ancestor != -1; ancestor = parent[ancestor])
            {
                fullPairs.Add([ancestor, node]);
            }
        }

        return fullPairs.ToArray();
    }

    // Extends the chain that starts at chainStart by chainLength nodes, wiring each node's
    // parent pointer to the previous node on the chain (or 0, the root, for the first node).
    // Returns the start of the next chain.
    private static int AppendChain(int[] parent, int nodeCount, int chainCount, int chainStart)
    {
        var chainLength = (nodeCount - 1) / chainCount;
        var previous = 0;

        for (var i = 0; i < chainLength && chainStart + i < nodeCount; i++)
        {
            var node = chainStart + i;
            parent[node] = previous;
            previous = node;
        }

        return chainStart + chainLength;
    }

    [Benchmark(Baseline = true)]
    public int DictionaryAndLinqSort()
    {
        var adjacency = BuildAdjacency(_pairs);
        var nodes = adjacency.Keys.OrderBy(node => adjacency[node].Count).ToArray();

        if (adjacency[nodes[^1]].Count != adjacency.Count - 1)
        {
            return 0;
        }

        return EvaluateAllNodes(adjacency, nodes);
    }

    private static Dictionary<int, HashSet<int>> BuildAdjacency(int[][] pairs)
    {
        var adjacency = new Dictionary<int, HashSet<int>>();

        foreach (var pair in pairs)
        {
            AddEdge(adjacency, pair[0], pair[1]);
            AddEdge(adjacency, pair[1], pair[0]);
        }

        return adjacency;
    }

    private static int EvaluateAllNodes(Dictionary<int, HashSet<int>> adjacency, int[] nodes)
    {
        var result = 1;

        for (var i = 0; i < nodes.Length - 1; i++)
        {
            var nodeResult = EvaluateNode(adjacency, nodes[i]);

            if (nodeResult is null)
            {
                return 0;
            }

            if (nodeResult == AmbiguousReconstructionCount)
            {
                result = AmbiguousReconstructionCount;
            }
        }

        return result;
    }

    // Determines node's candidate parent (its lowest-degree neighbor whose own degree is not
    // less than node's) and validates that every other neighbor of node is also a neighbor of
    // that parent. Returns null when no valid parent exists, otherwise AmbiguousReconstructionCount
    // when the parent choice was not unique (so the tree is not unique either) or 1 otherwise.
    private static int? EvaluateNode(Dictionary<int, HashSet<int>> adjacency, int node)
    {
        var ownDegree = adjacency[node].Count;
        var (parent, minQualifyingDegree, countAtMinDegree) = FindCandidateParent(adjacency, node, ownDegree);

        if (parent == -1)
        {
            return null;
        }

        if (!IsSupersetOfNeighbors(adjacency, node, parent))
        {
            return null;
        }

        return minQualifyingDegree == ownDegree || countAtMinDegree > 1 ? AmbiguousReconstructionCount : 1;
    }

    private static (int Parent, int MinQualifyingDegree, int CountAtMinDegree) FindCandidateParent(
        Dictionary<int, HashSet<int>> adjacency, int node, int ownDegree)
    {
        var candidate = (Parent: -1, MinQualifyingDegree: int.MaxValue, CountAtMinDegree: 0);

        foreach (var neighbor in adjacency[node])
        {
            candidate = ConsiderNeighbor(candidate, neighbor, adjacency[neighbor].Count, ownDegree);
        }

        return candidate;
    }

    // Folds one neighbor into the running (parent, degree, tie-count) candidate: neighbors whose
    // own degree is below ownDegree are ineligible; among eligible neighbors, the lowest-degree one
    // wins, with ties at that minimum degree counted (a tie means the parent choice is ambiguous).
    private static (int Parent, int MinQualifyingDegree, int CountAtMinDegree) ConsiderNeighbor(
        (int Parent, int MinQualifyingDegree, int CountAtMinDegree) candidate,
        int neighbor,
        int neighborDegree,
        int ownDegree)
    {
        if (neighborDegree < ownDegree)
        {
            return candidate;
        }

        if (neighborDegree < candidate.MinQualifyingDegree)
        {
            return (neighbor, neighborDegree, 1);
        }

        if (neighborDegree == candidate.MinQualifyingDegree)
        {
            return (candidate.Parent, candidate.MinQualifyingDegree, candidate.CountAtMinDegree + 1);
        }

        return candidate;
    }

    private static bool IsSupersetOfNeighbors(Dictionary<int, HashSet<int>> adjacency, int node, int parent)
    {
        foreach (var other in adjacency[node])
        {
            if (other != parent && !adjacency[parent].Contains(other))
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public int HashMapAndMergeSort()
    {
        var adjacency = BuildHashMapAdjacency(_pairs);
        var nodes = SortNodesByDegree(adjacency);

        if (Degree(adjacency, nodes[^1]) != adjacency.Count - 1)
        {
            return 0;
        }

        return EvaluateAllNodes(adjacency, nodes);
    }

    private static HashMap<int, HashMap<int, bool>> BuildHashMapAdjacency(int[][] pairs)
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
                var aDegree = Degree(adjacency, a);
                var bDegree = Degree(adjacency, b);
                return aDegree.CompareTo(bDegree);
            }));

        return nodes;
    }

    private static int EvaluateAllNodes(HashMap<int, HashMap<int, bool>> adjacency, int[] nodes)
    {
        var result = 1;

        for (var i = 0; i < nodes.Length - 1; i++)
        {
            var nodeResult = EvaluateNode(adjacency, nodes[i]);

            if (nodeResult is null)
            {
                return 0;
            }

            if (nodeResult == AmbiguousReconstructionCount)
            {
                result = AmbiguousReconstructionCount;
            }
        }

        return result;
    }

    // Determines node's candidate parent (its lowest-degree neighbor whose own degree is not
    // less than node's) and validates that every other neighbor of node is also a neighbor of
    // that parent. Returns null when no valid parent exists, otherwise AmbiguousReconstructionCount
    // when the parent choice was not unique (so the tree is not unique either) or 1 otherwise.
    private static int? EvaluateNode(HashMap<int, HashMap<int, bool>> adjacency, int node)
    {
        var ownDegree = Degree(adjacency, node);
        adjacency.TryGetValue(node, out var neighbors);
        var (parent, minQualifyingDegree, countAtMinDegree) = FindCandidateParent(adjacency, neighbors!, ownDegree);

        if (parent == -1)
        {
            return null;
        }

        if (!IsSupersetOfNeighbors(adjacency, neighbors!, parent))
        {
            return null;
        }

        return minQualifyingDegree == ownDegree || countAtMinDegree > 1 ? AmbiguousReconstructionCount : 1;
    }

    private static (int Parent, int MinQualifyingDegree, int CountAtMinDegree) FindCandidateParent(
        HashMap<int, HashMap<int, bool>> adjacency, HashMap<int, bool> neighbors, int ownDegree)
    {
        var candidate = (Parent: -1, MinQualifyingDegree: int.MaxValue, CountAtMinDegree: 0);

        foreach (var neighbor in neighbors.Keys)
        {
            var neighborDegree = Degree(adjacency, neighbor);
            candidate = ConsiderNeighbor(candidate, neighbor, neighborDegree, ownDegree);
        }

        return candidate;
    }

    private static bool IsSupersetOfNeighbors(
        HashMap<int, HashMap<int, bool>> adjacency, HashMap<int, bool> neighbors, int parent)
    {
        adjacency.TryGetValue(parent, out var parentNeighbors);
        foreach (var other in neighbors.Keys)
        {
            if (other != parent && !parentNeighbors!.HasKey(other))
            {
                return false;
            }
        }

        return true;
    }

    private static void AddEdge(Dictionary<int, HashSet<int>> adjacency, int from, int to)
    {
        if (!adjacency.TryGetValue(from, out var neighbors))
        {
            neighbors = [];
            adjacency[from] = neighbors;
        }

        neighbors.Add(to);
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
