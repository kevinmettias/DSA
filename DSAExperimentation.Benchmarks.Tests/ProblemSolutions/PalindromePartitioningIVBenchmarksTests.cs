using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromePartitioningIVBenchmarks (ARCHITECTURE 17.9): the baseline's verdict
// against the one the workload's shape fixes, which the generic check cannot supply - it holds the
// memoized recurrence to the baseline. Setup builds its documented
// "a"*BlockSize + "b" + "a"*BlockSize + "c" text, which the class comment already pins as having no
// valid three-way palindrome split, so the answer is a known false.
public sealed partial class PalindromePartitioningIVBenchmarksTests
{
    private const int SmallestBlockSize = 30;

    [Fact]
    public void CanPartitionIntoThreePalindromesByNaiveRecursion_CommentWorkload_ReportsNoThreeWaySplit() =>
        Assert.False(BuildHarness().CanPartitionIntoThreePalindromesByNaiveRecursion());

    private static PalindromePartitioningIVBenchmarks BuildHarness()
    {
        var harness = new PalindromePartitioningIVBenchmarks { BlockSize = SmallestBlockSize };
        harness.Setup();

        return harness;
    }
}
