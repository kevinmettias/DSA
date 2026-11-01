using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FruitIntoBasketsBenchmarks (ARCHITECTURE 17.9): the generic arms check only
// compares the arms with each other, so this pins the baseline's answer without reading it back out
// of either arm. _fruits alternates between exactly two tree types, which is the harness's own
// point: a third type never appears, so the window never has to shrink and every start in the
// rescan runs to the end of the array. With only two types in the whole array, the array itself is
// one legal basket pair, so the longest run is every fruit.
public sealed partial class FruitIntoBasketsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void BruteForce_TwoTreeTypesAcrossTheWholeArray_PicksEveryFruit() =>
        Assert.Equal(SmallestLength, BuildHarness().BruteForce());

    private static FruitIntoBasketsBenchmarks BuildHarness()
    {
        var harness = new FruitIntoBasketsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
