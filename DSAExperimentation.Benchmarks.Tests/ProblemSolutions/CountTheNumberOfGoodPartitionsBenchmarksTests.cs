using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfGoodPartitionsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - enumerating every cut mask against merging each
// value's first and last occurrence - so a harness whose arms disagree is timing two different
// problems, not two ways of answering one. Setup draws the array from one fixed seed, so the same
// ArrayLength must rebuild the same array; otherwise two published numbers were never comparable in
// the first place.
//
// The generated array is private and the partition count is the only thing either arm reports, so the
// documented shape is asserted through that: an ArrayLength-long array admits only ArrayLength - 1
// cut positions, so no partition count can reach 2^(ArrayLength - 1).
public sealed partial class CountTheNumberOfGoodPartitionsBenchmarksTests
{
    private const int SmallestArrayLength = 16;

    private const long PartitionCount = 1L << (SmallestArrayLength - 1);

    // The whole array is always one valid partition - it needs no cut at all - so no run can
    // report zero.
    private const long FewestPartitions = 1;

    [Fact]
    public void Setup_SameArrayLength_RebuildsTheSameArray()
    {
        Assert.InRange(BuildHarness().BruteForce(), FewestPartitions, PartitionCount);
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
    }

    [Fact]
    public void BruteForce_FourValueAlphabet_AgreesWithLastOccurrenceMerge()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LastOccurrenceMerge(), harness.BruteForce());
    }

    [Fact]
    public void LastOccurrenceMerge_FourValueAlphabet_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.LastOccurrenceMerge());
    }

    private static CountTheNumberOfGoodPartitionsBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfGoodPartitionsBenchmarks { ArrayLength = SmallestArrayLength };
        harness.Setup();

        return harness;
    }
}
