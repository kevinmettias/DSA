using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HowManyNumbersAreSmallerThanTheCurrentNumberBenchmarks (ARCHITECTURE
// 17.9): both arms are competing strategies for the same question - the O(n^2) pairwise count
// against sorting once and binary-searching each element's insertion point - so a harness whose
// arms disagree is timing two different problems. Each arm returns one per-position count, and
// LeetCode pins the position of every element, so the outer order is fixed and AnswerText.Of
// is the right rendering. The workload is seeded, so the same Length must rebuild the same
// values and therefore the same counts.
public sealed partial class HowManyNumbersAreSmallerThanTheCurrentNumberBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PairwiseCount()),
            AnswerText.Of(BuildHarness().PairwiseCount()));

    [Fact]
    public void PairwiseCount_SmallerCountsPerPosition_AgreeWithSortAndLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SortAndLowerBound()),
            AnswerText.Of(harness.PairwiseCount()));
    }

    [Fact]
    public void SortAndLowerBound_SmallerCountsPerPosition_AgreeWithPairwiseCount()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PairwiseCount()),
            AnswerText.Of(harness.SortAndLowerBound()));
    }

    private static HowManyNumbersAreSmallerThanTheCurrentNumberBenchmarks BuildHarness()
    {
        var harness =
            new HowManyNumbersAreSmallerThanTheCurrentNumberBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
