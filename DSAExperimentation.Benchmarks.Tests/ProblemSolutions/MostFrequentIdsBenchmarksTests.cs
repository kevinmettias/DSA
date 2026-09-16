using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MostFrequentIdsBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - the brute-force rescan after every update against the lazy-heap arm - so a harness
// whose arms disagree is timing two different problems. Both arms only read the seeded id/frequency arrays,
// so one harness instance is safe to call twice in either order. Setup draws that operation stream from one
// fixed seed, so the same Length must rebuild the same sequence; otherwise two published numbers were never
// comparable in the first place. Both arms return one answer per operation, and the problem pins that outer
// order, so the two renderings are compared element by element.
public sealed partial class MostFrequentIdsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameOperationStream() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_SeededUpdateStream_AgreesWithLazyHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LazyHeap()),
            AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void LazyHeap_SeededUpdateStream_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForce()),
            AnswerText.Of(harness.LazyHeap()));
    }

    private static MostFrequentIdsBenchmarks BuildHarness()
    {
        var harness = new MostFrequentIdsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
