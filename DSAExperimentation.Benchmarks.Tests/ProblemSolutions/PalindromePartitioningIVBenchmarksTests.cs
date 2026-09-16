using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromePartitioningIVBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one verdict - the naive recursion against the memoized recurrence - so a
// harness whose arms disagree is timing two different problems. Setup builds its documented
// "a"*BlockSize + "b" + "a"*BlockSize + "c" text, which the class comment already pins as having no
// valid three-way palindrome split, so the answer every arm owes is a known false; each arm is held
// both to that fixed verdict and to the other arm. The smallest tuned BlockSize is used, where the
// unmemoized arm re-explores the fewest repeated states.
public sealed partial class PalindromePartitioningIVBenchmarksTests
{
    private const int SmallestBlockSize = 30;

    [Fact]
    public void Setup_SameBlockSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().CanPartitionIntoThreePalindromesByNaiveRecursion(), BuildHarness().CanPartitionIntoThreePalindromesByNaiveRecursion());

    [Fact]
    public void CanPartitionIntoThreePalindromesByNaiveRecursion_CommentWorkload_ReportsNoThreeWaySplit()
    {
        var harness = BuildHarness();

        Assert.False(harness.CanPartitionIntoThreePalindromesByNaiveRecursion());
        Assert.Equal(
            harness.CanPartitionIntoThreePalindromesByMemoizedRecurrence(),
            harness.CanPartitionIntoThreePalindromesByNaiveRecursion());
    }

    [Fact]
    public void CanPartitionIntoThreePalindromesByMemoizedRecurrence_CommentWorkload_ReportsNoThreeWaySplit()
    {
        var harness = BuildHarness();

        Assert.False(harness.CanPartitionIntoThreePalindromesByMemoizedRecurrence());
        Assert.Equal(
            harness.CanPartitionIntoThreePalindromesByNaiveRecursion(),
            harness.CanPartitionIntoThreePalindromesByMemoizedRecurrence());
    }

    private static PalindromePartitioningIVBenchmarks BuildHarness()
    {
        var harness = new PalindromePartitioningIVBenchmarks { BlockSize = SmallestBlockSize };
        harness.Setup();

        return harness;
    }
}
