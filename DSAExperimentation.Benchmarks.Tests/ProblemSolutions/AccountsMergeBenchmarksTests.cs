using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AccountsMergeBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems. Setup's workload is seeded, so the same parameters must rebuild the same workload -
// otherwise two published numbers were never comparable in the first place.
public sealed partial class AccountsMergeBenchmarksTests
{
    private const int SmallestAccountCount = 50;

    [Fact]
    public void Setup_SameAccountCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PairwiseEmailOverlapScan()),
            AnswerText.Of(BuildHarness().PairwiseEmailOverlapScan()));

    [Fact]
    public void PairwiseEmailOverlapScan_AgreesWithUnionFindByEmail()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.UnionFindByEmail()),
            AnswerText.OfUnorderedSet(harness.PairwiseEmailOverlapScan()));
    }

    [Fact]
    public void UnionFindByEmail_AgreesWithPairwiseEmailOverlapScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.PairwiseEmailOverlapScan()),
            AnswerText.OfUnorderedSet(harness.UnionFindByEmail()));
    }

    private static AccountsMergeBenchmarks BuildHarness()
    {
        var harness = new AccountsMergeBenchmarks { AccountCount = SmallestAccountCount };
        harness.Setup();

        return harness;
    }
}
