using DSAExperimentation.LeetCode.Solutions.FindTheLongestValidObstacleCourseAtEachPosition;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheLongestValidObstacleCourseAtEachPosition;

// Harness only: both strategies live in
// FindTheLongestValidObstacleCourseAtEachPositionSolution and are asserted against the
// same cases: a single obstacle, the all-equal run (where non-decreasing beats strictly
// increasing) and the strictly decreasing case where every answer is 1. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class FindTheLongestValidObstacleCourseAtEachPositionSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [5], [1] },
            { [7, 7, 7], [1, 2, 3] },
            { [5, 4, 3], [1, 1, 1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestObstacleCourseByDynamicProgramming_Cases_ReturnsExpectedLengths(
        int[] obstacles, int[] expected) =>
        Assert.Equal(
            expected,
            FindTheLongestValidObstacleCourseAtEachPositionSolution.LongestObstacleCourseByDynamicProgramming(
                obstacles));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestObstacleCourseByPatienceSorting_Cases_ReturnsExpectedLengths(
        int[] obstacles, int[] expected) =>
        Assert.Equal(
            expected,
            FindTheLongestValidObstacleCourseAtEachPositionSolution.LongestObstacleCourseByPatienceSorting(
                obstacles));
}
