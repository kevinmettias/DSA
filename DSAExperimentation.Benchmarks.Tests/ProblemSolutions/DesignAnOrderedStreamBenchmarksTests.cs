using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAnOrderedStreamBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a pre-sized List<string?> plus cursor against this
// repo's own DynamicArray<string?> doing the same pre-filled-slots-plus-cursor bookkeeping - so a
// harness whose arms disagree is timing two different problems. Setup builds the arrival order
// from one fixed seed, so the same Size must rebuild the same insertion script, and that script
// has to cover every id 1..Size for either arm to hand anything out at all.
public sealed partial class DesignAnOrderedStreamBenchmarksTests
{
    private const int SmallestSize = 200;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameInsertionScript()
    {
        // The cursor only ever advances, so each slot is handed out at most once over the whole
        // script: a total of exactly Size is what a script covering every id 1..Size produces once
        // the last gap closes, and it is what a script that dropped or mistyped an id - leaving a
        // slot permanently null and the cursor stuck short of the end - could not.
        Assert.Equal(SmallestSize, BuildHarness().ListBacked());
        Assert.Equal(BuildHarness().ListBacked(), BuildHarness().ListBacked());
    }

    [Fact]
    public void ListBacked_ShuffledArrivalOrder_AgreesWithDynamicArrayBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DynamicArrayBacked(), harness.ListBacked());
    }

    [Fact]
    public void DynamicArrayBacked_ShuffledArrivalOrder_AgreesWithListBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListBacked(), harness.DynamicArrayBacked());
    }

    private static DesignAnOrderedStreamBenchmarks BuildHarness()
    {
        var harness = new DesignAnOrderedStreamBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
