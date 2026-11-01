using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ContainsDuplicateBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on a rebuilt workload, so this pins the baseline to the verdict the
// workload's shape decides, derived independently of either arm.
//
// The array is private, but the fixture makes its documented shape decisive: Setup builds a
// shuffled 0..Length-1 range, one value per position and no repeat, so the answer is false - the
// worst case the class comment says the workload exists to force, with no duplicate anywhere to
// stop the scan early.
public sealed partial class ContainsDuplicateBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void HasDuplicateByBruteForce_DuplicateFreeArray_ReturnsFalse() =>
        Assert.False(BuildHarness().HasDuplicateByBruteForce());

    private static ContainsDuplicateBenchmarks BuildHarness()
    {
        var harness = new ContainsDuplicateBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
