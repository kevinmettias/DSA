using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FrequenciesOfShortestSupersequencesBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. Setup builds the word set and its LetterGraph from a fixed seed, so the same
// WordCount must rebuild the same workload.
//
// Both arms share the solution's minimum-size subset search and differ only in how "is this subset a
// valid doubling?" is answered, so the sequence of frequency vectors is enumerated in the same mask
// order by both - the outer order is fixed by the shared driver, which makes AnswerText.Of's
// order-sensitive rendering the right comparison, not a set comparison that would hide a driver that
// stopped after the first valid subset.
public sealed partial class FrequenciesOfShortestSupersequencesBenchmarksTests
{
    private const int SmallestWordCount = 50;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().DfsSkipSet()),
            AnswerText.Of(BuildHarness().DfsSkipSet()));

    [Fact]
    public void DfsSkipSet_AgreesWithTopologicalSort()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.TopologicalSort()),
            AnswerText.Of(harness.DfsSkipSet()));
    }

    [Fact]
    public void TopologicalSort_AgreesWithDfsSkipSet()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DfsSkipSet()),
            AnswerText.Of(harness.TopologicalSort()));
    }

    private static FrequenciesOfShortestSupersequencesBenchmarks BuildHarness()
    {
        var harness = new FrequenciesOfShortestSupersequencesBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
