using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromePartitioningIIBenchmarks (ARCHITECTURE 17.9). Its two arms are
// competing strategies for the same question - the memoized suffix recurrence and the bottom-up cut
// table - so a harness whose arms disagree is cutting two different strings: both must report the
// same minimum. The class has neither [Params] nor a [GlobalSetup] - the workload is its private
// constant "aab", LeetCode 132's own example - so a harness is a bare initializer with nothing to
// set, and the answer is checked against that example's published minimum cut: one cut, "aa" | "b".
public sealed partial class PalindromePartitioningIIBenchmarksTests
{
    // LeetCode 132's answer for the benchmark's own "aab" workload.
    private const int ExpectedMinCut = 1;

    [Fact]
    public void MemoizedSuffixRecurrence_LeetCodeExample_ReturnsTheMinimumCut() =>
        Assert.Equal(ExpectedMinCut, new PalindromePartitioningIIBenchmarks().MemoizedSuffixRecurrence());

    [Fact]
    public void IterativeDynamicProgramming_LeetCodeExample_ReturnsTheMinimumCut() =>
        Assert.Equal(ExpectedMinCut, new PalindromePartitioningIIBenchmarks().IterativeDynamicProgramming());

    [Fact]
    public void IterativeDynamicProgramming_AgreesWithMemoizedSuffixRecurrence()
    {
        var harness = new PalindromePartitioningIIBenchmarks();

        Assert.Equal(harness.MemoizedSuffixRecurrence(), harness.IterativeDynamicProgramming());
    }
}
