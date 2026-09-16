using DSAExperimentation.LeetCode.MinimumPairRemovalToSortArrayI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumPairRemovalToSortArrayI;

// Harness only. Both strategies are MinimumPairRemovalToSortArrayISolution's -
// this file just pins them to LeetCode's published examples.
public sealed partial class MinimumPairRemovalToSortArrayITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [5, 2, 3, 1], 2 },
            { [1, 2, 2], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByBruteForce_LeetCodeExamples_ReturnsMergeCountToSort(int[] nums, int expected) =>
        Assert.Equal(expected, MinimumPairRemovalToSortArrayISolution.MinOperationsByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByLazyPairHeap_LeetCodeExamples_ReturnsMergeCountToSort(int[] nums, int expected) =>
        Assert.Equal(expected, MinimumPairRemovalToSortArrayISolution.MinOperationsByLazyPairHeap(nums));
}
