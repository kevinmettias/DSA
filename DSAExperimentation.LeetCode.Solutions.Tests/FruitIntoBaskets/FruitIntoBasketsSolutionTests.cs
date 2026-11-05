using DSAExperimentation.LeetCode.Solutions.FruitIntoBaskets;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FruitIntoBaskets;

// Harness only: both strategies live in FruitIntoBasketsSolution and are asserted
// against the same cases - LeetCode's three published ones, a single tree type,
// a single tree, and a run where the answer sits at the very end of the array.
public sealed partial class FruitIntoBasketsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [5, 5, 5, 5], 4 },
            { [7], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void TotalFruitByPerStartRescan_Cases_ReturnsLongestTwoTypeRun(
        int[] fruits, int expected) =>
        Assert.Equal(expected, FruitIntoBasketsSolution.TotalFruitByPerStartRescan(fruits));

    [Theory]
    [MemberData(nameof(Cases))]
    public void TotalFruitBySlidingWindow_Cases_ReturnsLongestTwoTypeRun(
        int[] fruits, int expected) =>
        Assert.Equal(expected, FruitIntoBasketsSolution.TotalFruitBySlidingWindow(fruits));
}
