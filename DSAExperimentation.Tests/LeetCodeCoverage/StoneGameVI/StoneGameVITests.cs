using DSAExperimentation.LeetCode.StoneGameVI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameVI;

// Harness only: both strategies are StoneGameVISolution's. This file pins them to
// LeetCode's three published examples plus the boundaries it never published - a
// single stone, a run of equal swings whose tie-break order must not matter, and a
// lopsided board where Bob's values decide every stone.
public sealed class StoneGameVITests
{
    // 1 when Alice ends ahead, -1 when Bob does, 0 on a tie.
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            // LeetCode's three published examples.
            { [1, 3], [2, 1], 1 },
            { [1, 2], [3, 1], 0 },
            { [2, 4, 3], [1, 6, 7], -1 },

            // One stone: Alice takes it and Bob never plays.
            { [5], [3], 1 },

            // Equal swings throughout, so the two sorts may order them differently
            // and still have to agree - the answer depends on which swings land on
            // Bob's turns, not on which stone does.
            { [2, 1], [1, 2], 0 },
            { [3, 2, 1], [1, 2, 3], 1 },

            // Bob's values dominate every swing, so every stone he takes costs
            // Alice more than the ones she keeps are worth.
            { [1, 1], [10, 10], -1 },

            // An odd count leaves Alice with the extra pick.
            { [4, 4, 4], [1, 1, 1], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WinnerByArraySortGreedy_LeetCodeExamples_ReportsOptimalPlayOutcome(
        int[] aliceValues, int[] bobValues, int expected) =>
        Assert.Equal(expected, StoneGameVISolution.WinnerByArraySortGreedy(aliceValues, bobValues));

    [Theory]
    [MemberData(nameof(Examples))]
    public void WinnerByMergeSortGreedy_LeetCodeExamples_ReportsOptimalPlayOutcome(
        int[] aliceValues, int[] bobValues, int expected) =>
        Assert.Equal(expected, StoneGameVISolution.WinnerByMergeSortGreedy(aliceValues, bobValues));
}
