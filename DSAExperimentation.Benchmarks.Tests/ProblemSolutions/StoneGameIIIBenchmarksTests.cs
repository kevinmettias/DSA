using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StoneGameIIIBenchmarks (ARCHITECTURE 17.9): both arms answer the same
// question - LC 1406's winner - one by plain minimax over the index, one by memoizing that
// index, so a harness whose arms disagree is timing two different problems. Setup's stone
// values are seeded, so the same pile count must rebuild the same workload.
public sealed partial class StoneGameIIIBenchmarksTests
{
    private const int SmallestPileCount = 20;

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

    private static StoneGameIIIBenchmarks BuildHarness()
    {
        var harness = new StoneGameIIIBenchmarks { PileCount = SmallestPileCount };
        harness.Setup();

        return harness;
    }
}
