using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for QueryKthSmallestTrimmedNumberBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the k-th smallest trimmed number for each query
// - so a harness whose arms disagree is timing two different problems. The answers come back one
// per query in query order, which the problem pins, so AnswerText.Of is the right rendering. Setup
// draws both the numbers and the query batch from one fixed seed, so the same Length must rebuild
// the same pair.
public sealed partial class QueryKthSmallestTrimmedNumberBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().SelectionScanPerQuery()),
            AnswerText.Of(BuildHarness().SelectionScanPerQuery()));

    [Fact]
    public void SelectionScanPerQuery_SeededNumberAndQueryBatch_AgreesWithMergeSortPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SelectionScanPerQuery()),
            AnswerText.Of(harness.MergeSortPerQuery()));
    }

    [Fact]
    public void MergeSortPerQuery_SeededNumberAndQueryBatch_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MergeSortPerQuery()),
            AnswerText.Of(harness.SelectionScanPerQuery()));
    }

    private static QueryKthSmallestTrimmedNumberBenchmarks BuildHarness()
    {
        var harness = new QueryKthSmallestTrimmedNumberBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
