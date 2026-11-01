using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignMemoryAllocatorBenchmarks (ARCHITECTURE 17.9): the generic arm check
// compares the arms only with each other, so what stays here is a total derived from the script
// rather than from either arm: the script has to fill the memory array exactly.
public sealed partial class DesignMemoryAllocatorBenchmarksTests
{
    private const int SmallestCount = 200;

    // The script makes Count one-unit allocations, each under its own fresh id, against a memory array
    // of exactly Count units - so every allocation takes the leftmost free unit and one free per id
    // releases exactly that one unit again. The freed total is therefore Count, and anything less is
    // what a script that reused an id, or an allocator that dropped a unit, would report.
    [Fact]
    public void ArrayScanFree_SingleUnitAllocationScript_FreesEveryAllocatedUnit() =>
        Assert.Equal(SmallestCount, BuildHarness().ArrayScanFree());

    private static DesignMemoryAllocatorBenchmarks BuildHarness()
    {
        var harness = new DesignMemoryAllocatorBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
