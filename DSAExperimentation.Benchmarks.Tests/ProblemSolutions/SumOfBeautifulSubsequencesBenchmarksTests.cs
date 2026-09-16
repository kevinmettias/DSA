using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumOfBeautifulSubsequencesBenchmarks (ARCHITECTURE 17.9): both arms are
// SumOfBeautifulSubsequencesSolution's - the 2^Size bitmask enumeration against the divisor sieve -
// so a harness whose arms disagree is timing two different questions. Both answer with a bare int, and
// the workload is the seeded draw the shared fixture builds, so a rebuild at the same Size has to
// produce the same pair of totals.
public sealed partial class SumOfBeautifulSubsequencesBenchmarksTests
{
    private const int SmallestSize = 12;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceBitmask(), BuildHarness().BruteForceBitmask());

    [Fact]
    public void BruteForceBitmask_SeededValues_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DivisorSieve(), harness.BruteForceBitmask());
    }

    [Fact]
    public void DivisorSieve_SeededValues_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceBitmask(), harness.DivisorSieve());
    }

    private static SumOfBeautifulSubsequencesBenchmarks BuildHarness()
    {
        var harness = new SumOfBeautifulSubsequencesBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
