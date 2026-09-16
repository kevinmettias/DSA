using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StockPriceFluctuationBenchmarks (ARCHITECTURE 17.9): both arms answer
// the same question - the replay of one seeded update/correction script through LC 2034's
// price tracker - one by rescanning every stored price per query, one by two heaps with lazy
// deletion, so a harness whose arms disagree is timing two different problems. Each arm
// constructs its own tracker inside the call, so one harness instance is safe to call twice
// in either order. Setup's script is seeded, so the same update count must rebuild it.
public sealed partial class StockPriceFluctuationBenchmarksTests
{
    private const int SmallestUpdateCount = 200;

    [Fact]
    public void Setup_SameUpdateCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().FullScanEveryQuery()),
            AnswerText.Of(BuildHarness().FullScanEveryQuery()));

    [Fact]
    public void FullScanEveryQuery_AgreesWithLazyDeletionTwoHeaps()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FullScanEveryQuery(), harness.LazyDeletionTwoHeaps());
    }

    [Fact]
    public void LazyDeletionTwoHeaps_AgreesWithFullScanEveryQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LazyDeletionTwoHeaps(), harness.FullScanEveryQuery());
    }

    private static StockPriceFluctuationBenchmarks BuildHarness()
    {
        var harness = new StockPriceFluctuationBenchmarks { UpdateCount = SmallestUpdateCount };
        harness.Setup();

        return harness;
    }
}
