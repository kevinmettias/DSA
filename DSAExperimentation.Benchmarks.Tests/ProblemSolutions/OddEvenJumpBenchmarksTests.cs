using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OddEvenJumpBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one count - the quadratic per-index forward scan against MergeSort plus a monotonic
// stack sweep - so a harness whose arms disagree is timing two different problems. Setup draws the
// array from one seeded Random, so the same Length must rebuild the same values; the smallest tuned
// Length keeps the quadratic arm's per-index scan cheap to call twice.
public sealed partial class OddEvenJumpBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceScan(), BuildHarness().BruteForceScan());

    [Fact]
    public void BruteForceScan_SmallestLength_AgreesWithMergeSortStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortStackSweep(), harness.BruteForceScan());
    }

    [Fact]
    public void MergeSortStackSweep_SmallestLength_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceScan(), harness.MergeSortStackSweep());
    }

    private static OddEvenJumpBenchmarks BuildHarness()
    {
        var harness = new OddEvenJumpBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
