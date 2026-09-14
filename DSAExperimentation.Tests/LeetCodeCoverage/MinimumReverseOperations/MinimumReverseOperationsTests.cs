using DSAExperimentation.LeetCode.MinimumReverseOperations;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumReverseOperations;

// Harness only. Both strategies - the O(n)-per-pop brute-force scan and the
// Reduce.Graph walk over ReversalTopology - are MinimumReverseOperationsSolution's;
// this file just pins them to LeetCode's published examples, plus the two degenerate
// window sizes the examples never reach: K = 1, where no reversal moves anything, and
// K = n, where the only window there is splits the array into mirror pairs.
public sealed class MinimumReverseOperationsTests
{
    public static TheoryData<int, int, int[], int, int[]> Examples =>
        new()
        {
            { 4, 0, [1, 2], 4, [0, -1, -1, 1] },
            { 5, 0, [2, 4], 3, [0, -1, -1, -1, -1] },
            { 4, 2, [0, 1, 3], 1, [-1, -1, 0, -1] },
            { 3, 1, [], 1, [-1, 0, -1] },
            { 4, 0, [], 4, [0, -1, -1, 1] },
            { 5, 2, [], 2, [2, 1, 0, 1, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByBruteForceScan_LeetCodeExamples_ReturnsShortestOperationCounts(
        int n, int p, int[] banned, int k, int[] expected) =>
        Assert.Equal(expected, MinimumReverseOperationsSolution.MinOperationsByBruteForceScan(n, p, banned, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByReduceGraph_LeetCodeExamples_ReturnsShortestOperationCounts(
        int n, int p, int[] banned, int k, int[] expected) =>
        Assert.Equal(expected, MinimumReverseOperationsSolution.MinOperationsByReduceGraph(n, p, banned, k));
}
