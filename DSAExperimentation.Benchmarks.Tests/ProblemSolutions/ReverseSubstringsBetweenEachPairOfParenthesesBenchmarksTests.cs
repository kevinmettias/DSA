using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseSubstringsBetweenEachPairOfParenthesesBenchmarks (ARCHITECTURE 17.9).
// Arm agreement is BenchmarkArmsTests' job; this pins the baseline's answer to one derived from the
// workload's shape. The input's shape fixes the answer: every group is a sibling, not nested, and holds
// the same body, so each group reverses to the same body rendered backwards and the answer is that
// reversed body repeated once per group - stated here from the class's documented input rather than
// read back out of an arm.
public sealed partial class ReverseSubstringsBetweenEachPairOfParenthesesBenchmarksTests
{
    private const int SmallestGroupCount = 25;

    // The body [GlobalSetup] writes into every sibling group, and its reversal.
    private const string GroupBody = "abcdef";
    private const string ReversedGroupBody = "fedcba";

    [Fact]
    public void NaiveRepeatedSplice_SiblingGroups_ReversesEveryGroupBody() =>
        Assert.Equal(ExpectedReversedGroups(SmallestGroupCount), BuildHarness().NaiveRepeatedSplice());

    // Every group is a sibling holding the same body, and the parentheses themselves are dropped, so
    // group i contributes the reversed body and nothing else.
    private static string ExpectedReversedGroups(int groupCount) =>
        string.Concat(Enumerable.Repeat(ReversedGroupBody, groupCount));

    private static ReverseSubstringsBetweenEachPairOfParenthesesBenchmarks BuildHarness()
    {
        var harness = new ReverseSubstringsBetweenEachPairOfParenthesesBenchmarks { GroupCount = SmallestGroupCount };
        harness.Setup();

        return harness;
    }
}
