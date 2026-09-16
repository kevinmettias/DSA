using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveInvalidParenthesesBenchmarks (ARCHITECTURE 17.9): both arms are
// RemoveInvalidParenthesesSolution's, competing strategies for the same question - every one of the
// 2^n character subsets against a level-by-level BFS that stops at the first level holding a valid
// candidate - so a harness whose arms disagree keeps two different sets of results. Setup builds an
// input with exactly two unmatched leading openers from .HalvingFactor, so both strategies do real
// removal work.
public sealed partial class RemoveInvalidParenthesesBenchmarksTests
{
    private const int SmallestLength = 14;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceAllSubsets()),
            AnswerText.Of(BuildHarness().BruteForceAllSubsets()));

    [Fact]
    public void BruteForceAllSubsets_AgreesWithQueueBfsMinimalRemoval()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.QueueBfsMinimalRemoval()),
            AnswerText.Of(harness.BruteForceAllSubsets()));
    }

    [Fact]
    public void QueueBfsMinimalRemoval_AgreesWithBruteForceAllSubsets()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceAllSubsets()),
            AnswerText.Of(harness.QueueBfsMinimalRemoval()));
    }

    private static RemoveInvalidParenthesesBenchmarks BuildHarness()
    {
        var harness = new RemoveInvalidParenthesesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
