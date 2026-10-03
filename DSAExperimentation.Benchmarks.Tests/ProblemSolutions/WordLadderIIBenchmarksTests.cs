using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordLadderIIBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for the same question - the shortest transformation sequences - so a harness whose arms
// disagree is timing two different problems. Each arm returns the sequences it built, and the
// workload's chain shape keeps them real rather than none, which is what the count assertion below
// pins.
public sealed partial class WordLadderIIBenchmarksTests
{
    private const int SmallestWordCount = 50;
    private const int MinimumSequenceCount = 1;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().MutationLayeredBfsBacktrack()),
            AnswerGraphText.Of(BuildHarness().MutationLayeredBfsBacktrack()));

    [Fact]
    public void MutationLayeredBfsBacktrack_ConnectedChain_FindsAnActualSequence() =>
        Assert.True(BuildHarness().MutationLayeredBfsBacktrack().Count >= MinimumSequenceCount);

    [Fact]
    public void ReduceGraphBfsBacktrack_ConnectedChain_FindsAnActualSequence() =>
        Assert.True(BuildHarness().ReduceGraphBfsBacktrack().Count >= MinimumSequenceCount);

    private static WordLadderIIBenchmarks BuildHarness()
    {
        var harness = new WordLadderIIBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
