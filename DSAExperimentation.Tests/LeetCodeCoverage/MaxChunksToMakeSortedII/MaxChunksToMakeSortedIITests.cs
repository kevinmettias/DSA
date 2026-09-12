using DSAExperimentation.LeetCode.MaxChunksToMakeSortedII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxChunksToMakeSortedII;

// Harness only. Both strategies are MaxChunksToMakeSortedIISolution's - this file
// just pins them to LeetCode's published examples plus a single-element edge case.
public sealed class MaxChunksToMakeSortedIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [5, 4, 3, 2, 1], 1 },
            { [2, 1, 3, 4, 4], 4 },
            { [1, 0, 2, 3, 4], 4 },
            { [7], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxChunksByBruteForceSuffixRescan_LeetCodeExamples_ReturnsMaxChunkCount(int[] arr, int expected) =>
        Assert.Equal(expected, MaxChunksToMakeSortedIISolution.MaxChunksByBruteForceSuffixRescan(arr));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxChunksByMonotonicStackMerge_LeetCodeExamples_ReturnsMaxChunkCount(int[] arr, int expected) =>
        Assert.Equal(expected, MaxChunksToMakeSortedIISolution.MaxChunksByMonotonicStackMerge(arr));
}
