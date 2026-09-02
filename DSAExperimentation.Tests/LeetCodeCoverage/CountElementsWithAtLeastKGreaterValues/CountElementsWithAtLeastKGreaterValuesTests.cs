using DSAExperimentation.LeetCode.CountElementsWithAtLeastKGreaterValues;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountElementsWithAtLeastKGreaterValues;

// Harness only. Both strategies are CountElementsWithAtLeastKGreaterValuesSolution's -
// this file just pins them to LeetCode's published examples.
public sealed class CountElementsWithAtLeastKGreaterValuesTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [3, 1, 2], 1, 2 },
            { [5, 5, 5], 2, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountQualifiedByBruteForce_LeetCodeExamples_ReturnsQualifiedCount(int[] nums, int k, int expected) =>
        Assert.Equal(expected, CountElementsWithAtLeastKGreaterValuesSolution.CountQualifiedByBruteForce(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountQualifiedBySortedUpperBound_LeetCodeExamples_ReturnsQualifiedCount(int[] nums, int k, int expected) =>
        Assert.Equal(expected, CountElementsWithAtLeastKGreaterValuesSolution.CountQualifiedBySortedUpperBound(nums, k));
}
