using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.LeetCode.Solutions.MostStonesRemovedWithSameRowOrColumn;

// LeetCode 947. Most Stones Removed with Same Row or Column: a stone may be removed
// while it still shares a row or column with another remaining stone, so every
// connected component of the "shares a row/column" graph collapses to exactly one
// survivor. The answer is therefore stones.Length minus the component count, and the
// only thing a strategy decides is how it discovers that the components are.
//
// Both strategies partition with a union-find that compresses paths and links by
// rank - a hand-written pair of parent and rank arrays in the baseline, this repo's own
// DisjointSet in the composed arm - so the contrast between them is edge discovery,
// not the partition structure:
//
//   PairwiseScan checks every O(n^2) stone pair for a shared row or column before
//   unioning by stone index - the textbook way to find the graph's edges.
//
//   RowColumnKeyedUnion never materializes those edges. It unions each stone's row id
//   with its column id (columns offset past every row id so the two axes share one
//   dense DisjointSet universe without colliding - the same "encode two id spaces into
//   one dense-int universe" move RedundantConnection's nodeCount+1 sizing makes for a
//   single axis). Two stones end up connected exactly when they route through the same
//   row or column id, whether or not they were ever compared, so this is O(n) unions
//   instead of O(n^2) comparisons.
[LeetCodeQuestion("most-stones-removed-with-same-row-or-column", LeetCodeAnswerOrder.AsWritten)]
internal static class MostStonesRemovedWithSameRowOrColumnSolution
{
    // The naive arm: discover the edges by brute force, union by stone index, then
    // count surviving roots. Internals are deliberately the textbook ones - a nested
    // pair scan, parent and rank arrays for the union-find, and a BCL HashSet of roots.
    public static int RemoveStonesByPairwiseScan(int[][] stones)
    {
        var count = stones.Length;
        var parent = SingletonParents(count);
        var rank = new int[count];

        for (var i = 0; i < count; i++)
        {
            for (var j = i + 1; j < count; j++)
            {
                if (HasSharedRowOrColumn(stones[i], stones[j]))
                {
                    UnionByRank(parent, rank, i, j);
                }
            }
        }

        var roots = new HashSet<int>();

        for (var i = 0; i < count; i++)
        {
            var root = FindRoot(parent, i);
            roots.Add(root);
        }

        return count - roots.Count;
    }

    // Every stone starts as the root of its own one-stone set.
    private static int[] SingletonParents(int count)
    {
        var parent = new int[count];

        for (var i = 0; i < count; i++)
        {
            parent[i] = i;
        }

        return parent;
    }

    private static bool HasSharedRowOrColumn(int[] first, int[] second) =>
        first[0] == second[0] || first[1] == second[1];

    // Hangs the lower-ranked root under the higher one, so no tree grows taller than
    // log n; a tie keeps the first root on top and raises its rank.
    private static void UnionByRank(int[] parent, int[] rank, int first, int second)
    {
        var firstRoot = FindRoot(parent, first);
        var secondRoot = FindRoot(parent, second);

        if (firstRoot == secondRoot)
        {
            return;
        }

        if (rank[firstRoot] < rank[secondRoot])
        {
            (firstRoot, secondRoot) = (secondRoot, firstRoot);
        }

        parent[secondRoot] = firstRoot;

        if (rank[firstRoot] == rank[secondRoot])
        {
            rank[firstRoot]++;
        }
    }

    // Union each stone into its row id and its column id instead of into other stones,
    // so connectivity falls out of the shared axis ids with no pair comparison at all.
    public static int RemoveStonesByRowColumnKeyedUnion(int[][] stones)
    {
        var columnOffset = stones.Max(stone => stone[0]) + 1;
        var universeSize = columnOffset + stones.Max(stone => stone[1]) + 1;
        var components = new DisjointSet(universeSize);

        foreach (var stone in stones)
        {
            components.Union(stone[0], columnOffset + stone[1]);
        }

        var roots = new HashSet<int>();

        foreach (var stone in stones)
        {
            roots.Add(components.Find(stone[0]));
        }

        return stones.Length - roots.Count;
    }

    // Walks up to the root, halving the path on the way so later finds take fewer steps.
    private static int FindRoot(int[] parent, int id)
    {
        while (parent[id] != id)
        {
            parent[id] = parent[parent[id]];
            id = parent[id];
        }

        return id;
    }

}
