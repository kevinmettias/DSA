using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignCircularDequeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a wraparound array against this repo's own Deque -
// so a harness whose arms disagree is timing two different problems. There is no [GlobalSetup]:
// each arm constructs its own deque and churns the same insert/delete cycle at the smallest
// Capacity, which is enough to force every operation through the wraparound path on both ends.
public sealed partial class DesignCircularDequeBenchmarksTests
{
    private const int SmallestCapacity = 8;

    [Fact]
    public void ArrayBacked_TwoEndedChurnCycle_AgreesWithDequeBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DequeBacked(), harness.ArrayBacked());
    }

    [Fact]
    public void DequeBacked_TwoEndedChurnCycle_AgreesWithArrayBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayBacked(), harness.DequeBacked());
    }

    private static DesignCircularDequeBenchmarks BuildHarness() => new() { Capacity = SmallestCapacity };
}
