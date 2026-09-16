using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignBitsetBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the textbook flip-every-bit array against the same array with
// a lazy flag it reports through - so a harness whose arms disagree is timing two different
// problems. There is no [GlobalSetup]: each arm constructs its own bitset and replays the same
// fix/unfix-then-flip-and-count workload, so the smallest Size is enough to drive either.
public sealed partial class DesignBitsetBenchmarksTests
{
    private const int SmallestSize = 500;

    [Fact]
    public void EagerArrayFlip_ScatteredFixThenFlipWorkload_AgreesWithLazyFlagDynamicArray()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LazyFlagDynamicArray(), harness.EagerArrayFlip());
    }

    [Fact]
    public void LazyFlagDynamicArray_ScatteredFixThenFlipWorkload_AgreesWithEagerArrayFlip()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.EagerArrayFlip(), harness.LazyFlagDynamicArray());
    }

    private static DesignBitsetBenchmarks BuildHarness() => new() { Size = SmallestSize };
}
