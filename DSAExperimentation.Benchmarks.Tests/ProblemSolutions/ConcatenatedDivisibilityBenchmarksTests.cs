using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConcatenatedDivisibilityBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt workload, so this asserts only the answer's shape,
// which comes from the problem rather than from either arm. The numbers and the divisor are both
// private, and IList<int> is the only thing either arm reports, so the shape is asserted through
// that: LC 3533's answer is either the empty arrangement (no permutation's concatenation is
// divisible) or a full SmallestNumberCount-element permutation - never a partial one.
public sealed partial class ConcatenatedDivisibilityBenchmarksTests
{
    private const int SmallestNumberCount = 6;

    [Fact]
    public void Backtracking_SixSeededNumbers_ReturnsNoneOrAFullPermutation() =>
        Assert.True(BuildHarness().Backtracking(SmallestNumberCount).Count is 0 or SmallestNumberCount);

    private static ConcatenatedDivisibilityBenchmarks BuildHarness()
    {
        var harness = new ConcatenatedDivisibilityBenchmarks();
        harness.Setup();

        return harness;
    }
}
