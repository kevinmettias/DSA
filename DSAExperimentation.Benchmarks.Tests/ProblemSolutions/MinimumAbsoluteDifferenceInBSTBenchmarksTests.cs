using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumAbsoluteDifferenceInBSTBenchmarks (ARCHITECTURE 17.9): Setup builds a
// balanced BST over Size consecutive integers, so the in-order walk is exactly 0, 1, ..., Size - 1
// and the smallest adjacent gap is 1 for every Size - an expected value derived from the workload's
// construction rather than from either arm, which the generic agreement check cannot supply.
public sealed partial class MinimumAbsoluteDifferenceInBSTBenchmarksTests
{
    private const int SmallestSize = 100;

    // Consecutive integers in the tree leave no room for a smaller positive gap between two
    // distinct node values.
    private const int ExpectedMinimumGap = 1;

    [Fact]
    public void RecursiveScan_ConsecutiveIntegers_FindsTheUnitGap() =>
        Assert.Equal(ExpectedMinimumGap, BuildHarness().RecursiveScan());

    private static MinimumAbsoluteDifferenceInBSTBenchmarks BuildHarness()
    {
        var harness = new MinimumAbsoluteDifferenceInBSTBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
