using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSubarraySumAfterAtMostKSwapsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the from-scratch re-sort of every window's
// inside/outside values against the order-statistics Fenwick sweep - so a harness whose arms disagree
// is timing two different problems. Setup draws the values from one fixed seed and derives the swap
// budget from the length, so the same Length must rebuild the same workload; otherwise two published
// numbers were never comparable in the first place.
public sealed partial class MaximumSubarraySumAfterAtMostKSwapsBenchmarksTests
{
    private const int SmallestLength = 30;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_ThirdOfLengthAsSwapBudget_AgreesWithOrderStatisticsFenwick()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.OrderStatisticsFenwick(), harness.BruteForce());
    }

    [Fact]
    public void OrderStatisticsFenwick_ThirdOfLengthAsSwapBudget_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.OrderStatisticsFenwick());
    }

    private static MaximumSubarraySumAfterAtMostKSwapsBenchmarks BuildHarness()
    {
        var harness = new MaximumSubarraySumAfterAtMostKSwapsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
