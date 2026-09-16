using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountKReducibleNumbersLessThanNBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - enumerating [1, n) against grouping by popcount -
// so a harness whose arms disagree is timing two different problems. Setup seeds a fixed random
// binary string, so the same Length must rebuild the same n.
public sealed partial class CountKReducibleNumbersLessThanNBenchmarksTests
{
    private const int SmallestLength = 16;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // Setup always sets the leading bit, so n has exactly SmallestLength bits and the count
        // ranges over [1, n): the answer can never leave [1, 2^SmallestLength - 1], because x = 1
        // needs no popcount steps at all and so is always k-reducible.
        Assert.InRange(first.BruteForce(), 1, (1 << SmallestLength) - 1);
        Assert.Equal(first.BruteForce(), second.BruteForce());
    }

    [Fact]
    public void BruteForce_MixedBitPattern_AgreesWithPopcountCombinatorics()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PopcountCombinatorics(), harness.BruteForce());
    }

    [Fact]
    public void PopcountCombinatorics_MixedBitPattern_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.PopcountCombinatorics());
    }

    private static CountKReducibleNumbersLessThanNBenchmarks BuildHarness()
    {
        var harness = new CountKReducibleNumbersLessThanNBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
