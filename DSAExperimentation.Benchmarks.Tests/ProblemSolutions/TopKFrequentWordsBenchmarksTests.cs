using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TopKFrequentWordsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the full sort with a tie-break comparer against counting into
// this repo's own HashMap and keeping only the k best words in a size-k min-heap - so a harness
// whose arms disagree is timing two different problems. Both arms return the words in LeetCode
// 692's own required order (higher frequency first, lexicographically smaller word on a tie), so
// the two arrays are rendered order-sensitively. Setup draws the words from a small pool from a
// fixed seed, so the same Length must rebuild the same workload; the pool is what forces the
// frequency ties that make the tie-break the deciding part of the answer.
public sealed partial class TopKFrequentWordsBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ByFullSort()),
            AnswerText.Of(BuildHarness().ByFullSort()));

    [Fact]
    public void ByFullSort_SmallestLength_AgreesWithByMinHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.ByMinHeap()), AnswerText.Of(harness.ByFullSort()));
    }

    [Fact]
    public void ByMinHeap_SmallestLength_AgreesWithByFullSort()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.ByFullSort()), AnswerText.Of(harness.ByMinHeap()));
    }

    private static TopKFrequentWordsBenchmarks BuildHarness()
    {
        var harness = new TopKFrequentWordsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
