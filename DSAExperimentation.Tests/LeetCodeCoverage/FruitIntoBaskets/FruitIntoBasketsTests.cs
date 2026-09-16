using DSAExperimentation.LeetCode.FruitIntoBaskets;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FruitIntoBaskets;

// Harness only: both strategies live in FruitIntoBasketsSolution and are asserted
// against the same examples - LeetCode's three published ones, a single tree type,
// a single tree, and a run where the answer sits at the very end of the array.
public sealed partial class FruitIntoBasketsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 1], 3 },
            { [0, 1, 2, 2], 3 },
            { [1, 2, 3, 2, 2], 4 },
            { [5, 5, 5, 5], 4 },
            { [7], 1 },
            { [3, 3, 3, 1, 2, 1, 1, 2, 3, 3, 4], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalFruitByPerStartRescan_LeetCodeExamples_ReturnsLongestTwoTypeRun(
        int[] fruits, int expected) =>
        Assert.Equal(expected, FruitIntoBasketsSolution.TotalFruitByPerStartRescan(fruits));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalFruitBySlidingWindow_LeetCodeExamples_ReturnsLongestTwoTypeRun(
        int[] fruits, int expected) =>
        Assert.Equal(expected, FruitIntoBasketsSolution.TotalFruitBySlidingWindow(fruits));
}
