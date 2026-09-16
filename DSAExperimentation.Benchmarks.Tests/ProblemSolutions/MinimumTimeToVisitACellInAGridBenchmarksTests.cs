using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumTimeToVisitACellInAGridBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the BCL PriorityQueue expansion against this repo's
// own Heap - so a harness whose arms disagree is timing two different problems. Both arms only read
// the wait-cost grid built in [GlobalSetup] (whose (0,1) cell Setup clears so every relaxation goes
// through the wait-and-parity arrival logic rather than a constant weight), so one harness instance is
// safe to call twice in either order. Setup draws that grid from one fixed seed, so the same Size must
// rebuild the same grid; otherwise two published numbers were never comparable in the first place.
public sealed partial class MinimumTimeToVisitACellInAGridBenchmarksTests
{
    private const int SmallestSize = 20;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BclPriorityQueue(), BuildHarness().BclPriorityQueue());

    [Fact]
    public void BclPriorityQueue_SeededWaitCostGrid_AgreesWithHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Heap(), harness.BclPriorityQueue());
    }

    [Fact]
    public void Heap_SeededWaitCostGrid_AgreesWithBclPriorityQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BclPriorityQueue(), harness.Heap());
    }

    private static MinimumTimeToVisitACellInAGridBenchmarks BuildHarness()
    {
        var harness = new MinimumTimeToVisitACellInAGridBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
