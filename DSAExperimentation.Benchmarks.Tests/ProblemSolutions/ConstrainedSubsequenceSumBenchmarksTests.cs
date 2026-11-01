using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstrainedSubsequenceSumBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt workload, so this asserts only a bound that comes
// from the workload's shape rather than from either arm.
//
// The array is private, so its documented shape is asserted through the arm's own answer: values are
// drawn from [-SmallestValueBound, SmallestValueBound) and the answer is the sum of a subsequence of
// at most Length of them, so it lands inside [0, Length * SmallestValueBound] for a seeded draw that
// is not entirely negative - which a 2000-draw run from this seed is not.
public sealed partial class ConstrainedSubsequenceSumBenchmarksTests
{
    private const int SmallestLength = 2_000;
    private const int SmallestValueBound = 1_000;

    [Fact]
    public void RescanWindowEachPosition_WindowOfFifty_StaysWithinTheValueBound() =>
        Assert.InRange(
            BuildHarness().RescanWindowEachPosition(),
            0,
            SmallestLength * SmallestValueBound);

    private static ConstrainedSubsequenceSumBenchmarks BuildHarness()
    {
        var harness = new ConstrainedSubsequenceSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
