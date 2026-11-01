using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignMovieRentalSystemBenchmarks (ARCHITECTURE 17.9): the generic arm check
// compares the arms only with each other, so what stays here is the answer's documented size, derived
// from the catalogue's construction rather than from either arm.
public sealed partial class DesignMovieRentalSystemBenchmarksTests
{
    private const int SmallestShopsForTargetMovie = 50;

    // LeetCode caps search() at the five cheapest shops; the target movie is stocked by far more
    // shops than that at either parameter value, so a correct search always fills the cap.
    private const int SearchResultCap = 5;

    [Fact]
    public void SortOnQuery_TargetMovieStockedByFiftyShops_FillsTheSearchCap() =>
        Assert.Equal(SearchResultCap, BuildHarness().SortOnQuery().Count);

    private static DesignMovieRentalSystemBenchmarks BuildHarness()
    {
        var harness = new DesignMovieRentalSystemBenchmarks { ShopsForTargetMovie = SmallestShopsForTargetMovie };
        harness.Setup();

        return harness;
    }
}
