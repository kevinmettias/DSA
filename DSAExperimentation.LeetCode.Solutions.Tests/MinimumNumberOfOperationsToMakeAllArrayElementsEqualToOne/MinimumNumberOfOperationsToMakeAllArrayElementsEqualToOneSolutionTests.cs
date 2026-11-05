using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOne;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOne;

// Harness only. Both gcd strategies are
// MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution's - this file
// just pins them to the three shapes the
// window scan has to get right on its own: an array that already contains 1s (no
// window is ever searched), a single element (no window of length two exists at
// all), and an array whose overall gcd is not 1 (every window is scanned and none
// succeeds).
public sealed partial class MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 1, 1], 0 },
            { [1, 2, 3], 2 },
            { [2, 3], 2 },
            { [3, 5], 2 },
            { [4, 6, 9, 3, 3], 6 },
            { [1], 0 },
            { [6], -1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinOperationsBySubtractionGcd_Cases_ReturnsWindowCollapseThenSpreadCost(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution.MinOperationsBySubtractionGcd(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinOperationsByEuclideanGcd_Cases_ReturnsWindowCollapseThenSpreadCost(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution.MinOperationsByEuclideanGcd(nums));
}
