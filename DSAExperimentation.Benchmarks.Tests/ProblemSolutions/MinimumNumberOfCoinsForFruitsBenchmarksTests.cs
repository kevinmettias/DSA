using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfCoinsForFruitsBenchmarks (ARCHITECTURE 17.9): both arms
// are MinimumNumberOfCoinsForFruitsSolution's, the same methods
// MinimumNumberOfCoinsForFruitsTests proves correct, and both return the fewest coins that buy
// the whole price list. The brute-force DP and the segment-tree DP answer the same recurrence -
// only the window minimum's cost differs - so arms that disagree are timing two different
// problems.
public sealed partial class MinimumNumberOfCoinsForFruitsBenchmarksTests
{
    // The smallest declared [Params] value: random prices keep the window from collapsing to a
    // fixed shape at any length, so the shorter list reaches the same comparison more cheaply.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().BruteForceDp(),
            BuildHarness().BruteForceDp());

    [Fact]
    public void BruteForceDp_AgreesWithSegmentTreeDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeDp(), harness.BruteForceDp());
    }

    [Fact]
    public void SegmentTreeDp_AgreesWithBruteForceDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceDp(), harness.SegmentTreeDp());
    }

    private static MinimumNumberOfCoinsForFruitsBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfCoinsForFruitsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
