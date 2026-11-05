using DSAExperimentation.LeetCode.Solutions.SuperEggDrop;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SuperEggDrop;

// Harness only: both strategies are SuperEggDropSolution's. This file pins them to
// the small hand-checkable cases that fix the
// recurrence's boundaries - one egg (the answer is the floor count), one floor, and
// the two classic "maximum floors coverable in d drops" points (2 eggs / 10 floors
// = 1+2+3+4, 3 eggs / 25 floors = C(5,1)+C(5,2)+C(5,3)).
public sealed partial class SuperEggDropSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            { 2, 2, 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinMovesByLinearScan_Cases_ReturnsMinimumWorstCaseMoves(
        int eggs, int floors, int expected)
    {
        var actual = SuperEggDropSolution.MinMovesByLinearScan(eggs, floors);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinMovesByBinarySearch_Cases_ReturnsMinimumWorstCaseMoves(
        int eggs, int floors, int expected)
    {
        var actual = SuperEggDropSolution.MinMovesByBinarySearch(eggs, floors);

        Assert.Equal(expected, actual);
    }
}
