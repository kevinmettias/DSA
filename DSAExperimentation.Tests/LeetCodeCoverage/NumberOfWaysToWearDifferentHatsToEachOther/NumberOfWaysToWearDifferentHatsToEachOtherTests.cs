using DSAExperimentation.LeetCode.NumberOfWaysToWearDifferentHatsToEachOther;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToWearDifferentHatsToEachOther;

// Harness only. Both the unmemoized recurrence and the Memoizer-routed one live in
// NumberOfWaysToWearDifferentHatsToEachOtherSolution; this file pins them to
// LeetCode's published examples plus the hand-checked cases the original test
// carried, so a disagreement names the strategy that broke.
public sealed class NumberOfWaysToWearDifferentHatsToEachOtherTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LeetCode example 1: person 2 can only wear hat 5, which forces the rest.
            { [[3, 4], [4, 5], [5]], 1 },

            // LeetCode example 2.
            { [[3, 5, 1], [3, 5]], 4 },

            // LeetCode example 3: four people over the same four hats is 4!.
            { [[1, 2, 3, 4], [1, 2, 3, 4], [1, 2, 3, 4], [1, 2, 3, 4]], 24 },

            // LeetCode example 4, the only one large enough to revisit states heavily.
            { [[1, 2, 3], [2, 3, 5, 6], [1, 3, 7, 9], [1, 8, 9], [2, 5, 7]], 111 },

            // Counts verified by direct enumeration of every distinct-hat assignment.
            { [[3, 4], [4, 5]], 3 },
            { [[1], [1]], 0 },
            { [[7]], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberWaysByBruteForceRecursion_LeetCodeExamples_CountsDistinctHatAssignments(
        int[][] hats, int expected) =>
        Assert.Equal(expected, NumberOfWaysToWearDifferentHatsToEachOtherSolution.NumberWaysByBruteForceRecursion(hats));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberWaysByMemoizedBitmask_LeetCodeExamples_CountsDistinctHatAssignments(
        int[][] hats, int expected) =>
        Assert.Equal(expected, NumberOfWaysToWearDifferentHatsToEachOtherSolution.NumberWaysByMemoizedBitmask(hats));
}
