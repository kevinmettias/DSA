using DSAExperimentation.LeetCode.KokoEatingBananas;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KokoEatingBananas;

// Harness only. Both strategies are KokoEatingBananasSolution's - the hand-rolled
// bisection that used to live only in the benchmark's baseline arm, and the
// BinarySearch.LowerBound walk over the feasibility sequence the test used to inline.
public sealed class KokoEatingBananasTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            // LC examples 1-3.
            { [3, 6, 7, 11], 8, 4 },
            { [30, 11, 23, 4, 20], 5, 30 },
            { [30, 11, 23, 4, 20], 6, 23 },

            // One pile with exactly one hour: the answer is the pile itself.
            { [1], 1, 1 },
            { [1000000000], 1, 1000000000 },

            // Hours to spare, so the slowest speed already clears every pile.
            { [3, 6, 7, 11], 100, 1 },

            // hourBudget equals the pile count, so every pile must go in a single
            // hour and the answer is the largest pile.
            { [4, 4, 4, 4], 4, 4 },

            // Ceilings matter: at speed 2 the piles cost 2 + 1 + 2 = 5 hours, at speed
            // 3 they cost 2 + 1 + 1 = 4.
            { [4, 2, 3], 4, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinEatingSpeedByManualBisection_LeetCodeExamples_ReturnsSmallestFeasibleSpeed(
        int[] piles, int hourBudget, int expected)
    {
        var actual =
            KokoEatingBananasSolution.MinEatingSpeedByManualBisection(piles, hourBudget);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinEatingSpeedBySequenceLowerBound_LeetCodeExamples_ReturnsSmallestFeasibleSpeed(
        int[] piles, int hourBudget, int expected)
    {
        var actual =
            KokoEatingBananasSolution.MinEatingSpeedBySequenceLowerBound(piles, hourBudget);

        Assert.Equal(expected, actual);
    }
}
