using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordLadderIIBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for the same question - the shortest transformation sequences - so a harness whose arms
// disagree is timing two different problems. Note what the arms agree ON: each returns the COUNT of
// the sequences it built, so agreement witnesses that both found the same NUMBER of shortest
// sequences and cannot witness that they are the same sequences. The workload's chain shape keeps
// that count real rather than zero, which is what the count assertion below pins.
public sealed partial class WordLadderIIBenchmarksTests
{
    private const int SmallestWordCount = 50;
    private const int MinimumSequenceCount = 1;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().MutationLayeredBfsBacktrack()),
            AnswerText.Of(BuildHarness().MutationLayeredBfsBacktrack()));

    [Fact]
    public void MutationLayeredBfsBacktrack_AgreesWithReduceGraphBfsBacktrack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MutationLayeredBfsBacktrack(), harness.ReduceGraphBfsBacktrack());
    }

    [Fact]
    public void ReduceGraphBfsBacktrack_AgreesWithMutationLayeredBfsBacktrack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraphBfsBacktrack(), harness.MutationLayeredBfsBacktrack());
    }

    [Fact]
    public void MutationLayeredBfsBacktrack_ConnectedChain_FindsAnActualSequence() =>
        Assert.True(BuildHarness().MutationLayeredBfsBacktrack() >= MinimumSequenceCount);

    [Fact]
    public void ReduceGraphBfsBacktrack_ConnectedChain_FindsAnActualSequence() =>
        Assert.True(BuildHarness().ReduceGraphBfsBacktrack() >= MinimumSequenceCount);

    private static WordLadderIIBenchmarks BuildHarness()
    {
        var harness = new WordLadderIIBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
