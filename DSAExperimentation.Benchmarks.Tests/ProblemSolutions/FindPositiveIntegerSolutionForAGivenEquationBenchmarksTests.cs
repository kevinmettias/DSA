using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindPositiveIntegerSolutionForAGivenEquationBenchmarks (ARCHITECTURE 17.9):
// all three arms are competing strategies for the same question - the every-pair brute force, the
// two-pointer walk and this repo's own BinarySearch.Find applied per row - so a harness whose arms
// disagree has solved two different equations. Each arm returns how many pairs solve it, and every
// arm walks x ascending and finds at most one y per x, so the arms are compared directly on that
// count and each is asserted against the count the workload's own arithmetic makes decisive.
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
    public void Setup_SameBound_RebuildsTheSameSolutionCount()
    {
        Assert.Equal(ExpectedSolutionCount, BuildHarness().BruteForceEveryPair());

        Assert.Equal(BuildHarness().BruteForceEveryPair(), BuildHarness().BruteForceEveryPair());
    }

    [Fact]
    public void BruteForceEveryPair_TargetOneBelowTheLargestSum_AgreesWithTwoPointer()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSolutionCount, harness.BruteForceEveryPair());
        Assert.Equal(harness.TwoPointer(), harness.BruteForceEveryPair());
    }

    [Fact]
    public void TwoPointer_TargetOneBelowTheLargestSum_AgreesWithBruteForceEveryPair()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSolutionCount, harness.TwoPointer());
        Assert.Equal(harness.BruteForceEveryPair(), harness.TwoPointer());
    }

    [Fact]
    public void BinarySearchPerRow_TargetOneBelowTheLargestSum_AgreesWithBruteForceEveryPair()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSolutionCount, harness.BinarySearchPerRow());
        Assert.Equal(harness.BruteForceEveryPair(), harness.BinarySearchPerRow());
    }

    [Fact]
    public void Evaluate_AllThreeArmsProbeTheSameSumFunction_AgreeOnTheSolutionCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceEveryPair(), harness.BinarySearchPerRow());
        Assert.Equal(harness.BruteForceEveryPair(), harness.TwoPointer());
    }

    private static FindPositiveIntegerSolutionForAGivenEquationBenchmarks BuildHarness()
    {
        var harness = new FindPositiveIntegerSolutionForAGivenEquationBenchmarks { Bound = SmallestBound };
        harness.Setup();

        return harness;
    }

    // The benchmark hoists one ICustomFunction - its own private nested SumFunction, f(x, y) = x + y -
    // and all three arms answer through it, so every count above is a statement about that oracle rather
    // than about a sum computed inline. Its companion is this nested class, named for the type that
    // declares Evaluate and carrying the test that addresses it: the arithmetic is reachable only
    // through the arms, and it is the arithmetic the counts depend on.
    public sealed partial class SumFunctionTests
    {
        [Fact]
        public void Evaluate_SumOfTwoPositiveIntegers_ReturnsTheTwoStraddlingPairs()
        {
            var harness = BuildHarness();

            Assert.Equal(ExpectedSolutionCount, harness.BruteForceEveryPair());
            Assert.Equal(ExpectedSolutionCount, harness.TwoPointer());
            Assert.Equal(ExpectedSolutionCount, harness.BinarySearchPerRow());
        }
    }
}
