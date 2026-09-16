using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheCountOfMonotonicPairsIIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for one question - the same quadratic over-values DP Part I carries,
// now with a value ceiling twenty times higher, against its prefix-sum column - so a harness whose
// arms disagree is timing two different problems. Setup draws nums from one fixed seed, so the
// same Length must rebuild the same array.
public sealed partial class FindTheCountOfMonotonicPairsIIBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceDP(), BuildHarness().BruteForceDP());

    [Fact]
    public void BruteForceDP_SmallestLength_AgreesWithPrefixSumDP()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrefixSumDP(), harness.BruteForceDP());
    }

    [Fact]
    public void PrefixSumDP_SmallestLength_AgreesWithBruteForceDP()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceDP(), harness.PrefixSumDP());
    }

    private static FindTheCountOfMonotonicPairsIIBenchmarks BuildHarness()
    {
        var harness = new FindTheCountOfMonotonicPairsIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
