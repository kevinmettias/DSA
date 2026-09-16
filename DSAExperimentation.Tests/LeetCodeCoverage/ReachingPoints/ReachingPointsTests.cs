using DSAExperimentation.LeetCode.ReachingPoints;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReachingPoints;

// Harness only: both strategies live in ReachingPointsSolution and are asserted against
// the same examples - including the naive subtractive reduction, which was previously
// only ever run by the benchmark, and which the benchmark had pinned to a source of
// (1, 1) so its divisibility tail was never exercised against a real source.
public sealed partial class ReachingPointsTests
{
    public static TheoryData<ReachabilityExample> Examples =>
        new()
        {
            { new ReachabilityExample(Sx: 1, Sy: 1, Tx: 3, Ty: 5, Expected: true) },
            { new ReachabilityExample(Sx: 1, Sy: 1, Tx: 2, Ty: 2, Expected: false) },
            { new ReachabilityExample(Sx: 1, Sy: 1, Tx: 1, Ty: 1, Expected: true) },
            { new ReachabilityExample(Sx: 3, Sy: 5, Tx: 13, Ty: 5, Expected: true) },
            { new ReachabilityExample(Sx: 1, Sy: 1, Tx: 1, Ty: 5, Expected: true) },
            { new ReachabilityExample(Sx: 2, Sy: 3, Tx: 5, Ty: 3, Expected: true) },
            { new ReachabilityExample(Sx: 1, Sy: 1, Tx: 2, Ty: 4, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableBySubtractiveReduction_LeetCodeExamples_MatchesExpectedReachability(
        ReachabilityExample example)
    {
        var actual = ReachingPointsSolution.IsReachableBySubtractiveReduction(
            example.Sx, example.Sy, example.Tx, example.Ty);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableByModuloReduction_LeetCodeExamples_MatchesExpectedReachability(
        ReachabilityExample example)
    {
        var actual = ReachingPointsSolution.IsReachableByModuloReduction(
            example.Sx, example.Sy, example.Tx, example.Ty);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the point the walk starts from, the point it must reach,
    // and whether it can. The four coordinates are named for the role they play, so
    // the source pair and the target pair cannot be exchanged unread.
    public readonly record struct ReachabilityExample(int Sx, int Sy, int Tx, int Ty, bool Expected);
}
