using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumScoreWordsFormedByLettersBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the hand-specialized include/skip recursion
// against Backtrack.Search closed over the identical choose/explore/unchoose steps - so a harness
// whose arms disagree is timing two different problems. Setup draws the words from one fixed seed and
// derives the letter pool from their combined usage, so the same WordCount must rebuild the same
// workload; otherwise two published numbers were never comparable in the first place.
public sealed partial class MaximumScoreWordsFormedByLettersBenchmarksTests
{
    private const int SmallestWordCount = 8;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NaiveRecursion(), BuildHarness().NaiveRecursion());

    [Fact]
    public void NaiveRecursion_HalfLetterBudget_AgreesWithBacktrackPrimitive()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BacktrackPrimitive(), harness.NaiveRecursion());
    }

    [Fact]
    public void BacktrackPrimitive_HalfLetterBudget_AgreesWithNaiveRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursion(), harness.BacktrackPrimitive());
    }

    private static MaximumScoreWordsFormedByLettersBenchmarks BuildHarness()
    {
        var harness = new MaximumScoreWordsFormedByLettersBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
