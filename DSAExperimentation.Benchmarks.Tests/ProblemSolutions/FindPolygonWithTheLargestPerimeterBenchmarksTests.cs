using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindPolygonWithTheLargestPerimeterBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - trying every subset of size >= 3 against a
// sorted greedy scan - so a harness whose arms disagree has maximized two different quantities over
// the same bag of sides. Both answers are one long, so they are compared directly.
//
// Setup's sides are the same seeded draws for a given SideCount, so the same parameters must rebuild
// the same bag and the same largest valid perimeter. Sixteen sides each at least 1 make the total
// comfortably larger than twice the longest one on every run, so a polygon exists and the answer is
// the full perimeter rather than -1.
public sealed partial class FindPolygonWithTheLargestPerimeterBenchmarksTests
{
    // The smaller of Setup's [Params(16, 20)] side counts, the one whose subset arm is affordable.
    private const int SmallestSideCount = 16;

    [Fact]
    public void Setup_SameSideCount_RebuildsTheSameLargestPerimeter() =>
        Assert.Equal(BuildHarness().BruteForceSubsets(), BuildHarness().BruteForceSubsets());

    [Fact]
    public void BruteForceSubsets_SeededSides_AgreesWithSortedRunningSum()
    {
        var harness = BuildHarness();

        Assert.True(harness.BruteForceSubsets() > 0);
        Assert.Equal(harness.SortedRunningSum(), harness.BruteForceSubsets());
    }

    [Fact]
    public void SortedRunningSum_SeededSides_AgreesWithBruteForceSubsets()
    {
        var harness = BuildHarness();

        Assert.True(harness.SortedRunningSum() > 0);
        Assert.Equal(harness.BruteForceSubsets(), harness.SortedRunningSum());
    }

    private static FindPolygonWithTheLargestPerimeterBenchmarks BuildHarness()
    {
        var harness = new FindPolygonWithTheLargestPerimeterBenchmarks { SideCount = SmallestSideCount };
        harness.Setup();

        return harness;
    }
}
