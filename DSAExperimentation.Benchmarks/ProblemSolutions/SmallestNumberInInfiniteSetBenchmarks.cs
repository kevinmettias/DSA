using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Smallest Number in Infinite Set (LC 2336): a plain List<int> holding added-back numbers, doing
// an O(n) linear scan for the minimum plus an O(n) Contains check per AddBack, vs. this repo's own
// Heap<Element,MinHeapOrder<Element>> + Set<Element> (SmallestNumberInInfiniteSetTests' exact
// composition) giving O(log n) PopSmallest and O(1) duplicate rejection. Both replay the identical
// operation sequence, generated so AddBack calls interleave with PopSmallest often enough that the
// added-back collection stays non-trivially populated instead of draining to empty every time.
[MemoryDiagnoser]
public class SmallestNumberInInfiniteSetBenchmarks
{
    private const int OpsCapacityMultiplier = 2;
    private const int PopOpType = 0;
    private const int AddBackOpType = 1;
    private const int RandomSeed = 2336;

    [Params(200, 5_000)]
    public int Length;

    private (int Type, int Num)[] _ops = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var ops = new List<(int Type, int Num)>(Length * OpsCapacityMultiplier);
        var producedSoFar = 0;

        for (var i = 0; i < Length; i++)
        {
            ops.Add((PopOpType, 0));
            producedSoFar++;

            if (producedSoFar > 1 && random.Next(2) == 0)
            {
                ops.Add((AddBackOpType, random.Next(1, producedSoFar)));
            }
        }

        _ops = [.. ops];
    }

    [Benchmark(Baseline = true)]
    public long ListScanPerOperation()
    {
        var addedBack = new List<int>();
        var nextUnused = 1;
        var checksum = 0L;

        foreach (var (type, num) in _ops)
        {
            if (type == AddBackOpType)
            {
                if (num < nextUnused && !addedBack.Contains(num))
                {
                    addedBack.Add(num);
                }

                continue;
            }

            checksum += PopSmallestFromList(addedBack, ref nextUnused);
        }

        return checksum;
    }

    [Benchmark]
    public long HeapAndSetPerOperation()
    {
        var addedBack = new Heap<int, MinHeapOrder<int>>();
        var pending = new Set<int>();
        var nextUnused = 1;
        var checksum = 0L;

        foreach (var (type, num) in _ops)
        {
            if (type == AddBackOpType)
            {
                if (num < nextUnused && pending.TryAdd(num))
                {
                    addedBack.Push(num);
                }

                continue;
            }

            if (addedBack.TryPop(out var restored))
            {
                pending.TryRemove(restored);
                checksum += restored;
            }
            else
            {
                checksum += nextUnused++;
            }
        }

        return checksum;
    }

    private static int PopSmallestFromList(List<int> addedBack, ref int nextUnused)
    {
        if (addedBack.Count == 0)
        {
            return nextUnused++;
        }

        var minIndex = 0;
        for (var i = 1; i < addedBack.Count; i++)
        {
            if (addedBack[i] < addedBack[minIndex])
            {
                minIndex = i;
            }
        }

        var smallest = addedBack[minIndex];
        addedBack.RemoveAt(minIndex);
        return smallest;
    }
}
