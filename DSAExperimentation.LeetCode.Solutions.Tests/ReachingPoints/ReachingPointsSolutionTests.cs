using DSAExperimentation.LeetCode.Solutions.ReachingPoints;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ReachingPoints;

// Harness only: both strategies live in ReachingPointsSolution and are asserted against
// the same cases - including the naive subtractive reduction, which was previously
// only ever run by the benchmark, and which the benchmark had pinned to a source of
// (1, 1) so its divisibility tail was never exercised against a real source.
public sealed partial class ReachingPointsSolutionTests
{
    public static TheoryData<ReachabilityCase> Cases =>
        new()
        {
            { new ReachabilityCase(Sx: 3, Sy: 5, Tx: 13, Ty: 5, Expected: true) },
            { new ReachabilityCase(Sx: 1, Sy: 1, Tx: 1, Ty: 5, Expected: true) },
            { new ReachabilityCase(Sx: 2, Sy: 3, Tx: 5, Ty: 3, Expected: true) },
            { new ReachabilityCase(Sx: 1, Sy: 1, Tx: 2, Ty: 4, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsReachableBySubtractiveReduction_Cases_MatchesExpectedReachability(
        ReachabilityCase testCase)
    {
        var actual = ReachingPointsSolution.IsReachableBySubtractiveReduction(
            testCase.Sx, testCase.Sy, testCase.Tx, testCase.Ty);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsReachableByModuloReduction_Cases_MatchesExpectedReachability(
        ReachabilityCase testCase)
    {
        var actual = ReachingPointsSolution.IsReachableByModuloReduction(
            testCase.Sx, testCase.Sy, testCase.Tx, testCase.Ty);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the point the walk starts from, the point it must reach,
    // and whether it can. The four coordinates are named for the role they play, so
    // the source pair and the target pair cannot be exchanged unread.
    public readonly record struct ReachabilityCase(int Sx, int Sy, int Tx, int Ty, bool Expected);
}
