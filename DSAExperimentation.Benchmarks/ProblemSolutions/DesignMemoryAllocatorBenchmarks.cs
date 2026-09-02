using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Memory Allocator (LC 2502): both strategies share the identical Allocate
// (leftmost-free-run scan over a plain int[] memory array - every accepted
// solution to this problem does this, so it is not the axis being compared here)
// and differ only in Free. ArrayScanFree rescans the whole memory array for every
// unit still stamped with mID, on every call - it has to, since a correct Free
// can't stop early without first ruling out a later match. HashMapTrackedFree
// instead composes HashMap<int, DynamicArray<int>> to remember exactly which
// indices each mID currently owns (the same "HashMap index over an array" shape
// DesignANumberContainerSystemBenchmarks already uses), so Free only visits those.
// Workload: `Count` single-unit allocations, each given its own distinct mID (so
// memory fills completely with no gaps), followed by freeing every one of those
// `Count` mIDs - both strategies still pay Allocate's real O(Count^2) leftmost-run
// scanning cost, so the composed HashMap index only pays off on the Free half; the
// speedup is real but bounded, not an asymptotic win (see
// MatrixCellsInDistanceOrderBenchmarks for the same "correct composition, not
// always a complexity-class jump" shape).
[MemoryDiagnoser]
public class DesignMemoryAllocatorBenchmarks
{
    [Params(200, 2_000)]
    public int Count;

    private int[] _memoryIds = null!;

    [GlobalSetup]
    public void Setup() => _memoryIds = Enumerable.Range(1, Count).ToArray();

    [Benchmark(Baseline = true)]
    public int ArrayScanFree()
    {
        var memory = new int[Count];

        foreach (var mID in _memoryIds)
        {
            memory[FindLeftmostFreeRun(memory)] = mID;
        }

        var freed = 0;

        foreach (var mID in _memoryIds)
        {
            freed += FreeByScan(memory, mID);
        }

        return freed;
    }

    private static int FreeByScan(int[] memory, int mID)
    {
        var freed = 0;

        for (var i = 0; i < memory.Length; i++)
        {
            if (memory[i] == mID)
            {
                memory[i] = 0;
                freed++;
            }
        }

        return freed;
    }

    [Benchmark]
    public int HashMapTrackedFree()
    {
        var memory = new int[Count];
        var blocksByMemoryId = new HashMap<int, DynamicArray<int>>();

        foreach (var mID in _memoryIds)
        {
            var index = FindLeftmostFreeRun(memory);
            memory[index] = mID;

            var indices = new DynamicArray<int>();
            indices.Add(index);
            blocksByMemoryId.Set(mID, indices);
        }

        var freed = 0;

        foreach (var mID in _memoryIds)
        {
            freed += FreeByHashMap(memory, blocksByMemoryId, mID);
        }

        return freed;
    }

    private static int FreeByHashMap(int[] memory, HashMap<int, DynamicArray<int>> blocksByMemoryId, int mID)
    {
        if (!blocksByMemoryId.TryGetValue(mID, out var indices))
        {
            return 0;
        }

        for (var i = 0; i < indices.Count; i++)
        {
            memory[indices.Get(i)] = 0;
        }

        blocksByMemoryId.TryRemove(mID);
        return indices.Count;
    }

    // Single-unit runs only (every allocation in this workload is size 1), so the
    // loop can return as soon as it sees one free cell - still the same leftmost-
    // free-cell scan a size-parameterized run search degrades to when size == 1.
    private static int FindLeftmostFreeRun(int[] memory)
    {
        for (var i = 0; i < memory.Length; i++)
        {
            if (memory[i] == 0)
            {
                return i;
            }
        }

        return -1;
    }
}
