using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OrderlyQueueBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same smallest rotation - comparing every candidate rotation against reading the
// smallest suffix start off a SuffixArray - so a harness whose arms disagree is timing two different
// problems. Setup draws the text from one seeded Random, so the same Length must rebuild the same
// string. Both arms return the rotation itself, so the answer is compared whole rather than by
// length; the smallest tuned Length keeps the quadratic arm's rotation sweep affordable, and the
// setup comparison uses the SuffixArray arm because it is the cheap one to run twice.
public sealed partial class OrderlyQueueBenchmarksTests
{
    private const int SmallestLength = 50_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().SuffixArraySmallestRotation(), BuildHarness().SuffixArraySmallestRotation());

    [Fact]
    public void BruteForceAllRotations_SmallestLength_AgreesWithSuffixArraySmallestRotation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SuffixArraySmallestRotation(), harness.BruteForceAllRotations());
    }

    [Fact]
    public void SuffixArraySmallestRotation_SmallestLength_AgreesWithBruteForceAllRotations()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceAllRotations(), harness.SuffixArraySmallestRotation());
    }

    private static OrderlyQueueBenchmarks BuildHarness()
    {
        var harness = new OrderlyQueueBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
