using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestStringWithSwapsBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job. The answer is a rearrangement of the same characters into the same
// positions, so it is always exactly as long as the source: that length is checked on the baseline
// so an arm that dropped or duplicated a position cannot agree with a sibling that did the same.
public sealed partial class SmallestStringWithSwapsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void AdjacencyListBfs_TwoHundredIndexSource_KeepsTheSourceLength() =>
        Assert.Equal(SmallestLength, BuildHarness().AdjacencyListBfs().Length);

    private static SmallestStringWithSwapsBenchmarks BuildHarness()
    {
        var harness = new SmallestStringWithSwapsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
