using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GuessNumberHigherOrLowerBenchmarks (ARCHITECTURE 17.9): both arms are
// GuessNumberHigherOrLowerSolution's - the linear scan against the halving search - so a harness
// whose arms disagree is timing two different problems. Both arms answer with the picked number
// itself, and that number is decisive from the fixture alone: [GlobalSetup] places the pick at a
// fixed fraction of NumberCount, which is what makes the linear scan pay close to its full O(n)
// worst case, and the number both arms must report is that same value. Setup derives it from
// NumberCount and nothing else, so the same NumberCount must rebuild the same pick.
public sealed partial class GuessNumberHigherOrLowerBenchmarksTests
{
    private const int SmallestNumberCount = 1_000;

    // The fraction [GlobalSetup] places the pick at, restated so the expected number can be derived
    // from the fixture rather than read back out of an arm.
    private const double PickFraction = 0.7;

    [Fact]
    public void Setup_SameNumberCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_PickNearTheTopOfTheRange_AgreesWithBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPick, harness.LinearScan());
        Assert.Equal(harness.BinarySearch(), harness.LinearScan());
    }

    [Fact]
    public void BinarySearch_PickNearTheTopOfTheRange_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPick, harness.BinarySearch());
        Assert.Equal(harness.LinearScan(), harness.BinarySearch());
    }

    private static int ExpectedPick => (int)(SmallestNumberCount * PickFraction);

    private static GuessNumberHigherOrLowerBenchmarks BuildHarness()
    {
        var harness = new GuessNumberHigherOrLowerBenchmarks { NumberCount = SmallestNumberCount };
        harness.Setup();

        return harness;
    }
}
