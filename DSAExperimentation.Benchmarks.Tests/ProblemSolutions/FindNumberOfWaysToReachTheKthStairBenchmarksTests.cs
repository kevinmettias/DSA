using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindNumberOfWaysToReachTheKthStairBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the recurrence evaluated by plain unmemoized
// recursion against the same recurrence run through this repo's own Memoizer - so a harness whose
// arms disagree is counting two different sets of operation sequences. Both answers are one int, so
// they are compared directly.
//
// The benchmark has no [GlobalSetup] and no tuned input beyond TargetStair itself, which is set on
// the bare initializer: there is nothing to build before calling an arm, and TargetStair is the
// whole workload. It is deterministic per value, so both arms must agree on the same counted ways.
//
// The agreement is WEAK BY CONSTRUCTION here, and honestly so: Alice's reachable stairs are the ones
// she can actually stand on - 2^m - d after m up moves and d down moves, with the no-two-downs-in-a-
// row rule bounding d by m + 1 - and neither of the benchmark's own target stairs is in that set, so
// both arms count 0 on every run. Agreement therefore witnesses that both walks exhausted the same
// reachable region, not that they agreed on a positive count. What it does catch is an arm that
// counts a sequence the other does not.
public sealed partial class FindNumberOfWaysToReachTheKthStairBenchmarksTests
{
    // The smaller of the benchmark's [Params(1_000_000, 20_000_000)] target stairs, the one below
    // which the memoized arm's own dictionary allocation is not yet paying for itself.
    private const int SmallestTargetStair = 1_000_000;

    // No (m, d) pair satisfies 2^m - d == SmallestTargetStair within d <= m + 1: 2^20 is 1,048,576,
    // and 2^19 - 21 is 524,267, so the target falls in the gap between two reachable bands.
    private const int ExpectedWayCount = 0;

    [Fact]
    public void BruteRecursion_UnreachableStair_AgreesWithMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedWayCount, harness.BruteRecursion());
        Assert.Equal(harness.MemoizedRecurrence(), harness.BruteRecursion());
    }

    [Fact]
    public void MemoizedRecurrence_UnreachableStair_AgreesWithBruteRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedWayCount, harness.MemoizedRecurrence());
        Assert.Equal(harness.BruteRecursion(), harness.MemoizedRecurrence());
    }

    private static FindNumberOfWaysToReachTheKthStairBenchmarks BuildHarness() =>
        new() { TargetStair = SmallestTargetStair };
}
