using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximizeNumberOfNiceDivisorsBenchmarks (ARCHITECTURE 17.9): both arms are
// competing strategies for one question - the largest count of nice divisors reachable from the
// given number of prime factors - so a harness whose arms disagree is timing two different problems.
// This class has no [GlobalSetup] and no tunable state beyond its own [Params]: the prime-factor
// count IS the whole input, so each arm is called on the parameter directly and there is nothing to
// build or rebuild.
public sealed partial class MaximizeNumberOfNiceDivisorsBenchmarksTests
{
    private const int SmallestPrimeFactorCount = 40;

    [Fact]
    public void NaiveRecursion_AgreesWithMemoizedTopDown()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursion(), harness.MemoizedTopDown());
    }

    [Fact]
    public void MemoizedTopDown_AgreesWithNaiveRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedTopDown(), harness.NaiveRecursion());
    }

    private static MaximizeNumberOfNiceDivisorsBenchmarks BuildHarness() =>
        new() { PrimeFactors = SmallestPrimeFactorCount };
}
