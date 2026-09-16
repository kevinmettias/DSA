using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TheNumberOfGoodSubsetsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the unmemoized (candidateIndex, usedPrimeMask)
// recursion against the identical recurrence routed through this repo's Memoizer - so a harness
// whose arms disagree is timing two different problems. Both arms return the subset count as an
// int, so they are compared directly. Setup reduces nums to the squarefree GoodSubsetCandidates
// the arms take and does so from a fixed seed, so the same Length must rebuild the same workload.
public sealed partial class TheNumberOfGoodSubsetsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRecursion(), BuildHarness().BruteForceRecursion());

    [Fact]
    public void BruteForceRecursion_SmallestLength_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.BruteForceRecursion());
    }

    [Fact]
    public void MemoizedRecursion_SmallestLength_AgreesWithBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRecursion(), harness.MemoizedRecursion());
    }

    private static TheNumberOfGoodSubsetsBenchmarks BuildHarness()
    {
        var harness = new TheNumberOfGoodSubsetsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
