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
    public static TheoryData<int, int, bool> Examples =>
        new()
        {
            { 6, 9, false },
            { 4, 7, true },
            { 1, 1, true },
            { 2, 2, true },
            { 12, 18, false },
            { 3, 3, false },
            { 5, 10, false },
            { 8, 12, true },
            { 2, 3, true },
            { 16, 24, true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableByBruteForceBfs_LeetCodeExamples_ReturnsWhetherTargetIsReachable(
        int targetX, int targetY, bool expected) =>
        Assert.Equal(expected, CheckIfPointIsReachableSolution.IsReachableByBruteForceBfs(targetX, targetY));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsReachableByGcd_LeetCodeExamples_ReturnsWhetherTargetIsReachable(
        int targetX, int targetY, bool expected) =>
        Assert.Equal(expected, CheckIfPointIsReachableSolution.IsReachableByGcd(targetX, targetY));
}
