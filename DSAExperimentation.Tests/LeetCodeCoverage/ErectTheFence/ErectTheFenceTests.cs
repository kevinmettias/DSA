using DSAExperimentation.LeetCode.ErectTheFence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ErectTheFence;

// Harness only: both strategies live in ErectTheFenceSolution and are asserted
// against the same examples, including the all-collinear case that has no
// interior at all. Order is not part of LeetCode's answer, so examples compare
// as sets.
public sealed class ErectTheFenceTests
{
    public static TheoryData<(int X, int Y)[], (int X, int Y)[]> Examples =>
        new()
        {
            {
                [(1, 1), (2, 2), (2, 0), (2, 4), (3, 3), (4, 2)],
                [(1, 1), (2, 0), (4, 2), (3, 3), (2, 4)]
            },
            {
                [(1, 2), (2, 2), (4, 2)],
                [(1, 2), (2, 2), (4, 2)]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void OuterTreesByBruteForceHalfPlaneScan_LeetCodeExamples_ReturnsFencePoints(
        (int X, int Y)[] points, (int X, int Y)[] expected) =>
        Assert.Equal(
            new HashSet<(int X, int Y)>(expected),
            new HashSet<(int X, int Y)>(ErectTheFenceSolution.OuterTreesByBruteForceHalfPlaneScan(points)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void OuterTreesByMonotoneChain_LeetCodeExamples_ReturnsFencePoints(
        (int X, int Y)[] points, (int X, int Y)[] expected) =>
        Assert.Equal(
            new HashSet<(int X, int Y)>(expected),
            new HashSet<(int X, int Y)>(ErectTheFenceSolution.OuterTreesByMonotoneChain(points)));
}
