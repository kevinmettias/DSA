using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Max Chunks To Make Sorted II (LC 768): the textbook boundary check re-scans the
// entire remaining suffix from scratch for every candidate split point (O(n) per
// candidate, O(n^2) worst case) vs. this repo's own monotonic Stack<int> of chunk
// maxes (DailyTemperaturesBenchmarks' exact shape), where each value is pushed once
// and popped at most once for a single O(n) pass. Values are a random permutation so
// no candidate boundary is confirmed or ruled out on the very next element.
[MemoryDiagnoser]
public class MaxChunksToMakeSortedIIBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(768);
        _values = Enumerable.Range(0, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceSuffixRescan() => MaxChunksBruteForce(_values);

    [Benchmark]
    public int MonotonicStackMerge() => MaxChunksToSorted(_values);

    private static int MaxChunksBruteForce(int[] arr)
    {
        var chunks = 0;
        var runningMax = int.MinValue;

        for (var i = 0; i < arr.Length; i++)
        {
            runningMax = Math.Max(runningMax, arr[i]);

            if (!RestIsAtLeast(arr, i + 1, runningMax))
            {
                continue;
            }

            chunks++;
            runningMax = int.MinValue;
        }

        return chunks;
    }

    private static bool RestIsAtLeast(int[] arr, int fromIndex, int threshold)
    {
        for (var j = fromIndex; j < arr.Length; j++)
        {
            if (arr[j] < threshold)
            {
                return false;
            }
        }

        return true;
    }

    private static int MaxChunksToSorted(int[] arr)
    {
        var chunkMaxes = new RepoIntStack();

        foreach (var num in arr)
        {
            if (chunkMaxes.TryPeek(out var currentMax) && num < currentMax)
            {
                chunkMaxes.TryPop(out var mergedMax);

                while (chunkMaxes.TryPeek(out var previousMax) && previousMax > num)
                {
                    chunkMaxes.TryPop(out _);
                }

                chunkMaxes.Push(mergedMax);
            }
            else
            {
                chunkMaxes.Push(num);
            }
        }

        return chunkMaxes.Count;
    }
}
