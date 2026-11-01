using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordLadderBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms
// to agreeing with each other, which two arms both failing fast would pass. The workload is a
// connected mutation chain, so a ladder always exists - the baseline's ladder length is asserted to
// be a real one.
public sealed partial class WordLadderBenchmarksTests
{
    private const int SmallestWordCount = 200;
    private const int MinimumLadderLength = 1;

    [Fact]
    public void MutationQueueBfs_ConnectedChain_FindsAnActualLadder() =>
        Assert.True(BuildHarness().MutationQueueBfs() >= MinimumLadderLength);

    private static WordLadderBenchmarks BuildHarness()
    {
        var harness = new WordLadderBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
