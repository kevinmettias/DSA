using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindNumberOfWaysToReachTheKthStairBenchmarks (ARCHITECTURE 17.9): the generic
// arms check only compares the arms with each other, so this pins the baseline's count to a value
// derived independently of every arm.
//
// The benchmark has no [GlobalSetup] and no tuned input beyond TargetStair itself, which is set on
// the bare initializer: there is nothing to build before calling an arm, and TargetStair is the
// whole workload.
//
// Alice's reachable stairs are the ones she can actually stand on - 2^m - d after m up moves and d
// down moves, with the no-two-downs-in-a-row rule bounding d by m + 1 - and the smallest target
// stair is not in that set, so the count is 0.
public sealed partial class FindNumberOfWaysToReachTheKthStairBenchmarksTests
{
    // The smaller of the benchmark's [Params(1_000_000, 20_000_000)] target stairs, the one below
    // which the memoized arm's own dictionary allocation is not yet paying for itself.
    private const int SmallestTargetStair = 1_000_000;

    // No (m, d) pair satisfies 2^m - d == SmallestTargetStair within d <= m + 1: 2^20 is 1,048,576,
    // and 2^19 - 21 is 524,267, so the target falls in the gap between two reachable bands.
    private const int ExpectedWayCount = 0;

    [Fact]
    public void BruteRecursion_UnreachableStair_CountsNoWays() =>
        Assert.Equal(ExpectedWayCount, BuildHarness().BruteRecursion());

    private static FindNumberOfWaysToReachTheKthStairBenchmarks BuildHarness() =>
        new() { TargetStair = SmallestTargetStair };
}
