using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.NumberOfWaysToReconstructATree;

// LeetCode 1719. Number Of Ways To Reconstruct A Tree: pairs is claimed to be the
// FULL ancestor/descendant relation of some rooted tree; report 0 if no tree
// produces it, 1 if exactly one does, 2 if more than one does.
//
// Both strategies run the same "candidate parent" argument and differ only in
// which containers carry it:
//   - Read pairs as an undirected adjacency map. A node's degree is how many
//     nodes it is related to, and an ancestor is related to everything its
//     descendant is, so a parent's degree is never below its child's.
//   - The root must therefore be related to everyone: if the highest degree is
//     not n - 1, no tree produces these pairs.
//   - Order the nodes by degree; for every node but the last (arbitrarily
//     designated the root) the only possible parent is the lowest-degree
//     neighbour whose degree is still at least the node's own. Transitivity then
//     requires every other neighbour of the node to be a neighbour of that parent
//     too - if one is not, no tree produces these pairs.
//   - A tie for that minimal qualifying degree - including a tie with the node's
//     own degree, which means the node and its parent could swap - means a second
//     tree reproduces the same relation.
internal static class NumberOfWaysToReconstructATreeSolution
{
    // LC 1719's own three answers.
    private const int NoTree = 0;
    private const int ExactlyOneTree = 1;
    private const int MoreThanOneTree = 2;

    // No candidate parent exists for this node, which rules out every tree.
    private const int NoParent = -1;

    // The textbook answer: a BCL Dictionary of HashSets for the adjacency and
    // LINQ's OrderBy for the degree ordering. Deliberately written without this
    // repo's primitives - it is the arm the composed strategy below has to
    // justify itself against.
    public static int CheckWaysByDictionaryAndLinqSort(int[][] pairs)
    {
        var adjacency = BuildDictionaryAdjacency(pairs);
        var nodes = adjacency.Keys.OrderBy(node => adjacency[node].Count).ToArray();

        if (adjacency[nodes[^1]].Count != adjacency.Count - 1)
        {
            return NoTree;
        }

        return EvaluateEveryNonRootNode(adjacency, nodes);
    }

    private static Dictionary<int, HashSet<int>> BuildDictionaryAdjacency(int[][] pairs)
    {
        var adjacency = new Dictionary<int, HashSet<int>>();

        foreach (var pair in pairs)
        {
            AddDictionaryEdge(adjacency, pair[0], pair[1]);
            AddDictionaryEdge(adjacency, pair[1], pair[0]);
        }

        return adjacency;
    }

    private static void AddDictionaryEdge(Dictionary<int, HashSet<int>> adjacency, int from, int to)
    {
        if (!adjacency.TryGetValue(from, out var neighbors))
        {
            neighbors = [];
            adjacency[from] = neighbors;
        }

        neighbors.Add(to);
    }

    // nodes[^1] is arbitrarily designated the root and skipped; every other node
    // (including any further node that also happens to have degree n - 1) is
    // validated against its own candidate parent.
    private static int EvaluateEveryNonRootNode(Dictionary<int, HashSet<int>> adjacency, int[] nodes)
    {
        var result = ExactlyOneTree;

        for (var i = 0; i < nodes.Length - 1; i++)
        {
            var nodeResult = EvaluateNode(adjacency, nodes[i]);

            if (nodeResult is null)
            {
                return NoTree;
            }

            if (nodeResult == MoreThanOneTree)
            {
                result = MoreThanOneTree;
            }
        }

        return result;
    }

    // Returns null when no tree can produce these pairs, MoreThanOneTree when this
    // node's parent choice was not forced, ExactlyOneTree when it was.
    private static int? EvaluateNode(Dictionary<int, HashSet<int>> adjacency, int node)
    {
        var ownDegree = adjacency[node].Count;
        var (parent, minQualifyingDegree, countAtMinDegree) = FindCandidateParent(adjacency, node, ownDegree);

        if (parent == NoParent)
        {
            return null;
        }

        if (!IsSupersetOfNeighbors(adjacency, node, parent))
        {
            return null;
        }

        return minQualifyingDegree == ownDegree || countAtMinDegree > 1 ? MoreThanOneTree : ExactlyOneTree;
    }

    private static (int Parent, int MinQualifyingDegree, int CountAtMinDegree) FindCandidateParent(
        Dictionary<int, HashSet<int>> adjacency, int node, int ownDegree)
    {
        var candidate = (Parent: NoParent, MinQualifyingDegree: int.MaxValue, CountAtMinDegree: 0);

        foreach (var neighbor in adjacency[node])
        {
            candidate = ConsiderNeighbor(candidate, neighbor, adjacency[neighbor].Count, ownDegree);
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

    // This repo's own composition: HashMap<int, HashMap<int, bool>>
    // (DataStructures/HashMap/HashMap.cs) is the map-of-set shape this repo's
    // adjacency-building problems already use, and MergeSort
    // (Algorithms/Sorting/MergeSort.cs) over an ArrayIndexedSequence<int> supplies
    // the degree ordering in place of LINQ's OrderBy.
    public static int CheckWaysByHashMapAndMergeSort(int[][] pairs)
    {
        var adjacency = BuildHashMapAdjacency(pairs);
        var nodes = SortNodesByDegree(adjacency);

        if (Degree(adjacency, nodes[^1]) != adjacency.Count - 1)
        {
            return NoTree;
        }

        return EvaluateEveryNonRootNode(adjacency, nodes);
    }

    private static HashMap<int, HashMap<int, bool>> BuildHashMapAdjacency(int[][] pairs)
    {
        var adjacency = new HashMap<int, HashMap<int, bool>>();

        foreach (var pair in pairs)
        {
            AddHashMapEdge(adjacency, pair[0], pair[1]);
            AddHashMapEdge(adjacency, pair[1], pair[0]);
        }

        return adjacency;
    }

    private static void AddHashMapEdge(HashMap<int, HashMap<int, bool>> adjacency, int from, int to)
    {
        if (!adjacency.TryGetValue(from, out var neighbors))
        {
            neighbors = new HashMap<int, bool>();
            adjacency.Set(from, neighbors);
        }

        neighbors.Set(to, true);
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

    private static int EvaluateEveryNonRootNode(HashMap<int, HashMap<int, bool>> adjacency, int[] nodes)
    {
        var result = ExactlyOneTree;

        for (var i = 0; i < nodes.Length - 1; i++)
        {
            var nodeResult = EvaluateNode(adjacency, nodes[i]);

            if (nodeResult is null)
            {
                return NoTree;
            }

            if (nodeResult == MoreThanOneTree)
            {
                result = MoreThanOneTree;
            }
        }

        return result;
    }

    private static int? EvaluateNode(HashMap<int, HashMap<int, bool>> adjacency, int node)
    {
        var ownDegree = Degree(adjacency, node);
        adjacency.TryGetValue(node, out var neighbors);
        var (parent, minQualifyingDegree, countAtMinDegree) = FindCandidateParent(adjacency, neighbors!, ownDegree);

        if (parent == NoParent)
        {
            return null;
        }

        if (!IsSupersetOfNeighbors(adjacency, neighbors!, parent))
        {
            return null;
        }

        return minQualifyingDegree == ownDegree || countAtMinDegree > 1 ? MoreThanOneTree : ExactlyOneTree;
    }

    private static (int Parent, int MinQualifyingDegree, int CountAtMinDegree) FindCandidateParent(
        HashMap<int, HashMap<int, bool>> adjacency, HashMap<int, bool> neighbors, int ownDegree)
    {
        var candidate = (Parent: NoParent, MinQualifyingDegree: int.MaxValue, CountAtMinDegree: 0);

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

    private static int Degree(HashMap<int, HashMap<int, bool>> adjacency, int node)
    {
        adjacency.TryGetValue(node, out var neighbors);

        return neighbors!.Count;
    }

    // Folds one neighbour into the running (parent, degree, tie-count) candidate:
    // neighbours whose own degree is below ownDegree cannot be an ancestor and are
    // ineligible; among the eligible ones the lowest-degree neighbour wins, with
    // ties at that minimum counted, because a tie means the parent choice - and so
    // the tree - is not unique. Shared by both strategies: it reads degrees, not
    // containers.
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
}
