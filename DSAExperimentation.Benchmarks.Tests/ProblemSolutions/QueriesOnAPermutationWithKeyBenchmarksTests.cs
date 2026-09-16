using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for QueriesOnAPermutationWithKeyBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the move-to-front simulation replayed over a
// BCL List<int> and over this repo's own DynamicArray<int> - so a harness whose arms disagree is
// timing two different problems. The answers come back one per query in query order, which the
// problem pins, so AnswerText.Of is the right rendering; a set rendering would score an answer
// against the wrong query. Setup draws the query stream from one fixed seed, so the same
// PermutationSize must rebuild the same stream.
public sealed partial class QueriesOnAPermutationWithKeyBenchmarksTests
{
    private const int SmallestPermutationSize = 200;

    [Fact]
    public void Setup_SamePermutationSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ListMoveToFront()),
            AnswerText.Of(BuildHarness().ListMoveToFront()));

    [Fact]
    public void ListMoveToFront_SeededQueryStream_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.ListMoveToFront()),
            AnswerText.Of(harness.DynamicArrayMoveToFront()));
    }

    [Fact]
    public void DynamicArrayMoveToFront_SeededQueryStream_AgreesWithTheListArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DynamicArrayMoveToFront()),
            AnswerText.Of(harness.ListMoveToFront()));
    }

    private static QueriesOnAPermutationWithKeyBenchmarks BuildHarness()
    {
        var harness = new QueriesOnAPermutationWithKeyBenchmarks { PermutationSize = SmallestPermutationSize };
        harness.Setup();

        return harness;
    }
}
