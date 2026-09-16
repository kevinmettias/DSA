using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfDistinctRollSequencesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the un-memoized recursion over (day, secondLastRoll,
// lastRoll) against the same recurrence routed through this repo's own Memoizer - so a harness whose
// arms disagree is counting two different sets of sequences. The class carries no [GlobalSetup]:
// SequenceLength is the whole workload, passed straight through to both arms, so the same
// SequenceLength must answer both.
//
// Both arms return a long, so they are compared directly; the memoized arm's cache is per-call, so
// one harness serves both arms in either order.
public sealed partial class NumberOfDistinctRollSequencesBenchmarksTests
{
    private const int SmallestSequenceLength = 6;

    [Fact]
    public void BruteForceRecursion_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.BruteForceRecursion());
    }

    [Fact]
    public void MemoizedRecursion_AgreesWithBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRecursion(), harness.MemoizedRecursion());
    }

    private static NumberOfDistinctRollSequencesBenchmarks BuildHarness() =>
        new() { SequenceLength = SmallestSequenceLength };
}
