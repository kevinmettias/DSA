using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindPositiveIntegerSolutionForAGivenEquationBenchmarks (ARCHITECTURE 17.9), for what
// BenchmarkArmsTests cannot pin: which pairs solve the equation, known from Setup's construction rather than from
// any arm. Each arm returns the solving pairs, and every arm walks x ascending and finds at most one y per x.
//
// The target is LC 1237's largest z, 100, which the arms' own sum oracle hits with exactly the 99 pairs
// (x, 100 - x) for x from 1 to 99 - every one inside 1..Bound, since Bound is at least 300.
public sealed partial class FindPositiveIntegerSolutionForAGivenEquationBenchmarksTests
{
    // The smaller of Setup's [Params(300, 1_000)] bounds.
    private const int SmallestBound = 300;

    // The benchmark's target, LC 1237's largest z.
    private const int TargetValue = 100;

    // x + y == 100 for every x from 1 to 99.
    private const int ExpectedSolutionCount = TargetValue - 1;

    [Fact]
    public void BruteForceEveryPair_LargestTarget_FindsEveryPairSummingToIt() =>
        Assert.Equal(PairsSummingToTarget(), BuildHarness().BruteForceEveryPair());

    [Fact]
    public void TwoPointer_LargestTarget_FindsEveryPairSummingToIt() =>
        Assert.Equal(PairsSummingToTarget(), BuildHarness().TwoPointer());

    [Fact]
    public void BinarySearchPerRow_LargestTarget_FindsEveryPairSummingToIt() =>
        Assert.Equal(PairsSummingToTarget(), BuildHarness().BinarySearchPerRow());

    // x ascending, as every arm walks it.
    private static (int X, int Y)[] PairsSummingToTarget() =>
        [.. Enumerable.Range(1, ExpectedSolutionCount).Select(x => (x, TargetValue - x))];

    private static FindPositiveIntegerSolutionForAGivenEquationBenchmarks BuildHarness() =>
        new() { Bound = SmallestBound };

    // The benchmark hoists one ICustomFunction - its own private nested SumFunction, f(x, y) = x + y -
    // and all three arms answer through it, so every answer above is a statement about that oracle rather
    // than about a sum computed inline. Its companion is this nested class, named for the type that
    // declares Evaluate and carrying the test that addresses it: the arithmetic is reachable only
    // through the arms, and it is the arithmetic the answers depend on.
    public sealed partial class SumFunctionTests
    {
        [Fact]
        public void Evaluate_SumOfTwoPositiveIntegers_ReturnsEveryPairSummingToTheTarget()
        {
            var harness = BuildHarness();

            Assert.Equal(ExpectedSolutionCount, harness.BruteForceEveryPair().Count);
            Assert.Equal(ExpectedSolutionCount, harness.TwoPointer().Count);
            Assert.Equal(ExpectedSolutionCount, harness.BinarySearchPerRow().Count);
        }
    }
}
