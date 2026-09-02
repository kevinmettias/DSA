using DSAExperimentation.LeetCode.TopKFrequentElements;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TopKFrequentElements;

// Harness only: both strategies live in TopKFrequentElementsSolution and are
// asserted against the same examples. Order is not part of LeetCode's contract
// for this problem, so equality is checked as a set.
public sealed class TopKFrequentElementsTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 1, 1, 2, 2, 3], 2, [1, 2] },
            { [1], 1, [1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindTopKFrequentByFullSort_LeetCodeExamples_ReturnsTheKMostFrequentValues(
        int[] nums, int k, int[] expected) =>
        Assert.Equal(
            new HashSet<int>(expected),
            new HashSet<int>(TopKFrequentElementsSolution.FindTopKFrequentByFullSort(nums, k)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindTopKFrequentBySizeKMinHeap_LeetCodeExamples_ReturnsTheKMostFrequentValues(
        int[] nums, int k, int[] expected) =>
        Assert.Equal(
            new HashSet<int>(expected),
            new HashSet<int>(TopKFrequentElementsSolution.FindTopKFrequentBySizeKMinHeap(nums, k)));
}
