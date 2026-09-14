using static DSAExperimentation.LeetCode.DesignMemoryAllocator.DesignMemoryAllocatorSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignMemoryAllocator;

// Harness only. Both strategies are DesignMemoryAllocatorSolution's - this file
// replays LeetCode's published call sequence, plus four hand-traced fragmentation
// cases, against each IMemoryAllocatorStrategy implementation via a small operation
// script, so a failure still names the strategy that broke even though the "input"
// here is a sequence of mutating calls rather than a single argument tuple.
// MemoryAllocatorOp.Apply is pure dispatch (which method to call with which
// arguments) - no run-finding or bookkeeping logic of its own.
public sealed class DesignMemoryAllocatorTests
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
        int n, MemoryAllocatorOp[] operations, int[] expected) =>
        RunScript(new MemoryAllocatorByArrayScan(n), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void MemoryAllocatorByHashMapIndex_LeetCodeExamples_MatchesPublishedOutputSequence(
        int n, MemoryAllocatorOp[] operations, int[] expected) =>
        RunScript(new MemoryAllocatorByHashMapIndex(n), operations, expected);

    private static void RunScript(IMemoryAllocatorStrategy allocator, MemoryAllocatorOp[] operations, int[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(allocator));
        }
    }
}

// One call in an allocator script: which method to invoke and with what arguments.
// Pure dispatch, built via the named factories below so a script (like Examples
// above) reads like the LeetCode call sequence it replays.
public readonly record struct MemoryAllocatorOp
{
    private readonly bool _isFree;
    private readonly int _size;
    private readonly int _memoryId;

    private MemoryAllocatorOp(bool isFree, int size, int memoryId)
    {
        _isFree = isFree;
        _size = size;
        _memoryId = memoryId;
    }

    public static MemoryAllocatorOp Allocate(int size, int memoryId) => new(isFree: false, size, memoryId);

    public static MemoryAllocatorOp Free(int memoryId) => new(isFree: true, size: 0, memoryId);

    // Both allocator calls report an int, so one expected value per operation is
    // enough and no null placeholder is needed. Internal, not public:
    // IMemoryAllocatorStrategy is internal to DesignMemoryAllocatorSolution, and
    // only this same assembly's RunScript ever calls Apply.
    internal int Apply(IMemoryAllocatorStrategy allocator)
        => _isFree ? allocator.Free(_memoryId) : allocator.Allocate(_size, _memoryId);
}
