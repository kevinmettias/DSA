using DSAExperimentation.LeetCode.SingleNumberIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SingleNumberIII;

// Harness only: both strategies live in SingleNumberIIISolution and are asserted
// against the same examples. LC260 accepts either order for the two singletons,
// so each result is sorted before comparing.
public sealed class SingleNumberIIITests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { new[] { 1, 2, 1, 3, 2, 5 }, new[] { 3, 5 } },
            { new[] { -1, 0 }, new[] { -1, 0 } },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSingleNumbersByBruteForce_LeetCodeExamples_ReturnsBothUniqueValues(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, SingleNumberIIISolution.FindSingleNumbersByBruteForce(nums).OrderBy(x => x));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSingleNumbersByHashMapFrequencyCount_LeetCodeExamples_ReturnsBothUniqueValues(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, SingleNumberIIISolution.FindSingleNumbersByHashMapFrequencyCount(nums).OrderBy(x => x));
}
