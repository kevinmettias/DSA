using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SplitTheArrayToMakeCoprimeProductsBenchmarks (ARCHITECTURE 17.9): both arms
// are SplitTheArrayToMakeCoprimeProductsSolution's searches for the same split index over the same
// generated values - one taking the literal definition and forming BigInteger products, the other
// sweeping each prime's last occurrence - so a harness whose arms disagree is timing two different
// problems. Setup is a pure function of Length and its own fixed seed, so the same Length must
// rebuild the same values.
//
// Each arm reports the split index itself, or the sentinel the problem uses when no split exists.
// Values are drawn from a shared prime pool, so the same primes reappear on both sides of most
// candidates; agreement on that sentinel is weaker than agreement on an index - it says only that
// no arm found a split the other missed - and is reported as such.
public sealed partial class SplitTheArrayToMakeCoprimeProductsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValues() =>
        Assert.Equal(BuildHarness().BigIntegerProductScan(), BuildHarness().BigIntegerProductScan());

    [Fact]
    public void BigIntegerProductScan_TwoHundredSharedPrimeProducts_AgreesWithPrimeLastOccurrenceSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrimeLastOccurrenceSweep(), harness.BigIntegerProductScan());
    }

    [Fact]
    public void PrimeLastOccurrenceSweep_TwoHundredSharedPrimeProducts_AgreesWithBigIntegerProductScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BigIntegerProductScan(), harness.PrimeLastOccurrenceSweep());
    }

    private static SplitTheArrayToMakeCoprimeProductsBenchmarks BuildHarness()
    {
        var harness = new SplitTheArrayToMakeCoprimeProductsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
