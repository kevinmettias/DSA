using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestValidParenthesesBenchmarks (ARCHITECTURE 17.9): the workload is
// "(()())" repeated, a closed form over Length with no draw from any stream, so the answer can be
// pinned independently of the arms - an expected value the generic agreement check, which only
// compares arms with each other, cannot supply.
public sealed partial class LongestValidParenthesesBenchmarksTests
{
    private const int SmallestLength = 200;

    // "(()())" is itself balanced, so the smallest [Params] length is 33 whole copies of it (198
    // characters) plus the two-character remainder "((". The 198-character whole-copy prefix is
    // valid on its own, and any longer substring has to end inside the trailing "((", where the
    // balance is non-zero - so the longest valid substring is exactly that prefix.
    private const int ExpectedLongestValidLength = 198;

    [Fact]
    public void DynamicProgrammingArray_RepeatedBalancedPattern_SpansTheWholeCopies() =>
        Assert.Equal(ExpectedLongestValidLength, BuildHarness().DynamicProgrammingArray());

    private static LongestValidParenthesesBenchmarks BuildHarness()
    {
        var harness = new LongestValidParenthesesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
