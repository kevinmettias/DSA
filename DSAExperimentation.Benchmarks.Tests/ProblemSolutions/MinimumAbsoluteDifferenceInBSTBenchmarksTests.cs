using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumAbsoluteDifferenceInBSTBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a recursive in-order walk that materializes
// every value and scans for the smallest adjacent gap, against this repo's InOrderTraversal hooks
// carrying only the previous value across visits - so a harness whose arms disagree is timing two
// different problems. Setup builds a balanced BST over Size consecutive integers, so the in-order
// walk is exactly 0, 1, ..., Size - 1 and the smallest adjacent gap is 1 for every Size; that is
// what makes a rebuilt tree's answer decisive rather than merely self-consistent.
public sealed partial class MinimumAbsoluteDifferenceInBSTBenchmarksTests
{
    private const int SmallestSize = 100;

    // Consecutive integers in the tree leave no room for a smaller positive gap between two
    // distinct node values.
    private const int ExpectedMinimumGap = 1;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameBalancedBst()
    {
        Assert.Equal(ExpectedMinimumGap, BuildHarness().RecursiveScan());
        Assert.Equal(ExpectedMinimumGap, BuildHarness().RecursiveScan());
    }

    [Fact]
    public void RecursiveScan_ConsecutiveIntegers_FindsTheUnitGapAndAgreesWithInOrderTraversalHooks()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMinimumGap, harness.RecursiveScan());
        Assert.Equal(harness.InOrderTraversalHooks(), harness.RecursiveScan());
    }

    [Fact]
    public void InOrderTraversalHooks_ConsecutiveIntegers_FindsTheUnitGapAndAgreesWithRecursiveScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMinimumGap, harness.InOrderTraversalHooks());
        Assert.Equal(harness.RecursiveScan(), harness.InOrderTraversalHooks());
    }

    private static MinimumAbsoluteDifferenceInBSTBenchmarks BuildHarness()
    {
        var harness = new MinimumAbsoluteDifferenceInBSTBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
