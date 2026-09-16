using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FibonacciNumberBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same recurrence - the naive O(2^n) double recursion against this repo's
// Memoizer-backed top-down DP - so a harness whose arms disagree is computing two different
// sequences. Both arms return a plain int. The class has no [GlobalSetup]: the [Params] sequence
// index is the whole workload, so each arm is called on its own harness and the smallest index the
// attribute lists is used.
public sealed partial class FibonacciNumberBenchmarksTests
{
    private const int SmallestSequenceIndex = 20;

    // LC 509's own published value for the twentieth term.
    private const int ExpectedTerm = 6765;

    [Fact]
    public void NaiveRecursion_TwentiethTerm_AgreesWithMemoizedTopDown()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTerm, harness.NaiveRecursion());

        Assert.Equal(harness.MemoizedTopDown(), harness.NaiveRecursion());
    }

    [Fact]
    public void MemoizedTopDown_TwentiethTerm_AgreesWithNaiveRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTerm, harness.MemoizedTopDown());

        Assert.Equal(harness.NaiveRecursion(), harness.MemoizedTopDown());
    }

    private static FibonacciNumberBenchmarks BuildHarness() =>
        new() { SequenceIndex = SmallestSequenceIndex };
}
