using DSAExperimentation.LeetCode.MaximumCompatibilityScoreSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumCompatibilityScoreSum;

// Harness only. Both the unmemoized recursion and the Memoizer-backed one are
// MaximumCompatibilityScoreSumSolution's - this file just pins them to LeetCode's
// published examples plus a case where greedily giving each student its own best
// mentor is wrong, which is what the search has to get right.
public sealed class MaximumCompatibilityScoreSumTests
{
    public static TheoryData<int[][], int[][], int> Examples =>
        new()
        {
            // LeetCode example 1: the best pairing is 0-2, 1-0, 2-1 for 3 + 2 + 3.
            { [[1, 1, 0], [1, 0, 1], [0, 0, 1]], [[1, 0, 0], [0, 0, 1], [1, 1, 0]], 8 },

            // LeetCode example 2: every student disagrees with every mentor on every
            // question, so no pairing scores at all.
            { [[0, 0], [0, 0], [0, 0]], [[1, 1], [1, 1], [1, 1]], 0 },

            // Greedy-in-student-order fails here: student 0's best mentor is mentor 0
            // (2 matches), but mentor 0 is worth 3 to student 1 and mentor 1 is worth
            // nothing to it, so 1 + 3 beats 2 + 0.
            { [[1, 1, 0], [1, 1, 1]], [[1, 1, 1], [0, 0, 0]], 4 },

            // A single pair has exactly one assignment: its own match count.
            { [[1, 0, 1]], [[0, 0, 1]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxCompatibilitySumByBruteForceRecursion_LeetCodeExamples_ReturnsBestAssignmentScore(
        int[][] students, int[][] mentors, int expected) =>
        Assert.Equal(
            expected,
            MaximumCompatibilityScoreSumSolution.MaxCompatibilitySumByBruteForceRecursion(students, mentors));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxCompatibilitySumByMemoizedBitmask_LeetCodeExamples_ReturnsBestAssignmentScore(
        int[][] students, int[][] mentors, int expected) =>
        Assert.Equal(
            expected,
            MaximumCompatibilityScoreSumSolution.MaxCompatibilitySumByMemoizedBitmask(students, mentors));
}
