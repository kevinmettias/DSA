using DSAExperimentation.LeetCode.Solutions.FindTheMostCompetitiveSubsequence;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheMostCompetitiveSubsequence;

// Harness only: both strategies are FindTheMostCompetitiveSubsequenceSolution's.
// This file just pins them to the boundaries
// LeetCode never published - selectionLength equal to the whole array, a
// selectionLength of 1, a run of equal elements, and a single-element array -
// each of which lands on a different branch of the greedy rule.
public sealed partial class FindTheMostCompetitiveSubsequenceSolutionTests
{
    public static TheoryData<int[], int, int[]> Cases =>
        new()
        {
            // selectionLength is the whole array: nothing may be removed,
            // however descending.
            { [1, 2, 3, 4], 4, [1, 2, 3, 4] },
            { [4, 3, 2, 1], 4, [4, 3, 2, 1] },

            // selectionLength of 1 is just the minimum, and its first occurrence.
            { [5, 4, 3, 2, 1], 1, [1] },
            { [2, 1, 3, 1], 1, [1] },

            // Equal elements are never strictly greater, so nothing pops and the
            // length bound alone decides - the prefix survives.
            { [2, 2, 2], 2, [2, 2] },

            // Already non-decreasing: the removals all come off the tail.
            { [1, 2, 3, 4, 5], 3, [1, 2, 3] },

            // The shortest input the constraints allow.
            { [7], 1, [7] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MostCompetitiveByRepeatedRemoval_Cases_ReturnsLexicographicallySmallestSubsequence(
        int[] nums, int selectionLength, int[] expected)
    {
        var actual =
            FindTheMostCompetitiveSubsequenceSolution.MostCompetitiveByRepeatedRemoval(
                nums, selectionLength);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MostCompetitiveByMonotonicStack_Cases_ReturnsLexicographicallySmallestSubsequence(
        int[] nums, int selectionLength, int[] expected)
    {
        var actual =
            FindTheMostCompetitiveSubsequenceSolution.MostCompetitiveByMonotonicStack(
                nums, selectionLength);

        Assert.Equal(expected, actual);
    }
}
