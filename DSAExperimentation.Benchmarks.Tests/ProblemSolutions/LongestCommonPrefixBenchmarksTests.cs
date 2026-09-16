using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestCommonPrefixBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - shrinking a candidate prefix one character at a
// time against binary-searching the monotone "this length is shared by every string" predicate -
// so a harness whose arms disagree is timing two different problems. Both arms return the shared
// prefix itself, a string compared directly. Setup gives every string the same PrefixLength run
// of 'x' and then a distinct single-character suffix, so the answer is exactly that run, which is
// the decisive value both arms must reach and the same PrefixLength must rebuild.
public sealed partial class LongestCommonPrefixBenchmarksTests
{
    private const int SmallestPrefixLength = 64;

    private const char SharedPrefixCharacter = 'x';

    [Fact]
    public void Setup_SmallestPrefixLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedSharedPrefix(), BuildHarness().BinarySearchPredicate());
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());
    }

    [Fact]
    public void LinearScan_SmallestPrefixLength_AgreesWithBinarySearchPredicate()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSharedPrefix(), harness.LinearScan());
        Assert.Equal(harness.BinarySearchPredicate(), harness.LinearScan());
    }

    [Fact]
    public void BinarySearchPredicate_SmallestPrefixLength_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSharedPrefix(), harness.BinarySearchPredicate());
        Assert.Equal(harness.LinearScan(), harness.BinarySearchPredicate());
    }

    // Setup's strings share their leading 'x' run and diverge only at the suffix, so the longest
    // common prefix is that whole run.
    private static string ExpectedSharedPrefix() => new(SharedPrefixCharacter, SmallestPrefixLength);

    private static LongestCommonPrefixBenchmarks BuildHarness()
    {
        var harness = new LongestCommonPrefixBenchmarks { PrefixLength = SmallestPrefixLength };
        harness.Setup();

        return harness;
    }
}
