using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfWaysToReconstructATreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one count - a Dictionary plus LINQ sort against a HashMap plus this repo's
// MergeSort - so a harness whose arms disagree is timing two different problems. Setup rebuilds the
// fixture's star of chains from a fixed shape, so the same NodeCount must rebuild the same pairs;
// the smallest tuned NodeCount keeps the star's candidate-parent walk cheap to call twice.
public sealed partial class NumberOfWaysToReconstructATreeBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DictionaryAndLinqSort(), BuildHarness().DictionaryAndLinqSort());

    [Fact]
    public void DictionaryAndLinqSort_SmallestNodeCount_AgreesWithHashMapAndMergeSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapAndMergeSort(), harness.DictionaryAndLinqSort());
    }

    [Fact]
    public void HashMapAndMergeSort_SmallestNodeCount_AgreesWithDictionaryAndLinqSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryAndLinqSort(), harness.HashMapAndMergeSort());
    }

    private static NumberOfWaysToReconstructATreeBenchmarks BuildHarness()
    {
        var harness = new NumberOfWaysToReconstructATreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
