using DSAExperimentation.LeetCode.JumpGameV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameV;

// Harness only. The reachability DAG is JumpNode/JumpTopology and both strategies
// are JumpGameVSolution's - this file just pins them to LeetCode's published
// examples, including the two the original test omitted (a strictly decreasing
// run, where every index is reachable in one chain, and the alternating array
// where no jump ever chains).
public sealed class JumpGameVTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [6, 4, 14, 6, 8, 13, 9, 7, 10, 6, 12], 2, 4 },
            { [3, 3, 3, 3, 3], 3, 1 },
            { [7, 6, 5, 4, 3, 2, 1], 1, 7 },
            { [7, 1, 7, 1, 7, 1], 2, 2 },
            { [66], 1, 1 },
            { [1], 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxIndicesVisitedByMemoizedDfs_LeetCodeExamples_ReturnsExpectedCount(
        int[] arr, int d, int expected) =>
        Assert.Equal(expected, JumpGameVSolution.MaxIndicesVisitedByMemoizedDfs(arr, d));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxIndicesVisitedByTopologicalSort_LeetCodeExamples_ReturnsExpectedCount(
        int[] arr, int d, int expected) =>
        Assert.Equal(expected, JumpGameVSolution.MaxIndicesVisitedByTopologicalSort(arr, d));
}
