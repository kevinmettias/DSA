using DSAExperimentation.LeetCode.CheckIfPointIsReachable;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfPointIsReachable;

// Harness only. Both strategies are CheckIfPointIsReachableSolution's - the
// state-space breadth-first search that used to live untested as the benchmark
// baseline, and the gcd-is-a-power-of-two closed form - pinned to LeetCode's
// published examples plus the smallest targets that separate the two failure
// shapes: an odd gcd greater than one (3, 3), a gcd that is a power of two but
// not one (8, 12), and coprime coordinates (4, 7).
public sealed class CheckIfPointIsReachableTests
{
    public static TheoryData<ReachabilityCase> Examples =>
        new()
        {
            { new ReachabilityCase(6, 9, Expected: false) },
            { new ReachabilityCase(4, 7, Expected: true) },
            { new ReachabilityCase(1, 1, Expected: true) },
            { new ReachabilityCase(2, 2, Expected: true) },
            { new ReachabilityCase(12, 18, Expected: false) },
            { new ReachabilityCase(3, 3, Expected: false) },
            { new ReachabilityCase(5, 10, Expected: false) },
            { new ReachabilityCase(8, 12, Expected: true) },
            { new ReachabilityCase(2, 3, Expected: true) },
            { new ReachabilityCase(16, 24, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableByBruteForceBfs_LeetCodeExamples_ReturnsWhetherTargetIsReachable(
        ReachabilityCase example)
    {
        var reachable = CheckIfPointIsReachableSolution.IsReachableByBruteForceBfs(
            example.TargetX, example.TargetY);

        Assert.Equal(example.Expected, reachable);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableByGcd_LeetCodeExamples_ReturnsWhetherTargetIsReachable(
        ReachabilityCase example)
    {
        var reachable = CheckIfPointIsReachableSolution.IsReachableByGcd(example.TargetX, example.TargetY);

        Assert.Equal(example.Expected, reachable);
    }

    // One LeetCode example: the target coordinates and whether they are reachable.
    // The expected value is named at every construction site, so a row reads as the
    // case it is rather than as a bare `true` whose meaning is its position. Nested
    // because it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct ReachabilityCase(int TargetX, int TargetY, bool Expected);
}
