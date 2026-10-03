using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

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
            AnswerGraphText.Of(BuildHarness().PairwiseEmailOverlapScan()),
            AnswerGraphText.Of(BuildHarness().PairwiseEmailOverlapScan()));

    [Fact]
    public void PairwiseEmailOverlapScan_AgreesWithUnionFindByEmail()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.OfUnordered(harness.UnionFindByEmail()),
            AnswerGraphText.OfUnordered(harness.PairwiseEmailOverlapScan()));
    }

    private static AccountsMergeBenchmarks BuildHarness()
    {
        var harness = new AccountsMergeBenchmarks { AccountCount = SmallestAccountCount };
        harness.Setup();

        return harness;
    }
}
