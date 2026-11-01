using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistributeElementsIntoTwoArraysIIBenchmarks (ARCHITECTURE 17.9): a property
// of the workload's shape. LC 3072's answer is the concatenation of arr1 and arr2, Setup draws
// Length values, and every element lands in exactly one of the two lists, so the concatenation is
// always Length long.
public sealed partial class DistributeElementsIntoTwoArraysIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void BruteForce_SeededValueArray_PlacesEveryElement() =>
        Assert.Equal(SmallestLength, BuildHarness().BruteForce().Length);

    private static DistributeElementsIntoTwoArraysIIBenchmarks BuildHarness()
    {
        var harness = new DistributeElementsIntoTwoArraysIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
