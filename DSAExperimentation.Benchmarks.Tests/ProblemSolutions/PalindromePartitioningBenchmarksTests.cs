using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromePartitioningBenchmarks (ARCHITECTURE 17.9). The class carries a
// single arm and has neither [Params] nor a [GlobalSetup] - the workload is its private constant
// "aab", LeetCode 131's own example - so a harness is a bare initializer with nothing to set. With no
// second strategy to reconcile the arm against, the answer is checked against that example's
// published partitions instead: the two ways "aab" cuts into palindromes, in the order the
// backtracking walk emits them (shortest prefix first).
public sealed partial class PalindromePartitioningBenchmarksTests
{
    // LeetCode 131's answer for the benchmark's own "aab" workload, as AnswerText renders it.
    private const string ExpectedPartitions = "[[\"a\",\"a\",\"b\"],[\"aa\",\"b\"]]";

    [Fact]
    public void Backtracking_LeetCodeExample_ReturnsEveryPalindromePartition() =>
        Assert.Equal(ExpectedPartitions, AnswerText.Of(new PalindromePartitioningBenchmarks().Backtracking()));
}
