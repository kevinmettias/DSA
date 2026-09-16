using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximizeCountOfDistinctPrimesAfterSplitBenchmarks (ARCHITECTURE 17.9): both
// arms are competing strategies for one question - the best achievable prefix/suffix distinct-prime
// split after each point update - so a harness whose arms disagree is timing two different problems.
// Setup draws both the array and the query list from a fixed seed, so the same length must rebuild
// the same workload; otherwise two published numbers were never comparable.
//
// Both arms write their query updates straight into the harness's own _nums field, so the subject
// is hoisted rather than built inside the call and arm order matters: the second arm to run sees the
// array the first arm left behind, not the array Setup seeded, and the two then answer different
// questions. Each arm therefore gets its own freshly seeded harness, which is also what makes the
// agreement meaningful - it is asserted on the workload Setup actually documents.
public sealed partial class MaximizeCountOfDistinctPrimesAfterSplitBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithPrefixSuffixScan() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().PrefixSuffixScan()));

    [Fact]
    public void PrefixSuffixScan_AgreesWithBruteForce() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PrefixSuffixScan()),
            AnswerText.Of(BuildHarness().BruteForce()));

    private static MaximizeCountOfDistinctPrimesAfterSplitBenchmarks BuildHarness()
    {
        var harness = new MaximizeCountOfDistinctPrimesAfterSplitBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
