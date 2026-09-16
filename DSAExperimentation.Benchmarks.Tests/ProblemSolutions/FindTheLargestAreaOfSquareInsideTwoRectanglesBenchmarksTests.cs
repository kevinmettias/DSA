using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheLargestAreaOfSquareInsideTwoRectanglesBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for one question - every pair compared against the pairs a
// sort lets the pruned arm skip - so a harness whose arms disagree is timing two different
// problems. Setup builds the rectangle list from one fixed seed, so the same RectangleCount must
// rebuild the same arrays.
public sealed partial class FindTheLargestAreaOfSquareInsideTwoRectanglesBenchmarksTests
{
    private const int SmallestRectangleCount = 50;

    [Fact]
    public void Setup_SameRectangleCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForcePairs(), BuildHarness().BruteForcePairs());

    [Fact]
    public void BruteForcePairs_SmallestRectangleCount_AgreesWithSortedPrunedPairs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortedPrunedPairs(), harness.BruteForcePairs());
    }

    [Fact]
    public void SortedPrunedPairs_SmallestRectangleCount_AgreesWithBruteForcePairs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForcePairs(), harness.SortedPrunedPairs());
    }

    private static FindTheLargestAreaOfSquareInsideTwoRectanglesBenchmarks BuildHarness()
    {
        var harness = new FindTheLargestAreaOfSquareInsideTwoRectanglesBenchmarks
        {
            RectangleCount = SmallestRectangleCount,
        };
        harness.Setup();

        return harness;
    }
}
