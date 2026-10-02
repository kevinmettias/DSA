using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MajorityElementBenchmarks (ARCHITECTURE 17.9). Its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems: the HashMap count's n/2 shortcut and the voting arm's full pass must still name the
// same element. Setup seeds just over half the array with one value and fills the rest with noise
// disjoint from it, so the majority value is the fixture's own - a decisive literal rather than a
// derived one. Because the workload is a seeded draw, the same Length must rebuild the same array
// and return the same value.
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

    [Fact]
    public void BoyerMooreVoting_SeededMajorityOverDisjointNoise_ReturnsTheSeededMajorityValue() =>
        Assert.Equal(ExpectedMajorityValue, BuildHarness().BoyerMooreVoting());

    [Fact]
    public void BoyerMooreVoting_AgreesWithHashMapCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapCount(), harness.BoyerMooreVoting());
    }

    private static MajorityElementBenchmarks BuildHarness()
    {
        var harness = new MajorityElementBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
