using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestHappyPrefixBenchmarks (ARCHITECTURE 17.9): Setup's text is
// Length - 1 copies of 'a' followed by one 'b': no proper prefix ends in 'b', so no non-empty
// candidate can be a suffix too and the answer is the empty prefix - an expected value derived from
// the workload's construction rather than from either arm, which the generic agreement check cannot
// supply. The baseline returns the prefix itself, compared directly.
public sealed partial class LongestHappyPrefixBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void ShrinkAndCompare_SmallestLength_ReturnsTheEmptyPrefix() =>
        Assert.Equal(string.Empty, BuildHarness().ShrinkAndCompare());

    private static LongestHappyPrefixBenchmarks BuildHarness()
    {
        var harness = new LongestHappyPrefixBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
