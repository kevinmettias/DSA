using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PartitionLabelsBenchmarks (ARCHITECTURE 17.9): its two arms are
// PartitionLabelsSolution's, competing strategies for the same partition - rescanning the rest of
// the string for each block's end against one recorded last-occurrence table - so a harness whose
// arms disagree is timing two different problems. Setup draws the text from one fixed seed, so the
// same Length must rebuild the same string and therefore the same block sizes.
public sealed partial class PartitionLabelsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceRescan()),
            AnswerText.Of(BuildHarness().BruteForceRescan()));

    [Fact]
    public void BruteForceRescan_RandomAlphabetText_AgreesWithHashMapOnePass()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.HashMapOnePass()), AnswerText.Of(harness.BruteForceRescan()));
    }

    [Fact]
    public void HashMapOnePass_RandomAlphabetText_AgreesWithBruteForceRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForceRescan()), AnswerText.Of(harness.HashMapOnePass()));
    }

    private static PartitionLabelsBenchmarks BuildHarness()
    {
        var harness = new PartitionLabelsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
