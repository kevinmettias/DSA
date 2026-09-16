using DSAExperimentation.LeetCode.MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOne;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOne;

// Harness only. Both gcd strategies are
// MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution's - this file
// just pins them to LeetCode's published examples plus the three shapes the
// window scan has to get right on its own: an array that already contains 1s (no
// window is ever searched), a single element (no window of length two exists at
// all), and an array whose overall gcd is not 1 (every window is scanned and none
// succeeds).
public sealed partial class MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 6, 3, 4], 4 },
            { [2, 10, 6, 14], -1 },
            { [1, 1, 1], 0 },
            { [1, 2, 3], 2 },
            { [2, 3], 2 },
            { [3, 5], 2 },
            { [4, 6, 9, 3, 3], 6 },
            { [2, 4, 6, 8], -1 },
            { [1], 0 },
            { [6], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsBySubtractionGcd_LeetCodeExamples_ReturnsWindowCollapseThenSpreadCost(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution.MinOperationsBySubtractionGcd(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByEuclideanGcd_LeetCodeExamples_ReturnsWindowCollapseThenSpreadCost(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution.MinOperationsByEuclideanGcd(nums));
}
