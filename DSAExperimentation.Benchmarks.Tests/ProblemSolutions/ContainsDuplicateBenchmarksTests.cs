using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ContainsDuplicateBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a pairwise brute force against a set probe - so a harness whose
// arms disagree is timing two different problems. Setup builds the workload from the fixture, which
// is a shuffled 0..Length-1 range, so the same Length must rebuild the same array; otherwise two
// published numbers were never comparable in the first place.
//
// The array is private, but the fixture makes its documented shape decisive through either arm: it
// holds one value per position and no repeat, so both strategies report false - the worst case the
// class comment says the workload exists to force, with no duplicate anywhere to stop the scan
// early.
public sealed partial class ContainsDuplicateBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameDuplicateFreeArray()
    {
        Assert.False(BuildHarness().HasDuplicateByBruteForce());
        Assert.Equal(BuildHarness().HasDuplicateBySetProbe(), BuildHarness().HasDuplicateBySetProbe());
    }

    [Fact]
    public void HasDuplicateByBruteForce_DuplicateFreeArray_AgreesWithHasDuplicateBySetProbe()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasDuplicateBySetProbe(), harness.HasDuplicateByBruteForce());
    }

    [Fact]
    public void HasDuplicateBySetProbe_DuplicateFreeArray_AgreesWithHasDuplicateByBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasDuplicateByBruteForce(), harness.HasDuplicateBySetProbe());
    }

    private static ContainsDuplicateBenchmarks BuildHarness()
    {
        var harness = new ContainsDuplicateBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
