using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StickersToSpellWordBenchmarks (ARCHITECTURE 17.9): the fewest stickers
// that spell the target is derivable from the pair count alone, so the baseline is asserted
// against it - the generic arm check only holds the memoized arm to the baseline's answer, which
// two arms sharing one wrong count would pass.
public sealed partial class StickersToSpellWordBenchmarksTests
{
    private const int SmallestPairCount = 3;

    // The target is "ab" repeated SmallestPairCount times and every sticker covers exactly one
    // 'a' and one 'b', so SmallestPairCount stickers are necessary and sufficient - the
    // maximal-overlap shape the benchmark's comment describes.
    private const int ExpectedMinStickers = SmallestPairCount;

    [Fact]
    public void Naive_ThreePairs_NeedsOneStickerPerPair() =>
        Assert.Equal(ExpectedMinStickers, BuildHarness().Naive());

    private static StickersToSpellWordBenchmarks BuildHarness()
    {
        var harness = new StickersToSpellWordBenchmarks { PairCount = SmallestPairCount };
        harness.Setup();

        return harness;
    }
}
