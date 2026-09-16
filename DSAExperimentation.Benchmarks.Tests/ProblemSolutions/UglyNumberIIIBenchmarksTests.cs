using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for UglyNumberIIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// UglyNumberIIISolution's competing strategies for the same question - counting candidates one at a
// time against LowerBound over the monotone "count(x) >= rank" virtual sequence - so a harness whose
// arms disagree is handing back two different numbers.
//
// Both arms answer with the rank-th number divisible by 2, 3 or 5, which is the quantity LC 1201
// asks for rather than a proxy for it, so agreeing on it is agreeing on the whole answer. The class
// carries no [GlobalSetup]: the two factors and the rank are the entire workload.
public sealed partial class UglyNumberIIIBenchmarksTests
{
    // The smaller of the class's [Params(2_000, 50_000)] ranks.
    private const int SmallestRank = 2_000;

    [Fact]
    public void BruteForceCount_SmallestRank_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchOnCount(), harness.BruteForceCount());
    }

    [Fact]
    public void BinarySearchOnCount_SmallestRank_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceCount(), harness.BinarySearchOnCount());
    }

    private static UglyNumberIIIBenchmarks BuildHarness() => new() { Rank = SmallestRank };
}
