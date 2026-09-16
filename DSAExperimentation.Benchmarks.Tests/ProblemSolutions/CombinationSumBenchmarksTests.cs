using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CombinationSumBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - specialized recursion over a fixed candidate set against the
// general backtracking engine - so a harness whose arms disagree is timing two different problems.
// Setup installs the candidate set the comment names, and Target is the only [Params] property, so
// the same Target must rebuild the same candidate workload; the combinations a candidate set can
// make are exactly the ones that sum to Target, which is the shape asserted below.
//
// AnswerText.OfUnorderedSet, not Of: LeetCode leaves the order of the returned combination set
// unspecified, while the order inside one combination is the ascending order the arms both build.
public sealed partial class CombinationSumBenchmarksTests
{
    private const int SmallestTarget = 30;

    [Fact]
    public void Setup_SameTarget_RebuildsTheSameCandidateWorkload()
    {
        Assert.All(BuildHarness().SpecializedRecursive(), combination => Assert.Equal(SmallestTarget, combination.Sum()));
        Assert.Equal(
            AnswerText.OfUnorderedSet(BuildHarness().Backtracking()),
            AnswerText.OfUnorderedSet(BuildHarness().Backtracking()));
    }

    [Fact]
    public void Backtracking_CandidatesTwoThreeFiveSeven_AgreesWithSpecializedRecursive()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.SpecializedRecursive()),
            AnswerText.OfUnorderedSet(harness.Backtracking()));
    }

    [Fact]
    public void SpecializedRecursive_CandidatesTwoThreeFiveSeven_AgreesWithBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.Backtracking()),
            AnswerText.OfUnorderedSet(harness.SpecializedRecursive()));
    }

    private static CombinationSumBenchmarks BuildHarness()
    {
        var harness = new CombinationSumBenchmarks { Target = SmallestTarget };
        harness.Setup();

        return harness;
    }
}
