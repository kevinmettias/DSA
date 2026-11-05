namespace DSAExperimentation.LeetCode.Solutions.DesignMemoryAllocator;

// LeetCode's freeMemory call on the allocator, which IMemoryAllocatorStrategy answers as Free (ARCHITECTURE
// 17.13).
internal static class DesignMemoryAllocatorCalls
{
    public static int FreeMemory(DesignMemoryAllocatorSolution.IMemoryAllocatorStrategy allocator, int mID) => allocator.Free(mID);
}
