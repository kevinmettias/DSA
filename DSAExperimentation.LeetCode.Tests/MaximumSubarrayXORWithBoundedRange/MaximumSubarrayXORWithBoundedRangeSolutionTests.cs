using DSAExperimentation.LeetCode.MaximumSubarrayXORWithBoundedRange;

namespace DSAExperimentation.LeetCode.Tests.MaximumSubarrayXORWithBoundedRange;

// Harness only. Both strategies are MaximumSubarrayXORWithBoundedRangeSolution's. The
// first two rows are LeetCode's published examples; the rest were computed by an
// independent quadratic scan and cover single-element answers (k = 0), a window that
// must shrink past an outlier, and a seeded 40-element array at three spreads.
public sealed partial class MaximumSubarrayXORWithBoundedRangeSolutionTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [5, 4, 5, 6], 2, 7 },
            { [5, 4, 5, 6], 1, 6 },
            { [7], 0, 7 },
            { [0, 0, 0], 0, 0 },

            // Adjacent spreads are 9, 8 and 7: k = 1 admits only single elements, while
            // k = 8 admits [2, 9] (XOR 11) but not [1, 10].
            { [1, 10, 2, 9], 1, 10 },
            { [1, 10, 2, 9], 8, 11 },
            { [3, 8, 3, 8, 1], 5, 11 },
            { [12, 9, 14, 3, 11, 6], 4, 14 },
            { SeededArray, 0, 62 },
            { SeededArray, 10, 62 },
            { SeededArray, 63, 63 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSubarrayXorByBruteForce_Examples_ReturnsTheBestXorWithinTheSpread(
        int[] nums, int maxSpread, int expected) =>
        Assert.Equal(expected, MaximumSubarrayXORWithBoundedRangeSolution.MaxSubarrayXorByBruteForce(nums, maxSpread));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSubarrayXorBySlidingWindowBitTrie_Examples_ReturnsTheBestXorWithinTheSpread(
        int[] nums, int maxSpread, int expected) =>
        Assert.Equal(expected, MaximumSubarrayXORWithBoundedRangeSolution.MaxSubarrayXorBySlidingWindowBitTrie(nums, maxSpread));

    // Forty values below 64, drawn once from Python's random.Random(3845); fixed here so the
    // expected answers above stay checkable by hand-run brute force.
    private static int[] SeededArray =>
    [
        38, 27, 1, 11, 15, 41, 11, 33, 7, 52, 28, 62, 8, 12, 44, 52, 61, 46, 34, 3,
        4, 15, 40, 1, 41, 35, 62, 54, 51, 10, 53, 38, 3, 51, 4, 57, 32, 38, 56, 7,
    ];
}
