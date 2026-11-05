using DSAExperimentation.LeetCode.Solutions.NumberOfWaysToWearDifferentHatsToEachOther;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysToWearDifferentHatsToEachOther;

// Harness only. Both the unmemoized recurrence and the Memoizer-routed one live in
// NumberOfWaysToWearDifferentHatsToEachOtherSolution; this file pins them to
// the hand-checked cases the original test
// carried, so a disagreement names the strategy that broke.
public sealed partial class NumberOfWaysToWearDifferentHatsToEachOtherSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // LeetCode's example 4, which the catalog does not carry, and the only row
            // large enough to revisit states heavily.
            { [[1, 2, 3], [2, 3, 5, 6], [1, 3, 7, 9], [1, 8, 9], [2, 5, 7]], 111 },

            // Counts verified by direct enumeration of every distinct-hat assignment.
            { [[3, 4], [4, 5]], 3 },
            { [[1], [1]], 0 },
            { [[7]], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberWaysByBruteForceRecursion_Cases_CountsDistinctHatAssignments(
        int[][] hats, int expected) =>
        Assert.Equal(expected, NumberOfWaysToWearDifferentHatsToEachOtherSolution.NumberWaysByBruteForceRecursion(hats));

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberWaysByMemoizedBitmask_Cases_CountsDistinctHatAssignments(
        int[][] hats, int expected) =>
        Assert.Equal(expected, NumberOfWaysToWearDifferentHatsToEachOtherSolution.NumberWaysByMemoizedBitmask(hats));
}
