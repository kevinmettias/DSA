using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromePartitioningBenchmarks (ARCHITECTURE 17.9). The class carries two
// arms - the backtracking walk and the precomputed-table walk - and has neither [Params] nor a
// [GlobalSetup]: the workload is its private constant "aab", LeetCode 131's own example, so a harness
// is a bare initializer with nothing to set. Each arm is checked against that example's published
// partitions - the two ways "aab" cuts into palindromes, in the order the walk emits them (shortest
// prefix first) - and the pair is checked against each other.
public sealed partial class PalindromePartitioningBenchmarksTests
{
    // LeetCode 131's answer for the benchmark's own "aab" workload, as AnswerText renders it.
    private const string ExpectedPartitions = "[[\"a\",\"a\",\"b\"],[\"aa\",\"b\"]]";

    [Fact]
    public void Backtracking_LeetCodeExample_ReturnsEveryPalindromePartition() =>
        Assert.Equal(ExpectedPartitions, AnswerText.Of(new PalindromePartitioningBenchmarks().Backtracking()));

    [Fact]
    public void PrecomputedPalindromeTable_LeetCodeExample_ReturnsEveryPalindromePartition() =>
        Assert.Equal(
            ExpectedPartitions,
            AnswerText.Of(new PalindromePartitioningBenchmarks().PrecomputedPalindromeTable()));

    [Fact]
    public void PrecomputedPalindromeTable_AgreesWithBacktracking()
    {
        var harness = new PalindromePartitioningBenchmarks();

        Assert.Equal(AnswerText.Of(harness.Backtracking()), AnswerText.Of(harness.PrecomputedPalindromeTable()));
    }
}
