using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RegularExpressionMatchingBenchmarks (ARCHITECTURE 17.9): both arms are
// RegularExpressionMatchingSolution's, competing strategies for the same question - plain recursion
// against the memoized recurrence - so a harness whose arms disagree matches two different
// languages. Setup builds `(a*)^Repetitions` + a trailing literal against `a^Repetitions` + that same
// literal, so the pair genuinely matches: the trailing literal is what no `a*` can consume, and the
// final `a*` unit absorbs one `a` each. That verdict is fixed by the fixture, so it is asserted
// beside the agreement - two bools agreeing on `true` would otherwise witness only that both arms
// said something.
public sealed partial class RegularExpressionMatchingBenchmarksTests
{
    private const int SmallestRepetitions = 8;

    [Fact]
    public void Setup_SameRepetitions_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IsMatchByRecursion(),
            BuildHarness().IsMatchByRecursion());

    [Fact]
    public void IsMatchByRecursion_RepeatedStarUnitsAndTrailingLiteral_ReportsMatch()
    {
        var harness = BuildHarness();
        var recursionVerdict = harness.IsMatchByRecursion();

        Assert.True(recursionVerdict);
        Assert.Equal(harness.IsMatchByMemoization(), recursionVerdict);
    }

    [Fact]
    public void IsMatchByMemoization_AgreesWithRecursion()
    {
        var harness = BuildHarness();
        var memoizedVerdict = harness.IsMatchByMemoization();

        Assert.True(memoizedVerdict);
        Assert.Equal(harness.IsMatchByRecursion(), memoizedVerdict);
    }

    private static RegularExpressionMatchingBenchmarks BuildHarness()
    {
        var harness = new RegularExpressionMatchingBenchmarks { Repetitions = SmallestRepetitions };
        harness.Setup();

        return harness;
    }
}
