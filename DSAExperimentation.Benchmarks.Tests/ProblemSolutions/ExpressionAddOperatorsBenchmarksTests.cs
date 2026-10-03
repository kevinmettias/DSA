using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ExpressionAddOperatorsBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin:
// that the harness's target is unreachable, so both arms walk the full O(4^n) space and still build nothing. The
// class has no [GlobalSetup]; the [Params] number is the whole workload, and the smallest one (seven digits, 4^7
// partial expressions) is already real work.
public sealed partial class ExpressionAddOperatorsBenchmarksTests
{
    private const string SmallestNumber = "1234567";

    // The harness's documented unreachable target: every expression built from the digits is orders
    // of magnitude away from int.MinValue, so neither strategy ever short-circuits and neither finds
    // a match.
    [Fact]
    public void RecursiveBacktrack_UnreachableTarget_BuildsNoExpression() =>
        Assert.Empty(BuildHarness().RecursiveBacktrack());

    [Fact]
    public void TraverseComposed_UnreachableTarget_BuildsNoExpression() =>
        Assert.Empty(BuildHarness().TraverseComposed());

    private static ExpressionAddOperatorsBenchmarks BuildHarness() =>
        new() { Number = SmallestNumber };
}
