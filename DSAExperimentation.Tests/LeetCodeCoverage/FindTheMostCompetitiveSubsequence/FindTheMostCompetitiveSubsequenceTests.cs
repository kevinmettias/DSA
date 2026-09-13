using DSAExperimentation.LeetCode.FindTheMostCompetitiveSubsequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheMostCompetitiveSubsequence;

// Harness only: both strategies are FindTheMostCompetitiveSubsequenceSolution's.
// This file just pins them to LeetCode's published examples plus the boundaries
// LeetCode never published - k equal to the whole array, k of 1, a run of equal
// elements, and a single-element array - each of which lands on a different branch
// of the greedy rule.
public sealed class FindTheMostCompetitiveSubsequenceTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            // LeetCode's two published examples.
            { [3, 5, 2, 6], 2, [2, 6] },
            { [2, 4, 3, 3, 5, 4, 9, 6], 4, [2, 3, 3, 4] },

            // k is the whole array: nothing may be removed, however descending.
            { [1, 2, 3, 4], 4, [1, 2, 3, 4] },
            { [4, 3, 2, 1], 4, [4, 3, 2, 1] },

            // k of 1 is just the minimum, and the first occurrence of it.
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
    [MemberData(nameof(Examples))]
    public void MostCompetitiveByRepeatedRemoval_LeetCodeExamples_ReturnsLexicographicallySmallestSubsequence(
        int[] nums, int k, int[] expected) =>
        Assert.Equal(
            expected,
            FindTheMostCompetitiveSubsequenceSolution.MostCompetitiveByRepeatedRemoval(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MostCompetitiveByMonotonicStack_LeetCodeExamples_ReturnsLexicographicallySmallestSubsequence(
        int[] nums, int k, int[] expected) =>
        Assert.Equal(
            expected,
            FindTheMostCompetitiveSubsequenceSolution.MostCompetitiveByMonotonicStack(nums, k));
}
