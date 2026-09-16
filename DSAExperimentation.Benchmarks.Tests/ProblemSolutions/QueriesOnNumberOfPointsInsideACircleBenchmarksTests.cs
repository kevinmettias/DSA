using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for QueriesOnNumberOfPointsInsideACircleBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - how many points fall inside each query
// circle - so a harness whose arms disagree is timing two different problems. The counts come back
// one per query in query order, which the problem pins, so AnswerText.Of is the right rendering.
// Setup builds the points and the queries from one fixed seed, so the same PointCount must rebuild
// the same pair; otherwise two published numbers were never comparable in the first place.
public sealed partial class QueriesOnNumberOfPointsInsideACircleBenchmarksTests
{
    private const int SmallestPointCount = 1_000;

    [Fact]
    public void Setup_SamePointCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceScan()),
            AnswerText.Of(BuildHarness().BruteForceScan()));

    [Fact]
    public void BruteForceScan_SeededPointAndQueryBatch_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceScan()),
            AnswerText.Of(harness.SortedXWithBinarySearchPruning()));
    }

    [Fact]
    public void SortedXWithBinarySearchPruning_SeededPointAndQueryBatch_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SortedXWithBinarySearchPruning()),
            AnswerText.Of(harness.BruteForceScan()));
    }

    private static QueriesOnNumberOfPointsInsideACircleBenchmarks BuildHarness()
    {
        var harness = new QueriesOnNumberOfPointsInsideACircleBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
