using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestValidParenthesesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the DP table recording the length ending at each
// position against the index-stack scan that matches each ')' to its '(' - so a harness whose arms
// disagree is timing two different problems. The workload is "(()())" repeated, a closed form over
// Length with no draw from any stream, so the same Length rebuilds the same string and the answer
// can be pinned rather than only compared.
public sealed partial class LongestValidParenthesesBenchmarksTests
{
    private const int SmallestLength = 200;

    // "(()())" is itself balanced, so the smallest [Params] length is 33 whole copies of it (198
    // characters) plus the two-character remainder "((". The 198-character whole-copy prefix is
    // valid on its own, and any longer substring has to end inside the trailing "((", where the
    // balance is non-zero - so the longest valid substring is exactly that prefix.
    private const int ExpectedLongestValidLength = 198;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().DynamicProgrammingArray(), BuildHarness().DynamicProgrammingArray());
        Assert.Equal(BuildHarness().StackScan(), BuildHarness().StackScan());
    }

    [Fact]
    public void DynamicProgrammingArray_RepeatedBalancedPattern_AgreesWithStackScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestValidLength, harness.DynamicProgrammingArray());
        Assert.Equal(harness.StackScan(), harness.DynamicProgrammingArray());
    }

    [Fact]
    public void StackScan_RepeatedBalancedPattern_AgreesWithDynamicProgrammingArray()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestValidLength, harness.StackScan());
        Assert.Equal(harness.DynamicProgrammingArray(), harness.StackScan());
    }

    private static LongestValidParenthesesBenchmarks BuildHarness()
    {
        var harness = new LongestValidParenthesesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
