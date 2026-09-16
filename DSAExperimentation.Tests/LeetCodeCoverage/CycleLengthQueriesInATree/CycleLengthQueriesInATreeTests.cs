using DSAExperimentation.LeetCode.CycleLengthQueriesInATree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CycleLengthQueriesInATree;

// Harness only. Both strategies are CycleLengthQueriesInATreeSolution's - the
// per-query ancestor Dictionary that used to live only in the benchmark's baseline
// arm, and the HeapArrayIndex.Parent two-pointer walk the test used to inline.
public sealed partial class CycleLengthQueriesInATreeTests
{
    public static TheoryData<int, int[][], int[]> Examples =>
        new()
        {
            // LC example 1.
            { 3, new[] { new[] { 5, 3 }, new[] { 4, 7 }, new[] { 2, 3 } }, [4, 5, 3] },

            // LC example 2: 1 and 2 are already parent and child, so the added edge
            // doubles that edge into a two-node cycle.
            { 2, new[] { new[] { 1, 2 } }, [2] },

            // treeLevels=2 (nodes 1,2,3): adding edge 2-3 closes the triangle 2-1-3-2.
            { 2, new[] { new[] { 2, 3 } }, [3] },

            // Root to a leaf (1-3-7-1) and two siblings (4-2-5-4), in one call, so
            // the per-query answers have to stay in query order.
            { 3, new[] { new[] { 1, 7 }, new[] { 4, 5 } }, [3, 3] },

            // Opposite corners of a four-level tree meet only at the root
            // (8-4-2-1-3-7-15-8), while 12 and 13 share the parent 6.
            { 4, new[] { new[] { 8, 15 }, new[] { 12, 13 } }, [7, 3] },

            // The deeper endpoint is lifted first whichever way round the query
            // states it.
            { 4, new[] { new[] { 15, 3 }, new[] { 3, 15 } }, [3, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CycleLengthQueriesByAncestorDictionary_LeetCodeExamples_ReturnsPerQueryCycleLengths(
        int treeLevels, int[][] queries, int[] expected)
    {
        var actual = CycleLengthQueriesInATreeSolution.CycleLengthQueriesByAncestorDictionary(
            treeLevels, queries);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CycleLengthQueriesByParentIndexWalk_LeetCodeExamples_ReturnsPerQueryCycleLengths(
        int treeLevels, int[][] queries, int[] expected)
    {
        var actual = CycleLengthQueriesInATreeSolution.CycleLengthQueriesByParentIndexWalk(
            treeLevels, queries);
        Assert.Equal(expected, actual);
    }
}
