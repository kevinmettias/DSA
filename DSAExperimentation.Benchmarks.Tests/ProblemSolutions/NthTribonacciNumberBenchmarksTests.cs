using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NthTribonacciNumberBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the naive triple recursion against this repo's Memoizer-backed
// top-down DP - so a harness whose arms disagree is reporting two different tribonacci terms. The
// class carries no [GlobalSetup]: TermIndex is the whole workload, passed straight through to both
// arms, so the same TermIndex must answer both.
//
// Both arms return an int, so they are compared directly; the memoized arm's cache is per-call (the
// recursion hands the Memoizer down its own call tree), so one harness serves both arms in either
// order.
public sealed partial class NthTribonacciNumberBenchmarksTests
{
    private const int SmallestTermIndex = 20;

    [Fact]
    public void NaiveRecursion_AgreesWithMemoizedTopDown()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedTopDown(), harness.NaiveRecursion());
    }

    [Fact]
    public void MemoizedTopDown_AgreesWithNaiveRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursion(), harness.MemoizedTopDown());
    }

    private static NthTribonacciNumberBenchmarks BuildHarness() => new() { TermIndex = SmallestTermIndex };
}
