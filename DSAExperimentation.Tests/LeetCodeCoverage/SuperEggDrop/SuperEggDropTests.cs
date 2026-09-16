using DSAExperimentation.LeetCode.SuperEggDrop;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SuperEggDrop;

// Harness only: both strategies are SuperEggDropSolution's. This file pins them to
// LeetCode's published examples plus the small hand-checkable cases that fix the
// recurrence's boundaries - one egg (the answer is the floor count), one floor, and
// the two classic "maximum floors coverable in d drops" points (2 eggs / 10 floors
// = 1+2+3+4, 3 eggs / 25 floors = C(5,1)+C(5,2)+C(5,3)).
public sealed partial class SuperEggDropTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 1, 2, 2 },
            { 2, 6, 3 },
            { 3, 14, 4 },
            { 1, 1, 1 },
            { 2, 1, 1 },
            { 2, 2, 2 },
            { 2, 10, 4 },
            { 3, 25, 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByLinearScan_LeetCodeExamples_ReturnsMinimumWorstCaseMoves(
        int eggs, int floors, int expected)
    {
        var actual = SuperEggDropSolution.MinMovesByLinearScan(eggs, floors);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByBinarySearch_LeetCodeExamples_ReturnsMinimumWorstCaseMoves(
        int eggs, int floors, int expected)
    {
        var actual = SuperEggDropSolution.MinMovesByBinarySearch(eggs, floors);

        Assert.Equal(expected, actual);
    }
}
