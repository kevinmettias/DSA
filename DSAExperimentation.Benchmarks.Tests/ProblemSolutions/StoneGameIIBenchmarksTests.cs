using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StoneGameIIBenchmarks (ARCHITECTURE 17.9): both arms answer the same
// question - the most stones Alice can take in LC 1140 - one by plain minimax over (index, M),
// one by memoizing that pair, so a harness whose arms disagree is timing two different
// problems. Setup's piles are seeded, so the same pile count must rebuild the same workload.
public sealed partial class StoneGameIIBenchmarksTests
{
    private const int SmallestPileCount = 10;

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

    private static StoneGameIIBenchmarks BuildHarness()
    {
        var harness = new StoneGameIIBenchmarks { PileCount = SmallestPileCount };
        harness.Setup();

        return harness;
    }
}
