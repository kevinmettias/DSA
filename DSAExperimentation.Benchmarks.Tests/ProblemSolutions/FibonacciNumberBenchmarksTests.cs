using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FibonacciNumberBenchmarks (ARCHITECTURE 17.9): the generic arms check only
// compares the arms with each other, so this pins the baseline to LC 509's own published twentieth
// term, a value derived independently of every arm. The class has no [GlobalSetup]: the [Params]
// sequence index is the whole workload, and the smallest index the attribute lists is used.
public sealed partial class FibonacciNumberBenchmarksTests
{
    private const int SmallestSequenceIndex = 20;

    // LC 509's own published value for the twentieth term.
    private const int ExpectedTerm = 6765;

    [Fact]
    public void NaiveRecursion_TwentiethTerm_ReturnsLeetCodesValue() =>
        Assert.Equal(ExpectedTerm, BuildHarness().NaiveRecursion());

    private static FibonacciNumberBenchmarks BuildHarness() =>
        new() { SequenceIndex = SmallestSequenceIndex };
}
