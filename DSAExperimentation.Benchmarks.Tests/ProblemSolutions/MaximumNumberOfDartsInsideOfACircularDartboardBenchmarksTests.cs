using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfDartsInsideOfACircularDartboardBenchmarks (ARCHITECTURE
// 17.9): both arms are MaximumNumberOfDartsInsideOfACircularDartboardSolution's competing
// strategies for one question. They run identical candidate-center geometry and differ only in the
// buffer the candidates are grown in - a BCL List against this repo's DynamicArray - so a harness
// whose arms disagree is timing two different problems, not two buffers. Both answer with a single
// dart count, compared directly.
public sealed partial class MaximumNumberOfDartsInsideOfACircularDartboardBenchmarksTests
{
    private const int SmallestDartCount = 20;

    [Fact]
    public void Setup_SameDartCount_RebuildsTheSameDartCloud()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The dart cloud is private, so the rebuild is pinned through the count it produces: the
        // same DartCount must draw the same seeded coordinates and cover them identically.
        Assert.Equal(first.PairwiseCandidateCentersWithList(), second.PairwiseCandidateCentersWithList());
        Assert.Equal(
            first.PairwiseCandidateCentersWithDynamicArray(),
            second.PairwiseCandidateCentersWithDynamicArray());
    }

    [Fact]
    public void PairwiseCandidateCentersWithList_SeededDartCloud_AgreesWithPairwiseCandidateCentersWithDynamicArray()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.PairwiseCandidateCentersWithDynamicArray(),
            harness.PairwiseCandidateCentersWithList());
    }

    [Fact]
    public void PairwiseCandidateCentersWithDynamicArray_SeededDartCloud_AgreesWithPairwiseCandidateCentersWithList()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.PairwiseCandidateCentersWithList(),
            harness.PairwiseCandidateCentersWithDynamicArray());
    }

    private static MaximumNumberOfDartsInsideOfACircularDartboardBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfDartsInsideOfACircularDartboardBenchmarks { DartCount = SmallestDartCount };
        harness.Setup();

        return harness;
    }
}
