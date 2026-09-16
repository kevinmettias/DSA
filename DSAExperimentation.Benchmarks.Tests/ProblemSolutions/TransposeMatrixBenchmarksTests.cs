using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TransposeMatrixBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the row-major double loop against the cache-blocked tile walk
// - so a harness whose arms disagree is timing two different problems. Both arms return a fresh
// n x m matrix built from the same source, so the transpose is rendered row by row in order.
// Neither arm mutates the source: Setup builds the square workload once from a fixed seed, so the
// same Size must rebuild the same matrix and both arms may be called on one harness.
public sealed partial class TransposeMatrixBenchmarksTests
{
    private const int SmallestSize = 100;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameMatrix() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().DirectIndexSwap()),
            AnswerText.Of(BuildHarness().DirectIndexSwap()));

    [Fact]
    public void DirectIndexSwap_SmallestSize_AgreesWithCacheBlockedTranspose()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.CacheBlockedTranspose()),
            AnswerText.Of(harness.DirectIndexSwap()));
    }

    [Fact]
    public void CacheBlockedTranspose_SmallestSize_AgreesWithDirectIndexSwap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DirectIndexSwap()),
            AnswerText.Of(harness.CacheBlockedTranspose()));
    }

    private static TransposeMatrixBenchmarks BuildHarness()
    {
        var harness = new TransposeMatrixBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
