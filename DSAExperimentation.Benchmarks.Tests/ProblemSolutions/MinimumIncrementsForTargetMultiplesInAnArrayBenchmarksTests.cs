using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumIncrementsForTargetMultiplesInAnArrayBenchmarks (ARCHITECTURE
// 17.9): both arms are MinimumIncrementsForTargetMultiplesInAnArraySolution's, the same methods
// MinimumIncrementsForTargetMultiplesInAnArrayTests proves correct, and both answer the same
// question - the fewest increments that make every target multiple covered. Arms that disagree
// are timing two different problems.
//
// Setup's workload is seeded, so the same parameters must rebuild the same workload; a workload
// that moved between runs would make two published numbers incomparable.
public sealed partial class MinimumIncrementsForTargetMultiplesInAnArrayBenchmarksTests
{
    // The smallest declared [Params] value: it already exercises the full target bitmask space,
    // and the memoized arm recurses once per nums element.
    private const int SmallestNumsCount = 100;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().BottomUpBitmaskDp(),
            BuildHarness().BottomUpBitmaskDp());

    [Fact]
    public void BottomUpBitmaskDp_AgreesWithMemoizedBitmaskDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedBitmaskDp(), harness.BottomUpBitmaskDp());
    }

    [Fact]
    public void MemoizedBitmaskDp_AgreesWithBottomUpBitmaskDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BottomUpBitmaskDp(), harness.MemoizedBitmaskDp());
    }

    private static MinimumIncrementsForTargetMultiplesInAnArrayBenchmarks BuildHarness()
    {
        var harness = new MinimumIncrementsForTargetMultiplesInAnArrayBenchmarks { NumsCount = SmallestNumsCount };
        harness.Setup();

        return harness;
    }
}
