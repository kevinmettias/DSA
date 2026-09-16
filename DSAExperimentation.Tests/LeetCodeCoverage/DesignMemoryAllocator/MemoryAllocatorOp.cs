using DSAExperimentation.LeetCode.DesignMemoryAllocator;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignMemoryAllocator;

// One call in an allocator script: which method to invoke and with what arguments.
// Pure dispatch, built via the named factories below so a script (like Examples in
// DesignMemoryAllocatorTests) reads like the LeetCode call sequence it replays.
public readonly record struct MemoryAllocatorOp(bool isFree, int size, int memoryId)
{
    public static MemoryAllocatorOp Allocate(int size, int memoryId) => new(isFree: false, size, memoryId);

    public static MemoryAllocatorOp Free(int memoryId) => new(isFree: true, size: 0, memoryId);

    // Both allocator calls report an int, so one expected value per operation is
    // enough and no null placeholder is needed. Internal, not public:
    // IMemoryAllocatorStrategy is internal to DesignMemoryAllocatorSolution, and
    // only this same assembly's RunScript ever calls Apply.
    internal int Apply(DesignMemoryAllocatorSolution.IMemoryAllocatorStrategy allocator)
        => isFree ? allocator.Free(memoryId) : allocator.Allocate(size, memoryId);
}
