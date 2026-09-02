using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignMemoryAllocator;

// LeetCode 2502. Design Memory Allocator: Allocate scans a plain int[] memory
// array (0 = free) for the leftmost run of `size` free units and stamps mID into
// it - no data structure abstracts "find the leftmost gap" any better than direct
// indexed access here, the same raw-array role DesignANumberContainerSystemTests'
// own NumberContainers gives its numberByIndex array. Free is where a repo
// primitive earns its place: rather than rescanning the whole array for every
// unit still carrying mID, this composes HashMap<int, DynamicArray<int>> to
// remember exactly which indices each mID currently owns, so Free only visits
// those indices - the same "HashMap index over an array" shape
// DesignANumberContainerSystemTests already uses, just tracking a set of indices
// per key instead of one.
public sealed partial class DesignMemoryAllocatorTests
{
    [Fact]
    public void Allocator_LeetCodeExample_MatchesPublishedOutputSequence()
    {
        var allocator = new Allocator(10);

        Assert.Equal(0, allocator.Allocate(1, 1));
        Assert.Equal(1, allocator.Allocate(1, 2));
        Assert.Equal(2, allocator.Allocate(1, 3));
        Assert.Equal(1, allocator.Free(2));
        Assert.Equal(3, allocator.Allocate(3, 4));
        Assert.Equal(1, allocator.Allocate(1, 1));
        Assert.Equal(6, allocator.Allocate(1, 1));
        Assert.Equal(3, allocator.Free(1));
        Assert.Equal(-1, allocator.Allocate(10, 2));
        Assert.Equal(0, allocator.Free(7));
    }

    [Fact]
    public void Allocate_NoRunLargeEnoughRemains_ReturnsNegativeOne()
    {
        var allocator = new Allocator(3);

        Assert.Equal(0, allocator.Allocate(2, 1));
        Assert.Equal(-1, allocator.Allocate(2, 2));
    }

    private sealed class Allocator(int n)
    {
        private readonly int[] _memory = new int[n];
        private readonly HashMap<int, DynamicArray<int>> _blocksByMemoryId = new();

        public int Allocate(int size, int mID)
        {
            var runStart = FindLeftmostFreeRun(size);

            if (runStart < 0)
            {
                return -1;
            }

            MarkAllocated(runStart, size, mID);
            return runStart;
        }

        public int Free(int mID)
        {
            if (!_blocksByMemoryId.TryGetValue(mID, out var indices))
            {
                return 0;
            }

            for (var i = 0; i < indices.Count; i++)
            {
                _memory[indices.Get(i)] = 0;
            }

            _blocksByMemoryId.TryRemove(mID);
            return indices.Count;
        }

        private int FindLeftmostFreeRun(int size)
        {
            var runStart = -1;
            var runLength = 0;

            for (var i = 0; i < _memory.Length; i++)
            {
                if (_memory[i] != 0)
                {
                    runStart = -1;
                    runLength = 0;
                    continue;
                }

                runStart = runLength == 0 ? i : runStart;
                runLength++;

                if (runLength == size)
                {
                    return runStart;
                }
            }

            return -1;
        }

        private void MarkAllocated(int start, int size, int mID)
        {
            if (!_blocksByMemoryId.TryGetValue(mID, out var indices))
            {
                indices = new DynamicArray<int>();
                _blocksByMemoryId.Set(mID, indices);
            }

            for (var i = start; i < start + size; i++)
            {
                _memory[i] = mID;
                indices.Add(i);
            }
        }
    }
}
