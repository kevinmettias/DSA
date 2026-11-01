using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestCommonSubsequenceBenchmarks (ARCHITECTURE 17.9): Setup gives both
// operands Length copies of one character, so every table cell is reachable and the answer is
// exactly Length - an expected value derived from the workload's construction rather than from
// either arm, which the generic agreement check cannot supply. The baseline returns the subsequence
// length, a scalar compared directly.
public sealed partial class LongestCommonSubsequenceBenchmarksTests
{
    private const int SmallestLength = 60;

    // Setup's two operands are the same all-'a' string, so their longest common subsequence is
    // the whole string.
    private const int ExpectedLongestCommonSubsequenceLength = SmallestLength;

    [Fact]
    public void Tabulation_SmallestLength_SpansTheWholeString() =>
        Assert.Equal(ExpectedLongestCommonSubsequenceLength, BuildHarness().Tabulation());

    private static LongestCommonSubsequenceBenchmarks BuildHarness()
    {
        var harness = new LongestCommonSubsequenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
