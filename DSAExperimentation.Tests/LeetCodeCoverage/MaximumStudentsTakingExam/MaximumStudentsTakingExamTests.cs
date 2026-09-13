using DSAExperimentation.LeetCode.MaximumStudentsTakingExam;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumStudentsTakingExam;

// Harness only: both strategies are MaximumStudentsTakingExamSolution's - the
// unmemoized bitmask recursion and the same recursion routed through Memoizer -
// pinned here to LeetCode's three published examples plus an all-broken classroom
// (nobody can sit), a single row (no diagonal constraint ever applies) and a single
// column (only the row-above rule can bite).
public sealed class MaximumStudentsTakingExamTests
{
    public static TheoryData<char[][], int> Examples =>
        new()
        {
            {
                [
                    ['#', '.', '#', '#', '.', '#'],
                    ['.', '#', '#', '#', '#', '.'],
                    ['#', '.', '#', '#', '.', '#'],
                ],
                4
            },
            {
                [
                    ['.', '#'],
                    ['#', '#'],
                    ['#', '.'],
                    ['#', '#'],
                    ['.', '#'],
                ],
                3
            },
            {
                [
                    ['#', '.', '.', '.', '#'],
                    ['.', '#', '.', '#', '.'],
                    ['.', '.', '#', '.', '.'],
                    ['.', '#', '.', '#', '.'],
                    ['#', '.', '.', '.', '#'],
                ],
                10
            },
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
    [MemberData(nameof(Examples))]
    public void MaxStudentsByBruteForceRecursion_LeetCodeExamples_ReturnsMostSeatableStudents(
        char[][] seats, int expected) =>
        Assert.Equal(expected, MaximumStudentsTakingExamSolution.MaxStudentsByBruteForceRecursion(seats));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxStudentsByMemoizedBitmask_LeetCodeExamples_ReturnsMostSeatableStudents(
        char[][] seats, int expected) =>
        Assert.Equal(expected, MaximumStudentsTakingExamSolution.MaxStudentsByMemoizedBitmask(seats));
}
