using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FruitIntoBasketsBenchmarks (ARCHITECTURE 17.9): both arms are
// FruitIntoBasketsSolution's - the per-start rescan against the sliding window - so a harness whose
// arms disagree is timing two different problems. _fruits alternates between exactly two tree
// types, which is the harness's own point: a third type never appears, so the window never has to
// shrink and every start in the rescan runs to the end of the array. That also makes the answer
// decisive without reading it back out of either arm: with only two types in the whole array, the
// array itself is one legal basket pair, so the longest run is every fruit. Setup is a pure
// function of Length, so the same Length must rebuild the same array.
public sealed partial class FruitIntoBasketsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_TwoTreeTypesAcrossTheWholeArray_AgreesWithSlidingWindowHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(SmallestLength, harness.BruteForce());
        Assert.Equal(harness.SlidingWindowHashMap(), harness.BruteForce());
    }

    [Fact]
    public void SlidingWindowHashMap_TwoTreeTypesAcrossTheWholeArray_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SlidingWindowHashMap());
    }

    private static FruitIntoBasketsBenchmarks BuildHarness()
    {
        var harness = new FruitIntoBasketsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
