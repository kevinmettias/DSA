using DSAExperimentation.LeetCode.ReachingPoints;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReachingPoints;

// Harness only: both strategies live in ReachingPointsSolution and are asserted against
// the same examples - including the naive subtractive reduction, which was previously
// only ever run by the benchmark, and which the benchmark had pinned to a source of
// (1, 1) so its divisibility tail was never exercised against a real source.
public sealed class ReachingPointsTests
{
    public static TheoryData<int, int, int, int, bool> Examples =>
        new()
        {
            { 1, 1, 3, 5, true },
            { 1, 1, 2, 2, false },
            { 1, 1, 1, 1, true },
            { 3, 5, 13, 5, true },
            { 1, 1, 1, 5, true },
            { 2, 3, 5, 3, true },
            { 1, 1, 2, 4, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableBySubtractiveReduction_LeetCodeExamples_MatchesExpectedReachability(
        int sx, int sy, int tx, int ty, bool expected) =>
        Assert.Equal(expected, ReachingPointsSolution.IsReachableBySubtractiveReduction(sx, sy, tx, ty));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableByModuloReduction_LeetCodeExamples_MatchesExpectedReachability(
        int sx, int sy, int tx, int ty, bool expected) =>
        Assert.Equal(expected, ReachingPointsSolution.IsReachableByModuloReduction(sx, sy, tx, ty));
}
