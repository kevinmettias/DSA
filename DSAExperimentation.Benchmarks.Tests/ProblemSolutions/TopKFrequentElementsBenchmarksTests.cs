using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TopKFrequentElementsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - counting with a BCL Dictionary and sorting every
// distinct value by frequency against this repo's HashMap plus a size-k min-heap - so a harness
// whose arms disagree is timing two different problems. Setup draws the values from a fixed seed,
// so the same Length must rebuild the same workload; the bounded value range is what makes the
// frequencies repeat.
//
// The answers are compared as unordered sets because LeetCode 347 itself asks for the topCount
// values "in any order": the outer order the arms emit is genuinely unfixed, and only the set of
// selected values is part of the answer. That is the one thing the set comparison cannot check, so
// see the class comment on the benchmark for how the k-th frequency boundary is chosen.
public sealed partial class TopKFrequentElementsBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.OfUnorderedSet(BuildHarness().FullSort()),
            AnswerText.OfUnorderedSet(BuildHarness().FullSort()));

    [Fact]
    public void FullSort_SmallestLength_AgreesWithSizeKMinHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.SizeKMinHeap()),
            AnswerText.OfUnorderedSet(harness.FullSort()));
    }

    [Fact]
    public void SizeKMinHeap_SmallestLength_AgreesWithFullSort()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.FullSort()),
            AnswerText.OfUnorderedSet(harness.SizeKMinHeap()));
    }

    private static TopKFrequentElementsBenchmarks BuildHarness()
    {
        var harness = new TopKFrequentElementsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
