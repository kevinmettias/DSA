using DSAExperimentation.LeetCode.MaximumTotalBeautyOfTheGardens;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumTotalBeautyOfTheGardens;

// Harness only: both strategies live in MaximumTotalBeautyOfTheGardensSolution.
// The linear descent used to exist solely as a benchmark baseline with nothing
// asserting it, so it is pinned to exactly the same examples as the composed
// sort-and-binary-search strategy here.
public sealed class MaximumTotalBeautyOfTheGardensTests
{
    public static TheoryData<int[], long, int, int, int, long> Examples =>
        new()
        {
            // LeetCode's first published example.
            { [1, 3, 1, 1], 7L, 6, 12, 1, 14L },

            // LeetCode's second published example.
            { [2, 4, 5, 3], 10L, 5, 2, 6, 30L },

            // Every garden is already complete, so no split can earn partial credit
            // however large the partial rate is.
            { [10, 10, 10], 0L, 5, 3, 100, 9L },

            // A single garden, already complete, with nothing to spend.
            { [5], 0L, 5, 10, 4, 10L },

            // Nothing can be completed (each garden needs 9 more), so the whole
            // answer is the partial term on a minimum that one flower cannot lift.
            { [1, 1], 1L, 10, 100, 1, 1L },

            // The budget completes every garden exactly, and doing so beats every
            // partial-credit split.
            { [1, 1, 1], 6L, 3, 5, 2, 15L },

            // Completing nothing is right: spreading the budget evenly raises the
            // minimum to 2, which the partial rate rewards more than a full garden.
            { [1, 1, 1, 1], 4L, 10, 1, 3, 6L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumBeautyByLinearSearchOnAnswer_LeetCodeExamples_ReturnsMaxAchievableBeauty(
        int[] flowers, long newFlowers, int target, int full, int partial, long expected) =>
        Assert.Equal(
            expected,
            MaximumTotalBeautyOfTheGardensSolution.MaximumBeautyByLinearSearchOnAnswer(
                flowers, newFlowers, target, new BeautyWeights(full, partial)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumBeautyBySortAndBinarySearch_LeetCodeExamples_ReturnsMaxAchievableBeauty(
        int[] flowers, long newFlowers, int target, int full, int partial, long expected) =>
        Assert.Equal(
            expected,
            MaximumTotalBeautyOfTheGardensSolution.MaximumBeautyBySortAndBinarySearch(
                flowers, newFlowers, target, new BeautyWeights(full, partial)));
}
