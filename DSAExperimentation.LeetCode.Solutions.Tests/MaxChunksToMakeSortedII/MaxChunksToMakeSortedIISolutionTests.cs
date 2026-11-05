using DSAExperimentation.LeetCode.Solutions.MaxChunksToMakeSortedII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaxChunksToMakeSortedII;

// Harness only. Both strategies are MaxChunksToMakeSortedIISolution's - this file
// just pins them to a single-element edge case.
public sealed partial class MaxChunksToMakeSortedIISolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [7], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxChunksByBruteForceSuffixRescan_Cases_ReturnsMaxChunkCount(int[] arr, int expected) =>
        Assert.Equal(expected, MaxChunksToMakeSortedIISolution.MaxChunksByBruteForceSuffixRescan(arr));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxChunksByMonotonicStackMerge_Cases_ReturnsMaxChunkCount(int[] arr, int expected) =>
        Assert.Equal(expected, MaxChunksToMakeSortedIISolution.MaxChunksByMonotonicStackMerge(arr));
}
