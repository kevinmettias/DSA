using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PrimeArrangementsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - primeCount! * (upperBound - primeCount)! modulo the problem's own
// modulus - so a harness whose arms disagree is timing two different problems. UpperBound is the
// whole input and the class carries no [GlobalSetup], so each axis value is its own harness.
public sealed partial class PrimeArrangementsBenchmarksTests
{
    private const int SmallestUpperBound = 2_000;

    [Fact]
    public void TrialDivision_AgreesWithSieveOfEratosthenes()
    {
        var harness = Harness(SmallestUpperBound);

        Assert.Equal(harness.SieveOfEratosthenes(), harness.TrialDivision());
    }

    [Fact]
    public void SieveOfEratosthenes_AgreesWithTrialDivision()
    {
        var harness = Harness(SmallestUpperBound);

        Assert.Equal(harness.TrialDivision(), harness.SieveOfEratosthenes());
    }

    private static PrimeArrangementsBenchmarks Harness(int upperBound) => new() { UpperBound = upperBound };
}
