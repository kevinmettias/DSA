using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CombinationSumIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the specialized recursion that sorts the candidates once
// against the general backtracking engine - so a harness whose arms disagree is timing two different
// problems. The class carries no [Params] and no [GlobalSetup]: LeetCode's own example candidates and
// target are the whole fixed workload, so every run must report the same combinations.
//
// AnswerText.OfUnorderedSet, not Of: LeetCode leaves the order of the returned combination set
// unspecified, while the order inside one combination is the ascending order the arms both build.
public sealed partial class CombinationSumIIBenchmarksTests
{
    [Fact]
    public void Backtracking_ExampleCandidatesWithARepeatedOne_AgreesWithSortAndBacktrackSpecialized()
    {
        var harness = new CombinationSumIIBenchmarks();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.SortAndBacktrackSpecialized()),
            AnswerText.OfUnorderedSet(harness.Backtracking()));
    }

    [Fact]
    public void SortAndBacktrackSpecialized_ExampleCandidatesWithARepeatedOne_AgreesWithBacktracking()
    {
        var harness = new CombinationSumIIBenchmarks();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.Backtracking()),
            AnswerText.OfUnorderedSet(harness.SortAndBacktrackSpecialized()));
    }
}
