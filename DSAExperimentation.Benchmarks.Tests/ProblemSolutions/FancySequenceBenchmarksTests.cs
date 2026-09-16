using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FancySequenceBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - re-scanning and rewriting every live element on each
// addAll/multAll against this repo's LazySegmentTree applying the same affine transform as one
// lazy range update - so a harness whose arms disagree is replaying two different operation
// scripts. Each arm builds its own sequence inside the call, so one harness can be replayed twice
// in either order. Both arms return the same proxy for the whole replay: the sum of every index
// read back. Setup's appended values are all positive and its multipliers are all above one, so
// every live index holds at least one, and every reported value is a residue modulo the problem's
// modulus - the range is what the replay's sum has to respect.
public sealed partial class FancySequenceBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every appended value is drawn from [1, 100) and the alternating operations only multiply by
    // 2..4 and add 2..4, so no live index can ever hold a value below one.
    private const long MinReplayedSum = SmallestLength;

    // LeetCode 1622 reports every value modulo 1e9+7.
    private const long Modulus = 1_000_000_007;
    private const long MaxReplayedSum = SmallestLength * (Modulus - 1);

    [Fact]
    public void Setup_SameLength_RebuildsTheSameOperationScript()
    {
        Assert.InRange(BuildHarness().ArrayRescan(), MinReplayedSum, MaxReplayedSum);

        Assert.Equal(BuildHarness().ArrayRescan(), BuildHarness().ArrayRescan());
    }

    [Fact]
    public void ArrayRescan_AlternatingAddAllAndMultAll_AgreesWithLazySegmentTreeAffine()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LazySegmentTreeAffine(), harness.ArrayRescan());
    }

    [Fact]
    public void LazySegmentTreeAffine_AlternatingAddAllAndMultAll_AgreesWithArrayRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayRescan(), harness.LazySegmentTreeAffine());
    }

    private static FancySequenceBenchmarks BuildHarness()
    {
        var harness = new FancySequenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
