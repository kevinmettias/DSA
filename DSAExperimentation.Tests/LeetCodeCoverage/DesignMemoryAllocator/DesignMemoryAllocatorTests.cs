using DSAExperimentation.LeetCode.DesignMemoryAllocator;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignMemoryAllocator;

// Harness only. Both strategies are DesignMemoryAllocatorSolution's - this file
// replays LeetCode's published call sequence, plus four hand-traced fragmentation
// cases, against each IMemoryAllocatorStrategy implementation via a small operation
// script, so a failure still names the strategy that broke even though the "input"
// here is a sequence of mutating calls rather than a single argument tuple.
// MemoryAllocatorOp.Apply is pure dispatch (which method to call with which
// arguments) - no run-finding or bookkeeping logic of its own.
public sealed partial class DesignMemoryAllocatorTests
{
    public static TheoryData<int, MemoryAllocatorOp[], int[]> Examples =>
        new()
        {
            // LeetCode's published example: single-unit allocations, a free that
            // opens a one-unit hole too small for the next request, reuse of that
            // hole, a multi-block mID freed in one call, a request larger than any
            // remaining run, and a free of an mID that never allocated anything.
            {
                10,
                [
                    MemoryAllocatorOp.Allocate(1, 1),
                    MemoryAllocatorOp.Allocate(1, 2),
                    MemoryAllocatorOp.Allocate(1, 3),
                    MemoryAllocatorOp.Free(2),
                    MemoryAllocatorOp.Allocate(3, 4),
                    MemoryAllocatorOp.Allocate(1, 1),
                    MemoryAllocatorOp.Allocate(1, 1),
                    MemoryAllocatorOp.Free(1),
                    MemoryAllocatorOp.Allocate(10, 2),
                    MemoryAllocatorOp.Free(7),
                ],
                [0, 1, 2, 1, 3, 1, 6, 3, -1, 0]
            },

            // No run large enough remains, even though enough units are free in
            // total - the scan must not accept a shorter run.
            {
                3,
                [
                    MemoryAllocatorOp.Allocate(2, 1),
                    MemoryAllocatorOp.Allocate(2, 2),
                ],
                [0, -1]
            },

            // One mID owning two disjoint blocks: a single free releases both, and
            // the four freed units are still split by a live block in the middle, so
            // a size-4 request fails while a size-2 one takes the leftmost hole.
            // Freeing the same mID again reports 0, as does an mID never seen.
            {
                6,
                [
                    MemoryAllocatorOp.Allocate(2, 1),
                    MemoryAllocatorOp.Allocate(2, 2),
                    MemoryAllocatorOp.Allocate(2, 1),
                    MemoryAllocatorOp.Free(1),
                    MemoryAllocatorOp.Allocate(4, 3),
                    MemoryAllocatorOp.Allocate(2, 3),
                    MemoryAllocatorOp.Free(9),
                    MemoryAllocatorOp.Free(1),
                ],
                [0, 2, 4, 4, -1, 0, 0, 0]
            },

            // A request wider than the whole memory array, then the exact-fit
            // request, freed in full and immediately reallocated from index 0.
            {
                3,
                [
                    MemoryAllocatorOp.Allocate(4, 1),
                    MemoryAllocatorOp.Allocate(3, 1),
                    MemoryAllocatorOp.Free(1),
                    MemoryAllocatorOp.Allocate(1, 2),
                ],
                [-1, 0, 3, 0]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MemoryAllocatorByArrayScan_LeetCodeExamples_MatchesPublishedOutputSequence(
        int memorySize, MemoryAllocatorOp[] operations, int[] expected) =>
        Assert.Equal(expected, RunScript(new DesignMemoryAllocatorSolution.MemoryAllocatorByArrayScan(memorySize), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MemoryAllocatorByHashMapIndex_LeetCodeExamples_MatchesPublishedOutputSequence(
        int memorySize, MemoryAllocatorOp[] operations, int[] expected) =>
        Assert.Equal(expected, RunScript(new DesignMemoryAllocatorSolution.MemoryAllocatorByHashMapIndex(memorySize), operations));

    private static int[] RunScript(
        DesignMemoryAllocatorSolution.IMemoryAllocatorStrategy allocator,
        MemoryAllocatorOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(allocator))];
}
