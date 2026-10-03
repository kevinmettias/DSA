using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CombinationSumBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - specialized recursion over a fixed candidate set against the
// general backtracking engine - so a harness whose arms disagree is timing two different problems.
// Setup installs the candidate set the comment names, and Target is the only [Params] property, so
// the same Target must rebuild the same candidate workload; the combinations a candidate set can
// make are exactly the ones that sum to Target, which is the shape asserted below.
//
// AnswerGraphText.OfUnordered, not Of: LeetCode leaves the order of the returned combination set
// unspecified, while the order inside one combination is the ascending order the arms both build.
public sealed partial class CombinationSumBenchmarksTests
{
    private const int SmallestTarget = 30;

    [Fact]
    public void Setup_SameTarget_RebuildsTheSameCandidateWorkload()
    {
        Assert.All(BuildHarness().SpecializedRecursive(), combination => Assert.Equal(SmallestTarget, combination.Sum()));
        Assert.Equal(
            AnswerGraphText.OfUnordered(BuildHarness().Backtracking()),
            AnswerGraphText.OfUnordered(BuildHarness().Backtracking()));
    }

    [Fact]
    public void Backtracking_CandidatesTwoThreeFiveSeven_AgreesWithSpecializedRecursive()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.OfUnordered(harness.SpecializedRecursive()),
            AnswerGraphText.OfUnordered(harness.Backtracking()));
    }

    [Fact]
    public void SpecializedRecursive_CandidatesTwoThreeFiveSeven_AgreesWithBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.OfUnordered(harness.Backtracking()),
            AnswerGraphText.OfUnordered(harness.SpecializedRecursive()));
    }

    private static CombinationSumBenchmarks BuildHarness()
    {
        var harness = new CombinationSumBenchmarks { Target = SmallestTarget };
        harness.Setup();

        return harness;
    }
}
