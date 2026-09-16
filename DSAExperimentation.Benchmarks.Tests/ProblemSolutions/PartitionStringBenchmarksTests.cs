using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PartitionStringBenchmarks (ARCHITECTURE 17.9): its two arms are
// PartitionStringSolution's, competing membership checks for the same greedy partition - a HashSet
// against the repo's own Set - so a harness whose arms disagree is timing two different problems.
// Setup draws the text from one fixed seed, so the same Length must rebuild the same string and
// therefore the same segments, in the same order.
public sealed partial class PartitionStringBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(AnswerText.Of(BuildHarness().HashSetScan()), AnswerText.Of(BuildHarness().HashSetScan()));

    [Fact]
    public void HashSetScan_SmallAlphabetText_AgreesWithSetScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.SetScan()), AnswerText.Of(harness.HashSetScan()));
    }

    [Fact]
    public void SetScan_SmallAlphabetText_AgreesWithHashSetScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.HashSetScan()), AnswerText.Of(harness.SetScan()));
    }

    private static PartitionStringBenchmarks BuildHarness()
    {
        var harness = new PartitionStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
