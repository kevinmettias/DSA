using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ClimbingStairsIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the unmemoized step-tree recursion against the same recurrence
// remembered per step - so a harness whose arms disagree is timing two different problems. Both arms
// answer with a single long, the cheapest way up, so agreement between them says the two walks
// settled on the same cost for the same step costs.
public sealed partial class ClimbingStairsIIBenchmarksTests
{
    private const int SmallestStepCount = 15;

    [Fact]
    public void Setup_SameStepCount_RebuildsTheSameCosts()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The cost array is private and both arms reduce it to one total, so two harnesses built from
        // the same StepCount reporting the same cheapest cost for each strategy is the reading the
        // rebuild can be pinned to: one seed means the same StepCount costs in the same order, and the
        // same costs mean the same two totals.
        Assert.Equal(first.BruteForceRecursion(), second.BruteForceRecursion());
        Assert.Equal(first.MemoizedRecurrence(), second.MemoizedRecurrence());
    }

    [Fact]
    public void BruteForceRecursion_SeededFifteenStepCosts_AgreesWithMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecurrence(), harness.BruteForceRecursion());
    }

    [Fact]
    public void MemoizedRecurrence_SeededFifteenStepCosts_AgreesWithBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRecursion(), harness.MemoizedRecurrence());
    }

    private static ClimbingStairsIIBenchmarks BuildHarness()
    {
        var harness = new ClimbingStairsIIBenchmarks { StepCount = SmallestStepCount };
        harness.Setup();

        return harness;
    }
}
