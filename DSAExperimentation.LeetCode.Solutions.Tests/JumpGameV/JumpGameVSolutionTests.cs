using DSAExperimentation.LeetCode.Solutions.JumpGameV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.JumpGameV;

// Harness only. The reachability DAG is JumpNode/JumpTopology and both strategies
// are JumpGameVSolution's - this file just pins them to the alternating array,
// where no jump ever chains, and two single-element arrays, where the starting
// index is the only one visited. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class JumpGameVSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [7, 1, 7, 1, 7, 1], 2, 2 },
            { [66], 1, 1 },
            { [1], 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxIndicesVisitedByMemoizedDfs_Cases_ReturnsExpectedCount(
        int[] arr, int maxJumpDistance, int expected)
    {
        var actual = JumpGameVSolution.MaxIndicesVisitedByMemoizedDfs(arr, maxJumpDistance);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxIndicesVisitedByTopologicalSort_Cases_ReturnsExpectedCount(
        int[] arr, int maxJumpDistance, int expected)
    {
        var actual = JumpGameVSolution.MaxIndicesVisitedByTopologicalSort(arr, maxJumpDistance);

        Assert.Equal(expected, actual);
    }
}
