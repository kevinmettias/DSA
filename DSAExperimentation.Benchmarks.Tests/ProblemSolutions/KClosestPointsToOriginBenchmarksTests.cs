using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KClosestPointsToOriginBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - which k points are nearest the origin - so a harness
// whose arms disagree is timing two different problems.
//
// The genuinely unfixed order here is the OUTER one: LC 973 says the answer may come back in any
// order, and the two arms really do use different ones - the full sort returns the k points in
// ascending distance, while the size-k max-heap pops its farthest root each time and returns
// them descending. The (x, y) pair inside each point keeps its own order and is compared
// order-sensitively, so this is not a way of hiding a disagreement about which points were
// selected - only about the sequence they are listed in.
public sealed partial class KClosestPointsToOriginBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePointCloud() =>
        Assert.Equal(
            AnswerText.OfUnorderedSet(BuildHarness().FullSort()),
            AnswerText.OfUnorderedSet(BuildHarness().FullSort()));

    [Fact]
    public void FullSort_SeededPointCloud_AgreesWithSizeKMaxHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.OfUnorderedSet(harness.SizeKMaxHeap()), AnswerText.OfUnorderedSet(harness.FullSort()));
    }

    [Fact]
    public void SizeKMaxHeap_SeededPointCloud_AgreesWithFullSort()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.OfUnorderedSet(harness.FullSort()), AnswerText.OfUnorderedSet(harness.SizeKMaxHeap()));
    }

    private static KClosestPointsToOriginBenchmarks BuildHarness()
    {
        var harness = new KClosestPointsToOriginBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
