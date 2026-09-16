using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.MostStonesRemovedWithSameRowOrColumn;

// LeetCode 947. Most Stones Removed with Same Row or Column: a stone may be removed
// while it still shares a row or column with another remaining stone, so every
// connected component of the "shares a row/column" graph collapses to exactly one
// survivor. The answer is therefore stones.Length minus the component count, and the
// only thing a strategy decides is how it discovers that the components are.
//
// Both strategies partition with this repo's own DisjointSet - the contrast between
// them is edge discovery, not the partition structure:
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
internal static class MostStonesRemovedWithSameRowOrColumnSolution
{
    // The naive arm: discover the edges by brute force, union by stone index, then
    // count surviving roots. Internals are deliberately the textbook ones - a nested
    // pair scan and a BCL HashSet of roots.
    public static int RemoveStonesByPairwiseScan(int[][] stones)
    {
        var count = stones.Length;
        var components = new DisjointSet(count);

        for (var i = 0; i < count; i++)
        {
            for (var j = i + 1; j < count; j++)
            {
                if (HasSharedRowOrColumn(stones[i], stones[j]))
                {
                    components.Union(i, j);
                }
            }
        }

        var roots = new HashSet<int>();

        for (var i = 0; i < count; i++)
        {
            roots.Add(components.Find(i));
        }

        return count - roots.Count;
    }

    private static bool HasSharedRowOrColumn(int[] first, int[] second) =>
        first[0] == second[0] || first[1] == second[1];

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
}
