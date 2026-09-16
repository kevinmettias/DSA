using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountArrayPairsDivisibleByKBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - testing every element pair against the divisor
// against pairing only the divisor-bounded gcd groups - so a harness whose arms disagree is timing
// two different problems. Both arms return a long, so they are compared directly. Setup draws from
// one fixed seed, so the same Length must rebuild the same array.
public sealed partial class CountArrayPairsDivisibleByKBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BruteForcePairwiseCheck(),
            BuildHarness().BruteForcePairwiseCheck());

    [Fact]
    public void BruteForcePairwiseCheck_TwoHundredValues_AgreesWithGcdGroupedHashMapCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GcdGroupedHashMapCount(), harness.BruteForcePairwiseCheck());
    }

    [Fact]
    public void GcdGroupedHashMapCount_TwoHundredValues_AgreesWithBruteForcePairwiseCheck()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForcePairwiseCheck(), harness.GcdGroupedHashMapCount());
    }

    private static CountArrayPairsDivisibleByKBenchmarks BuildHarness()
    {
        var harness = new CountArrayPairsDivisibleByKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
