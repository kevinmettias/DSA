using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignMemoryAllocatorBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - Free rescanning the whole memory array against
// Free visiting only the units a HashMap index says the id owns - so a harness whose arms disagree
// is timing two different problems. Setup derives the whole call script from Count alone, so the
// same Count must rebuild the same script, and that script has to fill the memory array exactly.
public sealed partial class DesignMemoryAllocatorBenchmarksTests
{
    private const int SmallestCount = 200;

    [Fact]
    public void Setup_SameCount_RebuildsTheSameAllocateAndFreeScript()
    {
        // The script makes Count one-unit allocations, each under its own fresh id, against a
        // memory array of exactly Count units - so every allocation takes the leftmost free unit
        // and one free per id releases exactly that one unit again. The freed total is therefore
        // Count, and anything less is what a script that reused an id, or an allocator that
        // dropped a unit, would report.
        Assert.Equal(SmallestCount, BuildHarness().ArrayScanFree());
        Assert.Equal(BuildHarness().ArrayScanFree(), BuildHarness().ArrayScanFree());
    }

    [Fact]
    public void ArrayScanFree_SingleUnitAllocationScript_AgreesWithHashMapTrackedFree()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapTrackedFree(), harness.ArrayScanFree());
    }

    [Fact]
    public void HashMapTrackedFree_SingleUnitAllocationScript_AgreesWithArrayScanFree()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayScanFree(), harness.HashMapTrackedFree());
    }

    private static DesignMemoryAllocatorBenchmarks BuildHarness()
    {
        var harness = new DesignMemoryAllocatorBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
