using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ExpressionAddOperatorsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a hand-rolled backtrack against this repo's
// DepthFirstSearch.Traverse over the identical implicit graph - so a harness whose arms disagree is
// exploring two different state spaces. Both arms return the number of expressions they built, and
// the harness's target is deliberately unreachable, so both must walk the full O(4^n) space and
// still report nothing. The class has no [GlobalSetup]; the [Params] number is the whole workload,
// and the smallest one (seven digits, 4^7 partial expressions) is already real work.
public sealed partial class ExpressionAddOperatorsBenchmarksTests
{
    private const string SmallestNumber = "1234567";

    // The harness's documented unreachable target: every expression built from the digits is orders
    // of magnitude away from int.MinValue, so neither strategy ever short-circuits and neither finds
    // a match.
    private const int ExpectedMatchCount = 0;

    [Fact]
    public void RecursiveBacktrack_UnreachableTarget_AgreesWithTraverseComposed()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatchCount, harness.RecursiveBacktrack());

        Assert.Equal(harness.TraverseComposed(), harness.RecursiveBacktrack());
    }

    [Fact]
    public void TraverseComposed_UnreachableTarget_AgreesWithRecursiveBacktrack()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatchCount, harness.TraverseComposed());

        Assert.Equal(harness.RecursiveBacktrack(), harness.TraverseComposed());
    }

    private static ExpressionAddOperatorsBenchmarks BuildHarness() =>
        new() { Number = SmallestNumber };
}
