using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidParenthesisStringBenchmarks (ARCHITECTURE 17.9): its two arms are
// ValidParenthesisStringSolution's competing strategies for the same question - the reachable-open-
// count DP against the two-index stack sweep - so a harness whose arms disagree is answering two
// different questions about LC 678.
//
// Setup's _text is one '(' followed by stars and one ')' - the shape that maximizes the DP's
// reachable-set growth while the sweep never inspects its star stack's contents. LC 678 lets each
// star stand for an empty string, so the well-formed reading is always available and the answer is
// true, which is the decisive literal asserted alongside the agreement. Both arms return bool, so
// this harness can only witness the verdict and not the reachable set behind it.
public sealed partial class ValidParenthesisStringBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // Setup's documented outcome: the stars can all stand for the empty string.
    private const bool ExpectedIsValid = true;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(
            BuildHarness().IsValidStringByTwoIndexStackSweep(),
            BuildHarness().IsValidStringByTwoIndexStackSweep());
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidStringByTwoIndexStackSweep());
    }

    [Fact]
    public void IsValidStringByReachableOpenCountDp_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsValidStringByTwoIndexStackSweep(), harness.IsValidStringByReachableOpenCountDp());
        Assert.Equal(ExpectedIsValid, harness.IsValidStringByReachableOpenCountDp());
    }

    [Fact]
    public void IsValidStringByTwoIndexStackSweep_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsValidStringByReachableOpenCountDp(), harness.IsValidStringByTwoIndexStackSweep());
        Assert.Equal(ExpectedIsValid, harness.IsValidStringByTwoIndexStackSweep());
    }

    private static ValidParenthesisStringBenchmarks BuildHarness()
    {
        var harness = new ValidParenthesisStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
