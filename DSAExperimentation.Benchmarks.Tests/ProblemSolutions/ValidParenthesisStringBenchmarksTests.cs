using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidParenthesisStringBenchmarks (ARCHITECTURE 17.9): the generic check holds
// the arms to agreeing with each other, which a shared wrong verdict would pass, so this pins the
// baseline's verdict to the one the workload's construction fixes.
//
// Setup's _text is one '(' followed by stars and one ')' - the shape that maximizes the DP's
// reachable-set growth while the sweep never inspects its star stack's contents. LC 678 lets each
// star stand for an empty string, so the well-formed reading is always available and the answer is
// true. Both arms return bool, so this harness can only witness the verdict and not the reachable
// set behind it.
public sealed partial class ValidParenthesisStringBenchmarksTests
{
    // The smaller of Setup's [Params(10, 100)] lengths.
    private const int SmallestLength = 10;

    // Setup's documented outcome: the stars can all stand for the empty string.
    private const bool ExpectedIsValid = true;

    [Fact]
    public void IsValidStringByReachableOpenCountDp_StarsBetweenOnePair_AcceptsTheText() =>
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidStringByReachableOpenCountDp());

    private static ValidParenthesisStringBenchmarks BuildHarness()
    {
        var harness = new ValidParenthesisStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
