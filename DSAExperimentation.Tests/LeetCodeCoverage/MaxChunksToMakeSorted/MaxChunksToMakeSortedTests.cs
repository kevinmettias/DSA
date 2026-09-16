using DSAExperimentation.LeetCode.MaxChunksToMakeSorted;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxChunksToMakeSorted;

// Harness only: both strategies live in MaxChunksToMakeSortedSolution and are
// asserted against the same examples.
public sealed partial class MaxChunksToMakeSortedTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [4, 3, 2, 1, 0], 1 },
            { [1, 0, 2, 3, 4], 4 },
            { [0, 1, 2, 3, 4], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxChunksByBruteForce_LeetCodeExamples_ReturnsMaxChunkCount(int[] arr, int expected) =>
        Assert.Equal(expected, MaxChunksToMakeSortedSolution.MaxChunksByBruteForce(arr));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxChunksByRunningMaxScan_LeetCodeExamples_ReturnsMaxChunkCount(int[] arr, int expected) =>
        Assert.Equal(expected, MaxChunksToMakeSortedSolution.MaxChunksByRunningMaxScan(arr));
}
