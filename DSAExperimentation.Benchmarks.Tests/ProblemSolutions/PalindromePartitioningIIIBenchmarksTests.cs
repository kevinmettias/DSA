using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromePartitioningIIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one minimum change count - the naive recursion against the memoized top
// down one - so a harness whose arms disagree is timing two different problems. Setup draws the text
// from one seeded Random and derives the partition count from the same Length, so the same Length
// must rebuild the same text and the same K; the smallest tuned Length keeps the naive arm's shared
// decision tree affordable.
public sealed partial class PalindromePartitioningIIIBenchmarksTests
{
    private const int SmallestLength = 12;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NaiveRecursion(), BuildHarness().NaiveRecursion());

    [Fact]
    public void NaiveRecursion_SmallestLength_AgreesWithMemoizedTopDown()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedTopDown(), harness.NaiveRecursion());
    }

    [Fact]
    public void MemoizedTopDown_SmallestLength_AgreesWithNaiveRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursion(), harness.MemoizedTopDown());
    }

    private static PalindromePartitioningIIIBenchmarks BuildHarness()
    {
        var harness = new PalindromePartitioningIIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
