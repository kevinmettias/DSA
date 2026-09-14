using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignMemoryAllocator.DesignMemoryAllocatorSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignMemoryAllocatorSolution's, the same classes
// DesignMemoryAllocatorTests proves correct. The two strategies share the identical
// Allocate (a leftmost-free-run scan over a plain int[] memory array - every
// accepted solution does this, so it is not the axis being compared) and differ only
// in Free: the array-scan arm rescans the whole memory array for every unit still
// stamped with mID on every call, while the HashMap-indexed arm visits only the
// units that mID actually owns.
//
// [GlobalSetup] builds the call script - `Count` single-unit allocations, each given
// its own distinct mID (so memory fills completely with no gaps), followed by
// freeing every one of those `Count` mIDs. Both arms still pay Allocate's real
// O(Count^2) leftmost-run scanning cost, so the composed HashMap index only pays off
// on the Free half; the speedup is real but bounded, not an asymptotic win (see
// MatrixCellsInDistanceOrderBenchmarks for the same "correct composition, not always
// a complexity-class jump" shape).
[MemoryDiagnoser]
public class DesignMemoryAllocatorBenchmarks
{
    // Every allocation in this workload is one unit wide.
    private const int SingleUnit = 1;

    [Params(200, 2_000)]
    public int Count;

    private int[] _memoryIds = null!;

    [GlobalSetup]
    public void Setup() => _memoryIds = Enumerable.Range(1, Count).ToArray();

    [Benchmark(Baseline = true)]
    public int ArrayScanFree() => Replay(new MemoryAllocatorByArrayScan(Count));

    [Benchmark]
    public int HashMapTrackedFree() => Replay(new MemoryAllocatorByHashMapIndex(Count));

    // Sums every reported unit count rather than discarding it, so the JIT can't
    // eliminate the replay as dead code.
    private int Replay(IMemoryAllocatorStrategy allocator)
    {
        foreach (var memoryId in _memoryIds)
        {
            allocator.Allocate(SingleUnit, memoryId);
        }

        var freed = 0;

        foreach (var memoryId in _memoryIds)
        {
            freed += allocator.Free(memoryId);
        }

        return freed;
    }
}
