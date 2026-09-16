using DSAExperimentation.LeetCode.MaximumTotalBeautyOfTheGardens;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumTotalBeautyOfTheGardens;

// Harness only: both strategies live in MaximumTotalBeautyOfTheGardensSolution.
// The linear descent used to exist solely as a benchmark baseline with nothing
// asserting it, so it is pinned to exactly the same examples as the composed
// sort-and-binary-search strategy here.
public sealed class MaximumTotalBeautyOfTheGardensTests
{
    public static TheoryData<BeautyExample> Examples =>
        new()
        {
            // LeetCode's first published example.
            { new BeautyExample(Flowers: [1, 3, 1, 1], NewFlowers: 7L, Target: 6, Full: 12, Partial: 1, Expected: 14L) },

            // LeetCode's second published example.
            { new BeautyExample(Flowers: [2, 4, 5, 3], NewFlowers: 10L, Target: 5, Full: 2, Partial: 6, Expected: 30L) },

            // Every garden is already complete, so no split can earn partial credit
            // however large the partial rate is.
            { new BeautyExample(Flowers: [10, 10, 10], NewFlowers: 0L, Target: 5, Full: 3, Partial: 100, Expected: 9L) },

            // A single garden, already complete, with nothing to spend.
            { new BeautyExample(Flowers: [5], NewFlowers: 0L, Target: 5, Full: 10, Partial: 4, Expected: 10L) },

            // Nothing can be completed (each garden needs 9 more), so the whole
            // answer is the partial term on a minimum that one flower cannot lift.
            { new BeautyExample(Flowers: [1, 1], NewFlowers: 1L, Target: 10, Full: 100, Partial: 1, Expected: 1L) },

            // The budget completes every garden exactly, and doing so beats every
            // partial-credit split.
            { new BeautyExample(Flowers: [1, 1, 1], NewFlowers: 6L, Target: 3, Full: 5, Partial: 2, Expected: 15L) },

            // Completing nothing is right: spreading the budget evenly raises the
            // minimum to 2, which the partial rate rewards more than a full garden.
            { new BeautyExample(Flowers: [1, 1, 1, 1], NewFlowers: 4L, Target: 10, Full: 1, Partial: 3, Expected: 6L) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumBeautyByLinearSearchOnAnswer_LeetCodeExamples_ReturnsMaxAchievableBeauty(
        BeautyExample example)
    {
        var actual =
            MaximumTotalBeautyOfTheGardensSolution.MaximumBeautyByLinearSearchOnAnswer(
                example.Flowers, example.NewFlowers, example.Target,
                new BeautyWeights(example.Full, example.Partial));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumBeautyBySortAndBinarySearch_LeetCodeExamples_ReturnsMaxAchievableBeauty(
        BeautyExample example)
    {
        var actual =
            MaximumTotalBeautyOfTheGardensSolution.MaximumBeautyBySortAndBinarySearch(
                example.Flowers, example.NewFlowers, example.Target,
                new BeautyWeights(example.Full, example.Partial));

        Assert.Equal(example.Expected, actual);
    }

    // One example as one argument: the six values that describe a single case. They
    // travel together - a row IS one case - and passed separately they made a
    // six-parameter signature that could only be read by counting commas.
    public readonly record struct BeautyExample(
        int[] Flowers, long NewFlowers, int Target, int Full, int Partial, long Expected);
}
