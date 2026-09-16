using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfSwapsToMakeTheStringBalancedBenchmarks (ARCHITECTURE
// 17.9): both arms are MinimumNumberOfSwapsToMakeTheStringBalancedSolution's, the same methods
// MinimumNumberOfSwapsToMakeTheStringBalancedTests proves correct, and both return the fewest
// swaps that balance the string. The backward scan and the stack scan are competing strategies
// for that one number, so arms that disagree are timing two different problems.
public sealed partial class MinimumNumberOfSwapsToMakeTheStringBalancedBenchmarksTests
{
    // The smallest declared [Params] value: Setup splits it into an equal pair of runs, so the
    // backward scan already pays a full rescan of the closer run at this size.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().BackwardScan(),
            BuildHarness().BackwardScan());

    [Fact]
    public void BackwardScan_AgreesWithStackScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StackScan(), harness.BackwardScan());
    }

    [Fact]
    public void StackScan_AgreesWithBackwardScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BackwardScan(), harness.StackScan());
    }

    private static MinimumNumberOfSwapsToMakeTheStringBalancedBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfSwapsToMakeTheStringBalancedBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
