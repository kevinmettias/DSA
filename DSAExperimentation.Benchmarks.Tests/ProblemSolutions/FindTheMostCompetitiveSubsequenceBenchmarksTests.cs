using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheMostCompetitiveSubsequenceBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - removing one first-descent element at a time
// against a single monotonic-stack sweep - so a harness whose arms disagree is timing two
// different problems. Both answers are a subsequence read left to right, so AnswerText.Of is the
// right rendering. Setup draws the array from one fixed seed and derives the selection length from
// Length, so the same Length must rebuild the same pair.
public sealed partial class FindTheMostCompetitiveSubsequenceBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RepeatedFirstDescentRemoval()),
            AnswerText.Of(BuildHarness().RepeatedFirstDescentRemoval()));

    [Fact]
    public void RepeatedFirstDescentRemoval_SmallestLength_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MonotonicStackSweep()),
            AnswerText.Of(harness.RepeatedFirstDescentRemoval()));
    }

    [Fact]
    public void MonotonicStackSweep_SmallestLength_AgreesWithRepeatedFirstDescentRemoval()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RepeatedFirstDescentRemoval()),
            AnswerText.Of(harness.MonotonicStackSweep()));
    }

    private static FindTheMostCompetitiveSubsequenceBenchmarks BuildHarness()
    {
        var harness = new FindTheMostCompetitiveSubsequenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
