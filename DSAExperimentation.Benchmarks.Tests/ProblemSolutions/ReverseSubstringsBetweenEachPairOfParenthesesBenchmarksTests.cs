using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseSubstringsBetweenEachPairOfParenthesesBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - re-splicing the whole string once
// per pair of matching parentheses against a single pass that keeps partial buffers on a stack - so
// a harness whose arms disagree is timing two different problems. Setup rebuilds the same run of
// sibling groups from GroupCount alone, so the same GroupCount must rebuild the same input;
// otherwise two published numbers were never comparable in the first place.
//
// Both arms read the hoisted input without writing to it, so one harness is safe to call twice in
// either order. The input's shape fixes the answer: every group is a sibling, not nested, and holds
// the same body, so each group reverses to the same body rendered backwards and the answer is that
// reversed body repeated once per group - stated here from the class's documented input rather than
// read back out of an arm.
public sealed partial class ReverseSubstringsBetweenEachPairOfParenthesesBenchmarksTests
{
    private const int SmallestGroupCount = 500;

    // The body [GlobalSetup] writes into every sibling group, and its reversal.
    private const string GroupBody = "abcdef";
    private const string ReversedGroupBody = "fedcba";

    [Fact]
    public void Setup_SameGroupCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().NaiveRepeatedSplice(),
            BuildHarness().NaiveRepeatedSplice());

    [Fact]
    public void NaiveRepeatedSplice_SiblingGroups_AgreesWithStackOfCharBuffers()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedReversedGroups(SmallestGroupCount), harness.NaiveRepeatedSplice());
        Assert.Equal(harness.StackOfCharBuffers(), harness.NaiveRepeatedSplice());
    }

    [Fact]
    public void StackOfCharBuffers_SiblingGroups_AgreesWithNaiveRepeatedSplice()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedReversedGroups(SmallestGroupCount), harness.StackOfCharBuffers());
        Assert.Equal(harness.NaiveRepeatedSplice(), harness.StackOfCharBuffers());
    }

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
