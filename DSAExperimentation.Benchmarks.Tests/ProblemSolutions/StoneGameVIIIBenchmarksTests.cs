using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StoneGameVIIIBenchmarks (ARCHITECTURE 17.9): both arms answer the same
// question - the score difference LC 1872's first player can force - one by plain minimax over
// the boundary chain, one by memoizing each boundary's result, so a harness whose arms disagree
// is timing two different problems. Setup builds the prefix table both arms are handed, so the
// same pile count must rebuild the same workload.
public sealed partial class StoneGameVIIIBenchmarksTests
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

    private static StoneGameVIIIBenchmarks BuildHarness()
    {
        var harness = new StoneGameVIIIBenchmarks { PileCount = SmallestPileCount };
        harness.Setup();

        return harness;
    }
}
