using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinCostClimbingStairsBenchmarks (ARCHITECTURE 17.9): its three arms are competing
// strategies for the same question - the uncached recurrence against this repo's own Memoizer over the
// same recurrence against the O(1)-space iterative fold - so a harness whose arms disagree is timing two
// different problems. All three return the minimum cost as an int, so they are compared directly, and
// all three only read the cost array, so one harness is safe to read twice in any order. Setup draws the
// per-step costs from one fixed seed, so the same StepCount must rebuild the same cost array and with it
// the same minimum.
public sealed partial class MinCostClimbingStairsBenchmarksTests
{
    private const int SmallestStepCount = 20;

    [Fact]
    public void Setup_SameStepCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IterativeConstantSpace(),
            BuildHarness().IterativeConstantSpace());

    [Fact]
    public void NaiveRecursive_SeededCosts_AgreesWithMemoizedRecurrenceAndIterativeConstantSpace()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecurrence(), harness.NaiveRecursive());
        Assert.Equal(harness.IterativeConstantSpace(), harness.NaiveRecursive());
    }

    [Fact]
    public void MemoizedRecurrence_SeededCosts_AgreesWithNaiveRecursiveAndIterativeConstantSpace()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursive(), harness.MemoizedRecurrence());
        Assert.Equal(harness.IterativeConstantSpace(), harness.MemoizedRecurrence());
    }

    [Fact]
    public void IterativeConstantSpace_SeededCosts_AgreesWithNaiveRecursiveAndMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursive(), harness.IterativeConstantSpace());
        Assert.Equal(harness.MemoizedRecurrence(), harness.IterativeConstantSpace());
    }

    private static MinCostClimbingStairsBenchmarks BuildHarness()
    {
        var harness = new MinCostClimbingStairsBenchmarks { StepCount = SmallestStepCount };
        harness.Setup();

        return harness;
    }
}
