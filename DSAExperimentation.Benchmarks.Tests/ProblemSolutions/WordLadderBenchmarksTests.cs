using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordLadderBenchmarks (ARCHITECTURE 17.9): both arms are competing BFS
// strategies for one question, so a harness whose arms disagree is timing two different problems.
// The workload is a connected mutation chain, so a ladder always exists and neither arm can agree
// by both failing fast - the baseline's ladder length is asserted to be a real one.
public sealed partial class WordLadderBenchmarksTests
{
    private const int SmallestWordCount = 200;
    private const int MinimumLadderLength = 1;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().MutationQueueBfs()),
            AnswerText.Of(BuildHarness().MutationQueueBfs()));

    [Fact]
    public void MutationQueueBfs_AgreesWithReduceGraphBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MutationQueueBfs(), harness.ReduceGraphBfs());
    }

    [Fact]
    public void ReduceGraphBfs_AgreesWithMutationQueueBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraphBfs(), harness.MutationQueueBfs());
    }

    [Fact]
    public void MutationQueueBfs_ConnectedChain_FindsAnActualLadder() =>
        Assert.True(BuildHarness().MutationQueueBfs() >= MinimumLadderLength);

    [Fact]
    public void ReduceGraphBfs_ConnectedChain_FindsAnActualLadder() =>
        Assert.True(BuildHarness().ReduceGraphBfs() >= MinimumLadderLength);

    private static WordLadderBenchmarks BuildHarness()
    {
        var harness = new WordLadderBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
