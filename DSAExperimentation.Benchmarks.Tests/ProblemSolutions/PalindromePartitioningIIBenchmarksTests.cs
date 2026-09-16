using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromePartitioningIIBenchmarks (ARCHITECTURE 17.9). The class carries a
// single arm and has neither [Params] nor a [GlobalSetup] - the workload is its private constant
// "aab", LeetCode 132's own example - so a harness is a bare initializer with nothing to set. With no
// second strategy to reconcile the arm against, the answer is checked against that example's
// published minimum cut instead: one cut, "aa" | "b".
public sealed partial class PalindromePartitioningIIBenchmarksTests
{
    // LeetCode 132's answer for the benchmark's own "aab" workload.
    private const int ExpectedMinCut = 1;

    [Fact]
    public void MemoizedSuffixRecurrence_LeetCodeExample_ReturnsTheMinimumCut() =>
        Assert.Equal(ExpectedMinCut, new PalindromePartitioningIIBenchmarks().MemoizedSuffixRecurrence());
}
