using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ThresholdMajorityQueriesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the per-query full range scan against the block
// index's O(sqrt(n)) candidate verification - so a harness whose arms disagree is timing two
// different problems. Both arms answer the queries in input order and return one int per query, so
// the answers are rendered order-sensitively. Setup builds the nums, the queries and the block
// index from one fixed seed, so the same ElementCount must rebuild the same workload.
public sealed partial class ThresholdMajorityQueriesBenchmarksTests
{
    private const int SmallestElementCount = 500;

    [Fact]
    public void Setup_SameElementCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_SmallestElementCount_AgreesWithBlockMode()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BlockMode()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void BlockMode_SmallestElementCount_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.BlockMode()));
    }

    private static ThresholdMajorityQueriesBenchmarks BuildHarness()
    {
        var harness = new ThresholdMajorityQueriesBenchmarks { ElementCount = SmallestElementCount };
        harness.Setup();

        return harness;
    }
}
