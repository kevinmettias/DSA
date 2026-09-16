using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShoppingOffersBenchmarks (ARCHITECTURE 17.9): both arms price the same basket
// against the same offers and prices, so a harness whose arms disagree is timing two different
// baskets. Setup derives the need vector from NeedsPerItem alone, and the price list and the
// pairwise offers are static readonly fields neither arm writes to, so one harness instance is safe
// to call twice in either order and the same NeedsPerItem must rebuild the same basket.
public sealed partial class ShoppingOffersBenchmarksTests
{
    private const int SmallestNeedsPerItem = 4;

    [Fact]
    public void Setup_SameNeedsPerItem_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_PairwiseOffers_AgreesWithMemoizedDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedDfs(), harness.BruteForce());
    }

    [Fact]
    public void MemoizedDfs_PairwiseOffers_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.MemoizedDfs());
    }

    private static ShoppingOffersBenchmarks BuildHarness()
    {
        var harness = new ShoppingOffersBenchmarks { NeedsPerItem = SmallestNeedsPerItem };
        harness.Setup();

        return harness;
    }
}
