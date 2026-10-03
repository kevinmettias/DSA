using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindPositiveIntegerSolutionForAGivenEquationBenchmarks (ARCHITECTURE 17.9), for what
// BenchmarkArmsTests cannot pin: which pairs solve the equation, known from Setup's construction rather than from
// any arm. Each arm returns the solving pairs, and every arm walks x ascending and finds at most one y per x.
//
// Setup pins the target one below the largest reachable sum (Bound + Bound), which the arms' own
// sum oracle can only hit with the two pairs straddling it - (Bound - 1, Bound) and (Bound, Bound -
// 1) - so the answer is exactly two pairs and never an early exit at the first or last probe.
public sealed partial class FindPositiveIntegerSolutionForAGivenEquationBenchmarksTests
{
    // The smaller of Setup's [Params(300, 1_000)] bounds.
    private const int SmallestBound = 300;

    // Target (2 * Bound) - 1 is one below the largest reachable sum, so only the two pairs straddling
    // it solve x + y == target inside 1..Bound.
    private const int ExpectedSolutionCount = 2;

    [Fact]
    public void BruteForceEveryPair_TargetOneBelowTheLargestSum_FindsTheTwoStraddlingPairs() =>
        Assert.Equal(StraddlingPairs(), BuildHarness().BruteForceEveryPair());

    [Fact]
    public void TwoPointer_TargetOneBelowTheLargestSum_FindsTheTwoStraddlingPairs() =>
        Assert.Equal(StraddlingPairs(), BuildHarness().TwoPointer());

    [Fact]
    public void BinarySearchPerRow_TargetOneBelowTheLargestSum_FindsTheTwoStraddlingPairs() =>
        Assert.Equal(StraddlingPairs(), BuildHarness().BinarySearchPerRow());

    // x ascending, as every arm walks it.
    private static (int X, int Y)[] StraddlingPairs() =>
        [(SmallestBound - 1, SmallestBound), (SmallestBound, SmallestBound - 1)];

    private static FindPositiveIntegerSolutionForAGivenEquationBenchmarks BuildHarness()
    {
        var harness = new FindPositiveIntegerSolutionForAGivenEquationBenchmarks { Bound = SmallestBound };
        harness.Setup();

        return harness;
    }

    // The benchmark hoists one ICustomFunction - its own private nested SumFunction, f(x, y) = x + y -
    // and all three arms answer through it, so every answer above is a statement about that oracle rather
    // than about a sum computed inline. Its companion is this nested class, named for the type that
    // declares Evaluate and carrying the test that addresses it: the arithmetic is reachable only
    // through the arms, and it is the arithmetic the answers depend on.
    public sealed partial class SumFunctionTests
    {
        [Fact]
        public void Evaluate_SumOfTwoPositiveIntegers_ReturnsTheTwoStraddlingPairs()
        {
            var harness = BuildHarness();

            Assert.Equal(ExpectedSolutionCount, harness.BruteForceEveryPair().Count);
            Assert.Equal(ExpectedSolutionCount, harness.TwoPointer().Count);
            Assert.Equal(ExpectedSolutionCount, harness.BinarySearchPerRow().Count);
        }
    }
}
