using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GreatestCommonDivisorOfStringsBenchmarks (ARCHITECTURE 17.9): both arms are
// GreatestCommonDivisorOfStringsSolution's - the str1+str2 == str2+str1 identity against this
// repo's PrefixFunctionSearch - so a harness whose arms disagree is timing two different problems.
// Both arms return the dividing string itself, so they are compared as strings. The workload makes
// that answer decisive: both inputs are the same unit repeated a different, coprime number of
// times, so the two lengths are coprime multiples of the unit length, and the longest string
// dividing both is the unit - whatever unit [GlobalSetup] happens to draw. The length of that
// answer is therefore fixed by the fixture's own arithmetic, independently of either arm. The unit
// is drawn off a fixed seed, so the same UnitLength must rebuild the same unit and the same pair.
public sealed partial class GreatestCommonDivisorOfStringsBenchmarksTests
{
    private const int SmallestUnitLength = 20;

    [Fact]
    public void Setup_SameUnitLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().ConcatenationEqualityCheck(),
            BuildHarness().ConcatenationEqualityCheck());

    [Fact]
    public void ConcatenationEqualityCheck_CoprimeRepeatCounts_AgreesWithPrefixFunctionPeriod()
    {
        var harness = BuildHarness();

        Assert.Equal(SmallestUnitLength, harness.ConcatenationEqualityCheck().Length);
        Assert.Equal(harness.PrefixFunctionPeriod(), harness.ConcatenationEqualityCheck());
    }

    [Fact]
    public void PrefixFunctionPeriod_CoprimeRepeatCounts_AgreesWithConcatenationEqualityCheck()
    {
        var harness = BuildHarness();

        Assert.Equal(SmallestUnitLength, harness.PrefixFunctionPeriod().Length);
        Assert.Equal(harness.ConcatenationEqualityCheck(), harness.PrefixFunctionPeriod());
    }

    private static GreatestCommonDivisorOfStringsBenchmarks BuildHarness()
    {
        var harness = new GreatestCommonDivisorOfStringsBenchmarks { UnitLength = SmallestUnitLength };
        harness.Setup();

        return harness;
    }
}
