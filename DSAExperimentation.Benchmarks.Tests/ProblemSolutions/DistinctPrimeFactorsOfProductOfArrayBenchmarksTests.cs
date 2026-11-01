using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistinctPrimeFactorsOfProductOfArrayBenchmarks (ARCHITECTURE 17.9): a bound
// the workload's shape fixes without either arm. Setup draws every value from [2, 1000], so no
// element is 1, the product always has at least one prime factor, and no prime factor can exceed
// the largest element: the count is therefore at least one and at most the 168 primes below 1000.
public sealed partial class DistinctPrimeFactorsOfProductOfArrayBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int MinimumDistinctPrimeFactorCount = 1;
    private const int PrimeCountBelowOneThousand = 168;

    [Fact]
    public void ProductThenTrialDivide_ValuesAboveOne_CountsAtLeastOneAndAtMostEveryPrimeBelowOneThousand() =>
        Assert.InRange(
            BuildHarness().ProductThenTrialDivide(),
            MinimumDistinctPrimeFactorCount,
            PrimeCountBelowOneThousand);

    private static DistinctPrimeFactorsOfProductOfArrayBenchmarks BuildHarness()
    {
        var harness = new DistinctPrimeFactorsOfProductOfArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
