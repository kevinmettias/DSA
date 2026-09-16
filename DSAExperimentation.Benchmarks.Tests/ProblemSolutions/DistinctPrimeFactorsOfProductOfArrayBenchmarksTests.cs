using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistinctPrimeFactorsOfProductOfArrayBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - building the whole product and trial
// dividing it against factoring each element and unioning the results - so a harness whose arms
// disagree is timing two different problems. Setup draws every value from [2, 1000], so no element
// is 1, the product always has at least one prime factor, and no prime factor can exceed the
// largest element: the count is therefore at least one and at most the 168 primes below 1000. The
// values come from one fixed seed, so the same Length must rebuild the same array.
public sealed partial class DistinctPrimeFactorsOfProductOfArrayBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int MinimumDistinctPrimeFactorCount = 1;
    private const int PrimeCountBelowOneThousand = 168;

    [Fact]
    public void Setup_ValuesAboveOne_FactorIntoAtLeastOnePrimeAndRebuildTheSameWorkload()
    {
        var harness = BuildHarness();
        var factors = harness.ProductThenTrialDivide();

        Assert.InRange(factors, MinimumDistinctPrimeFactorCount, PrimeCountBelowOneThousand);
        Assert.Equal(factors, BuildHarness().ProductThenTrialDivide());
    }

    [Fact]
    public void ProductThenTrialDivide_SeededValueArray_AgreesWithPerElementFactorSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PerElementFactorSet(), harness.ProductThenTrialDivide());
    }

    [Fact]
    public void PerElementFactorSet_SeededValueArray_AgreesWithProductThenTrialDivide()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ProductThenTrialDivide(), harness.PerElementFactorSet());
    }

    private static DistinctPrimeFactorsOfProductOfArrayBenchmarks BuildHarness()
    {
        var harness = new DistinctPrimeFactorsOfProductOfArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
