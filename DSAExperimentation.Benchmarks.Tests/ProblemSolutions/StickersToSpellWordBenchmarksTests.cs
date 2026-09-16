using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StickersToSpellWordBenchmarks (ARCHITECTURE 17.9): both arms answer the
// same question - the fewest stickers that spell the target - one by re-exploring every
// equivalent choice path, one by memoizing the state those choices lead to, so a harness whose
// arms disagree is timing two different problems. Setup builds the target from a fixed pair
// count, so the same count must rebuild the same workload.
public sealed partial class StickersToSpellWordBenchmarksTests
{
    private const int SmallestPairCount = 10;

    // The target is "ab" repeated SmallestPairCount times and every sticker covers exactly one
    // 'a' and one 'b', so SmallestPairCount stickers are necessary and sufficient - the
    // maximal-overlap shape the benchmark's comment describes.
    private const int ExpectedMinStickers = SmallestPairCount;

    [Fact]
    public void Setup_SamePairCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().Naive()),
            AnswerText.Of(BuildHarness().Naive()));

    [Fact]
    public void Naive_AgreesWithMemoized()
    {
        var harness = BuildHarness();
        var naive = harness.Naive();

        Assert.Equal(naive, harness.Memoized());
        Assert.Equal(ExpectedMinStickers, naive);
    }

    [Fact]
    public void Memoized_AgreesWithNaive()
    {
        var harness = BuildHarness();
        var memoized = harness.Memoized();

        Assert.Equal(memoized, harness.Naive());
        Assert.Equal(ExpectedMinStickers, memoized);
    }

    private static StickersToSpellWordBenchmarks BuildHarness()
    {
        var harness = new StickersToSpellWordBenchmarks { PairCount = SmallestPairCount };
        harness.Setup();

        return harness;
    }
}
