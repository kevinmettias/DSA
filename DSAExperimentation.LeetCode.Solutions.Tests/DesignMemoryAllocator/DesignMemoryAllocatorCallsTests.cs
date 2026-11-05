using DSAExperimentation.LeetCode.Solutions.DesignMemoryAllocator;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DesignMemoryAllocator;

public sealed partial class DesignMemoryAllocatorCallsTests
{
    [Fact]
    public void FreeMemory_IdHoldingUnits_FreesThemAll()
    {
        var allocator = DesignMemoryAllocatorSolution.CreateByArrayScan(10);
        allocator.Allocate(3, 1);

        Assert.Equal(3, DesignMemoryAllocatorCalls.FreeMemory(allocator, 1));
    }

    [Fact]
    public void FreeMemory_IdHoldingNothing_FreesNothing() =>
        Assert.Equal(0, DesignMemoryAllocatorCalls.FreeMemory(DesignMemoryAllocatorSolution.CreateByArrayScan(10), 1));
}
