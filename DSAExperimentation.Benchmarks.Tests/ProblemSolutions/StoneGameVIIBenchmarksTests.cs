using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StoneGameVIIBenchmarks (ARCHITECTURE 17.9): both arms answer the same
// question - the score difference LC 1690's first player can force - one by plain minimax over
// (left, right) bounds, one by memoizing that pair, so a harness whose arms disagree is timing
// two different problems. Setup's stone values are seeded, so the same pile count must rebuild
// the same workload.
public sealed partial class StoneGameVIIBenchmarksTests
{
    private const int SmallestPileCount = 22;

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

    private static StoneGameVIIBenchmarks BuildHarness()
    {
        var harness = new StoneGameVIIBenchmarks { PileCount = SmallestPileCount };
        harness.Setup();

        return harness;
    }
}
