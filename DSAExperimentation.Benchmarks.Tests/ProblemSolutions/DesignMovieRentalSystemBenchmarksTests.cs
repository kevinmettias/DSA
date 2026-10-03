using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignMovieRentalSystemBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - filtering and sorting the whole catalogue per query
// against a per-movie search tree walked in order - so a harness whose arms disagree is timing two
// different problems. Setup builds one fixed entry table, so the same ShopsForTargetMovie must
// rebuild the same catalogue for both arms to be handed.
public sealed partial class DesignMovieRentalSystemBenchmarksTests
{
    private const int SmallestShopsForTargetMovie = 50;

    // LeetCode caps search() at the five cheapest shops; the target movie is stocked by far more
    // shops than that at either parameter value, so a correct search always fills the cap.
    private const int SearchResultCap = 5;

    [Fact]
    public void Setup_SameShopsForTargetMovie_RebuildsTheSameCatalogue()
    {
        Assert.Equal(SearchResultCap, BuildHarness().SortOnQuery().Count);
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().SortOnQuery()),
            AnswerGraphText.Of(BuildHarness().SortOnQuery()));
    }

    // search() answers with the five cheapest shops in price order, so the order is part of the
    // answer rather than an artifact of how each arm collected it - AnswerGraphText.Of pins it instead
    // of the outer-order-agnostic OfUnordered.
    [Fact]
    public void SortOnQuery_TargetMovieStockedByFiftyShops_AgreesWithBstMaintainedSorted()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.Of(harness.BstMaintainedSorted()),
            AnswerGraphText.Of(harness.SortOnQuery()));
    }

    [Fact]
    public void BstMaintainedSorted_TargetMovieStockedByFiftyShops_AgreesWithSortOnQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.Of(harness.SortOnQuery()),
            AnswerGraphText.Of(harness.BstMaintainedSorted()));
    }

    private static DesignMovieRentalSystemBenchmarks BuildHarness()
    {
        var harness = new DesignMovieRentalSystemBenchmarks { ShopsForTargetMovie = SmallestShopsForTargetMovie };
        harness.Setup();

        return harness;
    }
}
