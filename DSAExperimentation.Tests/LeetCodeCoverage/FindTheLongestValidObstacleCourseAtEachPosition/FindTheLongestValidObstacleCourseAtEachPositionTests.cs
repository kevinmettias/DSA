using DSAExperimentation.LeetCode.FindTheLongestValidObstacleCourseAtEachPosition;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheLongestValidObstacleCourseAtEachPosition;

// Harness only: both strategies live in
// FindTheLongestValidObstacleCourseAtEachPositionSolution and are asserted against the
// same examples, including the all-equal run (where non-decreasing beats strictly
// increasing) and the strictly decreasing case where every answer is 1.
public sealed class FindTheLongestValidObstacleCourseAtEachPositionTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3, 2], [1, 2, 3, 3] },
            { [2, 2, 1], [1, 2, 1] },
            { [3, 1, 5, 6, 4, 2], [1, 1, 2, 3, 2, 2] },
            { [5], [1] },
            { [7, 7, 7], [1, 2, 3] },
            { [5, 4, 3], [1, 1, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestObstacleCourseByDynamicProgramming_LeetCodeExamples_ReturnsExpectedLengths(
        int[] obstacles, int[] expected) =>
        Assert.Equal(
            expected,
            FindTheLongestValidObstacleCourseAtEachPositionSolution.LongestObstacleCourseByDynamicProgramming(
                obstacles));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestObstacleCourseByPatienceSorting_LeetCodeExamples_ReturnsExpectedLengths(
        int[] obstacles, int[] expected) =>
        Assert.Equal(
            expected,
            FindTheLongestValidObstacleCourseAtEachPositionSolution.LongestObstacleCourseByPatienceSorting(
                obstacles));
}
