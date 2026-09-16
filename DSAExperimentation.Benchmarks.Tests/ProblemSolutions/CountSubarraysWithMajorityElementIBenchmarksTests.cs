using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubarraysWithMajorityElementIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - enumerating every subarray against a Fenwick
// prefix-sum sweep - so a harness whose arms disagree is timing two different problems rather than
// two ways of answering one. Setup draws the array from one fixed seed, so the same Length must
// rebuild the same array; otherwise two published numbers were never comparable in the first place.
//
// The generated array is private and the subarray count is the only thing either arm reports, so the
// documented shape is asserted through that: the answer counts subarrays of a Length-long array, so
// it can never exceed Length * (Length + 1) / 2.
public sealed partial class CountSubarraysWithMajorityElementIBenchmarksTests
{
    private const int SmallestLength = 200;

    private const int SubarrayCount = SmallestLength * (SmallestLength + 1) / 2;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameArray()
    {
        Assert.InRange(BuildHarness().BruteForce(), 0, SubarrayCount);
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
    }

    [Fact]
    public void BruteForce_SeededFiveValueAlphabet_AgreesWithFenwickPrefixSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickPrefixSum(), harness.BruteForce());
    }

    [Fact]
    public void FenwickPrefixSum_SeededFiveValueAlphabet_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.FenwickPrefixSum());
    }

    private static CountSubarraysWithMajorityElementIBenchmarks BuildHarness()
    {
        var harness = new CountSubarraysWithMajorityElementIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
