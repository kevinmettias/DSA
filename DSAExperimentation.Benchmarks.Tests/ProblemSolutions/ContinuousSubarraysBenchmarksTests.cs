using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ContinuousSubarraysBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on a rebuilt workload, so this asserts only a bound that comes from the
// problem rather than from either arm: every single-element subarray has spread zero, which the
// problem's limit of 2 admits, so no admissible answer can ever fall below the number of starting
// points.
public sealed partial class ContinuousSubarraysBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void DoubleMonotonicDeque_NarrowValues_CountsAtLeastEveryStartingPoint() =>
        Assert.True(BuildHarness().DoubleMonotonicDeque() >= SmallestLength);

    private static ContinuousSubarraysBenchmarks BuildHarness()
    {
        var harness = new ContinuousSubarraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
