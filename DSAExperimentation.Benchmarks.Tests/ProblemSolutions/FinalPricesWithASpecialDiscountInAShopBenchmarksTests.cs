using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FinalPricesWithASpecialDiscountInAShopBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the O(n^2) forward scan against the O(n)
// monotonic-stack pass over this repo's own Stack<int> - so a harness whose arms disagree is
// discounting two different shops. Both arms return the final price of every item, and the position
// in that array is the item, so it is the answer rather than an incidental order and the two arrays
// are compared as ordered sequences. The price array is drawn from a fixed seed, so the same Length
// must rebuild the same prices, with one final price per item.
public sealed partial class FinalPricesWithASpecialDiscountInAShopBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePrices()
    {
        Assert.Equal(SmallestLength, BuildHarness().BruteForce().Length);

        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));
    }

    [Fact]
    public void BruteForce_SeededPrices_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.MonotonicStack()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void MonotonicStack_SeededPrices_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.MonotonicStack()));
    }

    private static FinalPricesWithASpecialDiscountInAShopBenchmarks BuildHarness()
    {
        var harness = new FinalPricesWithASpecialDiscountInAShopBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
