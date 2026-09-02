using DSAExperimentation.LeetCode.MinimumMovesToCaptureTheQueen;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumMovesToCaptureTheQueen;

// Harness only: the algorithms live in MinimumMovesToCaptureTheQueenSolution. One
// test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke (TwoSumTests precedent).
public sealed class MinimumMovesToCaptureTheQueenTests
{
    public static TheoryData<int, int, int, int, int, int, int> Examples =>
        new()
        {
            { 1, 1, 8, 8, 2, 3, 2 },
            { 5, 3, 3, 4, 5, 2, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByDestinationEnumeration_LeetCodeExamples_ReturnsFewestMoves(
        int a, int b, int c, int d, int e, int f, int expected) =>
        Assert.Equal(expected, MinimumMovesToCaptureTheQueenSolution.MinMovesByDestinationEnumeration(a, b, c, d, e, f));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByLineOfSight_LeetCodeExamples_ReturnsFewestMoves(
        int a, int b, int c, int d, int e, int f, int expected) =>
        Assert.Equal(expected, MinimumMovesToCaptureTheQueenSolution.MinMovesByLineOfSight(a, b, c, d, e, f));
}
