using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumCompatibilityScoreSum;

// LeetCode 1947. Maximum Compatibility Score Sum: the standard "assign the next
// student a still-unused mentor" bitmask DP, expressed as a memoized recursion over
// the tuple state (student, usedMentorMask) via this repo's own Memoizer - the
// identical (int, int) tuple-state shape
// MinimumCostToConnectTwoGroupsOfPointsTests already uses for LC 1595's (index,
// mask) recursion, just maximizing a per-question-match dot product instead of
// minimizing an edge cost, and over a square (every student paired with exactly one
// mentor) match instead of a rectangular one.
public sealed partial class MaximumCompatibilityScoreSumTests
{
    [Fact]
    public void MaxCompatibilitySum_LeetCodeExampleOne_ReturnsBestAssignmentScore()
    {
        int[][] students = [[1, 1, 0], [1, 0, 1], [0, 0, 1]];
        int[][] mentors = [[1, 0, 0], [0, 0, 1], [1, 1, 0]];

        var actual = MaxCompatibilitySum(students, mentors);

        Assert.Equal(8, actual);
    }

    [Fact]
    public void MaxCompatibilitySum_LeetCodeExampleTwo_AllPairsMismatch_ReturnsZero()
    {
        int[][] students = [[0, 0], [0, 0], [0, 0]];
        int[][] mentors = [[1, 1], [1, 1], [1, 1]];

        var actual = MaxCompatibilitySum(students, mentors);

        Assert.Equal(0, actual);
    }

    private static int MaxCompatibilitySum(int[][] students, int[][] mentors)
    {
        var m = students.Length;
        var score = BuildScoreMatrix(students, mentors);

        return Memoizer.Memoize<(int Student, int UsedMask), int>((0, 0), (state, best) =>
        {
            var (student, usedMask) = state;

            if (student == m)
            {
                return 0;
            }

            var top = int.MinValue;
            for (var mentor = 0; mentor < m; mentor++)
            {
                if ((usedMask & (1 << mentor)) != 0)
                {
                    continue;
                }

                var candidate = score[student][mentor] + best((student + 1, usedMask | (1 << mentor)));
                top = Math.Max(top, candidate);
            }

            return top;
        });
    }

    private static int[][] BuildScoreMatrix(int[][] students, int[][] mentors)
    {
        var m = students.Length;
        var questionCount = students[0].Length;
        var score = new int[m][];

        for (var i = 0; i < m; i++)
        {
            score[i] = BuildScoreRow(students[i], mentors, m, questionCount);
        }

        return score;
    }

    private static int[] BuildScoreRow(int[] student, int[][] mentors, int mentorCount, int questionCount)
    {
        var row = new int[mentorCount];

        for (var j = 0; j < mentorCount; j++)
        {
            row[j] = CountMatches(student, mentors[j], questionCount);
        }

        return row;
    }

    private static int CountMatches(int[] student, int[] mentor, int questionCount)
    {
        var matches = 0;

        for (var k = 0; k < questionCount; k++)
        {
            if (student[k] == mentor[k])
            {
                matches++;
            }
        }

        return matches;
    }
}
