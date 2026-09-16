using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheSequenceOfStringsAppearedOnTheScreenBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question, so a harness whose arms disagree is
// timing two different problems. Setup builds the target from Length alone, so the same Length must
// rebuild the same workload - otherwise two published numbers were never comparable.
//
// Both arms walk the screen buffers left to right and emit one string per roll, so the outer order
// of the returned sequence is fixed by the problem itself and AnswerText.Of's order-sensitive
// rendering is the right comparison.
public sealed partial class FindTheSequenceOfStringsAppearedOnTheScreenBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().StringBuilderWalk()),
            AnswerText.Of(BuildHarness().StringBuilderWalk()));

    [Fact]
    public void StringBuilderWalk_AgreesWithGrowableBufferWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.GrowableBufferWalk()),
            AnswerText.Of(harness.StringBuilderWalk()));
    }

    [Fact]
    public void GrowableBufferWalk_AgreesWithStringBuilderWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.StringBuilderWalk()),
            AnswerText.Of(harness.GrowableBufferWalk()));
    }

    private static FindTheSequenceOfStringsAppearedOnTheScreenBenchmarks BuildHarness()
    {
        var harness = new FindTheSequenceOfStringsAppearedOnTheScreenBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
