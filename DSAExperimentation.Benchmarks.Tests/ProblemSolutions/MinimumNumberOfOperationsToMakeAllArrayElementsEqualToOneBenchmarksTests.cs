using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneBenchmarks
// (ARCHITECTURE 17.9): both arms are
// MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution's, the same methods
// MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneTests proves correct, and both run
// the same window scan - they differ only in how each pairwise gcd step is computed - so arms
// that disagree are timing two different problems.
//
// The agreement asserted here is weak by construction: every generated value is a multiple of
// the fixture's factor, so no window's running gcd ever reaches 1, which is exactly what forces
// the full scan and also pins both arms to the same "no window works" answer. Agreement still
// catches an arm that reports a reachable window the other does not, but it would also hold for
// an arm that returned that constant without computing anything.
public sealed partial class MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneBenchmarksTests
{
    // The smallest declared [Params] value: both arms scan every (start, end) window, so a
    // shorter array is the cheaper way to reach the same gcd comparison.
    private const int SmallestLength = 30;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().MinOperationsBySubtractionGcd(),
            BuildHarness().MinOperationsBySubtractionGcd());

    [Fact]
    public void MinOperationsBySubtractionGcd_AgreesWithMinOperationsByEuclideanGcd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MinOperationsByEuclideanGcd(), harness.MinOperationsBySubtractionGcd());
    }

    [Fact]
    public void MinOperationsByEuclideanGcd_AgreesWithMinOperationsBySubtractionGcd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MinOperationsBySubtractionGcd(), harness.MinOperationsByEuclideanGcd());
    }

    private static MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
