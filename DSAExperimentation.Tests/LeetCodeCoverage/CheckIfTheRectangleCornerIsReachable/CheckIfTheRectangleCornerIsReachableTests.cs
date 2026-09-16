using DSAExperimentation.LeetCode.CheckIfTheRectangleCornerIsReachable;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfTheRectangleCornerIsReachable;

// Harness only. Both strategies are
// CheckIfTheRectangleCornerIsReachableSolution's - this file just pins them to
// LeetCode's published examples, including the two-circle chain (Example 3) that only
// blocks the path once its members are unioned together, not individually.
public sealed partial class CheckIfTheRectangleCornerIsReachableTests
{
    public static TheoryData<CornerPathCase> Examples =>
        new()
        {
            { new CornerPathCase(3, 4, new[] { new[] { 2, 1, 1 } }, Expected: true) },
            { new CornerPathCase(3, 3, new[] { new[] { 1, 1, 2 } }, Expected: false) },
            { new CornerPathCase(3, 3, new[] { new[] { 2, 1, 1 }, new[] { 1, 2, 1 } }, Expected: false) },
            { new CornerPathCase(4, 4, new[] { new[] { 5, 5, 1 } }, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableByBoundaryFloodFill_LeetCodeExamples_ReturnsWhetherACornerToCornerPathExists(
        CornerPathCase example)
    {
        var reachable =
            CheckIfTheRectangleCornerIsReachableSolution.IsReachableByBoundaryFloodFill(
                example.XCorner, example.YCorner, example.Circles);

        Assert.Equal(example.Expected, reachable);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableByDisjointSet_LeetCodeExamples_ReturnsWhetherACornerToCornerPathExists(
        CornerPathCase example)
    {
        var reachable = CheckIfTheRectangleCornerIsReachableSolution.IsReachableByDisjointSet(
            example.XCorner, example.YCorner, example.Circles);

        Assert.Equal(example.Expected, reachable);
    }

    // One LeetCode example: the destination corner and the circles that block the
    // rectangle, plus whether a corner-to-corner path survives them. The expected
    // value is named at every construction site, so a row reads as the case it is
    // rather than as a bare `true` whose meaning is its position. Nested because it
    // is only ever used inside this test class - it is this harness's own vocabulary,
    // not a type another file would import.
    public readonly record struct CornerPathCase(int XCorner, int YCorner, int[][] Circles, bool Expected);
}
