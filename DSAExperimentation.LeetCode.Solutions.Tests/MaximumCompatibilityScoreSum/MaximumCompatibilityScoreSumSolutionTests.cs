using DSAExperimentation.LeetCode.Solutions.MaximumCompatibilityScoreSum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumCompatibilityScoreSum;

// Harness only. Both the unmemoized recursion and the Memoizer-backed one are
// MaximumCompatibilityScoreSumSolution's - this file just pins them to a case where
// greedily giving each student its own best mentor is wrong, which is what the
// search has to get right, and to a single student-mentor pair. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class MaximumCompatibilityScoreSumSolutionTests
{
    public static TheoryData<int[][], int[][], int> Cases =>
        new()
        {
            // Greedy-in-student-order fails here: student 0's best mentor is mentor 0
            // (2 matches), but mentor 0 is worth 3 to student 1 and mentor 1 is worth
            // nothing to it, so 1 + 3 beats 2 + 0.
            { [[1, 1, 0], [1, 1, 1]], [[1, 1, 1], [0, 0, 0]], 4 },

            // A single pair has exactly one assignment: its own match count.
            { [[1, 0, 1]], [[0, 0, 1]], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxCompatibilitySumByBruteForceRecursion_Cases_ReturnsBestAssignmentScore(
        int[][] students, int[][] mentors, int expected)
    {
        var actual = MaximumCompatibilityScoreSumSolution.MaxCompatibilitySumByBruteForceRecursion(students, mentors);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxCompatibilitySumByMemoizedBitmask_Cases_ReturnsBestAssignmentScore(
        int[][] students, int[][] mentors, int expected)
    {
        var actual = MaximumCompatibilityScoreSumSolution.MaxCompatibilitySumByMemoizedBitmask(students, mentors);

        Assert.Equal(expected, actual);
    }
}
