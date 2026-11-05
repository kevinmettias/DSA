using DSAExperimentation.LeetCode.Solutions.MaximumStudentsTakingExam;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumStudentsTakingExam;

// Harness only: both strategies are MaximumStudentsTakingExamSolution's - the
// unmemoized bitmask recursion and the same recursion routed through Memoizer -
// pinned here to an all-broken classroom
// (nobody can sit), a single row (no diagonal constraint ever applies) and a single
// column (only the row-above rule can bite).
public sealed partial class MaximumStudentsTakingExamSolutionTests
{
    public static TheoryData<char[][], int> Cases =>
        new()
        {
            {
                [
                    ['#', '#', '#'],
                    ['#', '#', '#'],
                ],
                0
            },
            { [['.', '.', '.', '.', '.']], 3 },
            {
                [
                    ['.'],
                    ['.'],
                    ['.'],
                ],
                3
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxStudentsByBruteForceRecursion_Cases_ReturnsMostSeatableStudents(
        char[][] seats, int expected) =>
        Assert.Equal(expected, MaximumStudentsTakingExamSolution.MaxStudentsByBruteForceRecursion(seats));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxStudentsByMemoizedBitmask_Cases_ReturnsMostSeatableStudents(
        char[][] seats, int expected) =>
        Assert.Equal(expected, MaximumStudentsTakingExamSolution.MaxStudentsByMemoizedBitmask(seats));
}
