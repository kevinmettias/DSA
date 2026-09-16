using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MajorityElementBenchmarks (ARCHITECTURE 17.9). There is one arm, so there is
// no second strategy to agree with: the expected answer comes from the fixture instead. Setup seeds
// just over half the array with one value and fills the rest with noise disjoint from it, so the
// majority value is the fixture's own - a decisive literal rather than a derived one. Because the
// workload is a seeded draw, the same Length must rebuild the same array and return the same value.
public sealed partial class MajorityElementBenchmarksTests
{
    private const int SmallestLength = 200;

    // The seeded value Setup repeats (Length / 2) + 1 times, which is a strict majority of the
    // smallest [Params] length; the noise fills [] 1, 1000000) and so can never coincide with it.
    private const int ExpectedMajorityValue = -1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().HashMapCount(), BuildHarness().HashMapCount());

    [Fact]
    public void HashMapCount_SeededMajorityOverDisjointNoise_ReturnsTheSeededMajorityValue() =>
        Assert.Equal(ExpectedMajorityValue, BuildHarness().HashMapCount());

    private static MajorityElementBenchmarks BuildHarness()
    {
        var harness = new MajorityElementBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
