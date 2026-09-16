using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StoneGameVBenchmarks (ARCHITECTURE 17.9): both arms answer the same
// question - the best score LC 1563's first player can force - one by plain interval recursion,
// one by memoizing each (left, right) range, so a harness whose arms disagree is timing two
// different problems. Setup's stone values are seeded, so the same pile count must rebuild the
// same workload.
public sealed partial class StoneGameVBenchmarksTests
{
    private const int SmallestPileCount = 120;

    [Fact]
    public void Setup_SamePileCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().MemoizedRecursion()),
            AnswerText.Of(BuildHarness().MemoizedRecursion()));

    [Fact]
    public void UnmemoizedRecursion_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    private static StoneGameVBenchmarks BuildHarness()
    {
        var harness = new StoneGameVBenchmarks { PileCount = SmallestPileCount };
        harness.Setup();

        return harness;
    }
}
