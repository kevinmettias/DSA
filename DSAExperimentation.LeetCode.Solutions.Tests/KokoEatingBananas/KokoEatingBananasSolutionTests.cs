using DSAExperimentation.LeetCode.Solutions.KokoEatingBananas;

namespace DSAExperimentation.LeetCode.Solutions.Tests.KokoEatingBananas;

// Harness only. Both strategies are KokoEatingBananasSolution's - the hand-rolled
// bisection that used to live only in the benchmark's baseline arm, and the
// predicate search over the speed range that the test used to inline as a
// feasibility sequence.
public sealed partial class KokoEatingBananasSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            // One pile with exactly one hour: the answer is the pile itself.
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
    [MemberData(nameof(Cases))]
    public void MinEatingSpeedByManualBisection_Cases_ReturnsSmallestFeasibleSpeed(
        int[] piles, int hourBudget, int expected)
    {
        var actual =
            KokoEatingBananasSolution.MinEatingSpeedByManualBisection(piles, hourBudget);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinEatingSpeedByPredicateSearch_Cases_ReturnsSmallestFeasibleSpeed(
        int[] piles, int hourBudget, int expected)
    {
        var actual =
            KokoEatingBananasSolution.MinEatingSpeedByPredicateSearch(piles, hourBudget);

        Assert.Equal(expected, actual);
    }
}
