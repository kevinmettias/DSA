using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SellingPiecesOfWoodBenchmarks (ARCHITECTURE 17.9): both arms share one
// recurrence and differ only in whether its recursive calls go through the memoizer, so a harness
// whose arms disagree is timing two different recurrences. Setup prices every (height, width) pair up
// to Size from one fixed seed, so the same Size must rebuild the same price index; neither arm writes
// to it, so one harness instance is safe to read twice in either order.
public sealed partial class SellingPiecesOfWoodBenchmarksTests
{
    private const int SmallestSize = 4;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().UnmemoizedRecursion(), BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_PricedUpToSize_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_PricedUpToSize_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static SellingPiecesOfWoodBenchmarks BuildHarness()
    {
        var harness = new SellingPiecesOfWoodBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
