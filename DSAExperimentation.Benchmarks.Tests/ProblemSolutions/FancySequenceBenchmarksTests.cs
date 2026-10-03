using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FancySequenceBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin: a bound
// on every value read back, which follows from Setup's construction rather than from either arm. Each arm builds its
// own sequence inside the call and returns every value GetIndex read back, in index order. Setup's appended values
// are all positive and its multipliers are all above one, so every live index holds at least one, and every
// reported value is a residue modulo the problem's modulus.
public sealed partial class FancySequenceBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every appended value is drawn from [1, 100) and the alternating operations only multiply by
    // 2..4 and add 2..4, so no live index can ever hold a value below one.
    private const int MinReplayedValue = 1;

    // LeetCode 1622 reports every value modulo 1e9+7.
    private const long Modulus = 1_000_000_007;

    [Fact]
    public void ArrayRescan_AlternatingAddAllAndMultAll_ReadsBackEveryIndexAsAPositiveResidue() =>
        AssertReadsBackEveryIndexAsAPositiveResidue(BuildHarness().ArrayRescan());

    [Fact]
    public void LazySegmentTreeAffine_AlternatingAddAllAndMultAll_ReadsBackEveryIndexAsAPositiveResidue() =>
        AssertReadsBackEveryIndexAsAPositiveResidue(BuildHarness().LazySegmentTreeAffine());

    private static void AssertReadsBackEveryIndexAsAPositiveResidue(int[] values) =>
        Assert.All(values, value => Assert.InRange(value, MinReplayedValue, Modulus - 1));

    private static FancySequenceBenchmarks BuildHarness()
    {
        var harness = new FancySequenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
