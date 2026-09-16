using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestCommonSubsequenceBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - bottom-up tabulation against this repo's Memoizer
// running the same suffix-pair recurrence - so a harness whose arms disagree is timing two
// different problems. Both arms return the subsequence length, a scalar compared directly. Setup
// gives both operands Length copies of one character, so every table cell is reachable and the
// answer is exactly Length, which is the decisive value both arms must reach and the same Length
// must rebuild.
public sealed partial class LongestCommonSubsequenceBenchmarksTests
{
    private const int SmallestLength = 60;

    // Setup's two operands are the same all-'a' string, so their longest common subsequence is
    // the whole string.
    private const int ExpectedLongestCommonSubsequenceLength = SmallestLength;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedLongestCommonSubsequenceLength, BuildHarness().MemoizedRecurrence());
        Assert.Equal(BuildHarness().Tabulation(), BuildHarness().Tabulation());
    }

    [Fact]
    public void Tabulation_SmallestLength_AgreesWithMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestCommonSubsequenceLength, harness.Tabulation());
        Assert.Equal(harness.MemoizedRecurrence(), harness.Tabulation());
    }

    [Fact]
    public void MemoizedRecurrence_SmallestLength_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestCommonSubsequenceLength, harness.MemoizedRecurrence());
        Assert.Equal(harness.Tabulation(), harness.MemoizedRecurrence());
    }

    private static LongestCommonSubsequenceBenchmarks BuildHarness()
    {
        var harness = new LongestCommonSubsequenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
