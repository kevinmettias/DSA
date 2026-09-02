using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Remaining, int AvailableAt)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Task Scheduler (LC 621): the naive tick-by-tick simulation that rescans a flat
// 26-slot frequency array every CPU tick to find the most-frequent available task
// (O(ticks * 26)) vs. this repo's own HashMap<char,int> (frequency counting) +
// Heap<int,MaxHeapOrder<int>> (always offers the most-frequent remaining task) +
// Queue<(int,int)> (holds a just-run task until its cooldown expires) - O(ticks *
// log distinctTasks) instead of O(ticks * 26).
[MemoryDiagnoser]
public class TaskSchedulerBenchmarks
{
    private const int Cooldown = 3;
    private const int AlphabetSize = 6;

    // LC problem number, used as the deterministic seed for task-sequence generation.
    private const int RandomSeed = 621;

    // Fixed A-Z array width used for direct-index frequency/last-used tracking,
    // independent of AlphabetSize (how many distinct tasks this benchmark's random data uses).
    private const int EnglishAlphabetSize = 26;

    // A safely-in-the-past "never used" sentinel, halved to avoid overflow when
    // computing `time - lastUsed[i]`.
    private const int NeverUsedSentinel = int.MinValue / 2;

    [Params(2_000, 40_000)]
    public int TaskCount;

    private char[] _tasks = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _tasks = Enumerable.Range(0, TaskCount)
            .Select(_ => (char)('A' + random.Next(AlphabetSize)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int TickByTickArrayScan()
    {
        var counts = BuildFrequencyCounts(_tasks);
        return SimulateTickByTick(counts, _tasks.Length);
    }

    private static int[] BuildFrequencyCounts(char[] tasks)
    {
        var counts = new int[EnglishAlphabetSize];

        foreach (var task in tasks)
        {
            counts[task - 'A']++;
        }

        return counts;
    }

    private static int SimulateTickByTick(int[] counts, int taskCount)
    {
        var lastUsed = new int[EnglishAlphabetSize];
        Array.Fill(lastUsed, NeverUsedSentinel);

        var remaining = taskCount;
        var time = 0;

        while (remaining > 0)
        {
            var best = FindBestAvailableTask(counts, lastUsed, time);

            if (best != -1)
            {
                counts[best]--;
                lastUsed[best] = time;
                remaining--;
            }

            time++;
        }

        return time;
    }

    private static int FindBestAvailableTask(int[] counts, int[] lastUsed, int time)
    {
        var best = -1;

        for (var i = 0; i < EnglishAlphabetSize; i++)
        {
            if (counts[i] > 0 && time - lastUsed[i] > Cooldown && (best == -1 || counts[i] > counts[best]))
            {
                best = i;
            }
        }

        return best;
    }

    [Benchmark]
    public int HashMapHeapAndCooldownQueue()
    {
        var counts = CountTaskFrequencies(_tasks);
        var heap = BuildFrequencyHeap(counts);
        return SimulateCooldownSchedule(heap);
    }

    private static HashMap<char, int> CountTaskFrequencies(char[] tasks)
    {
        var counts = new HashMap<char, int>();

        foreach (var task in tasks)
        {
            counts.TryGetValue(task, out var count);
            counts.Set(task, count + 1);
        }

        return counts;
    }

    private static Heap<int, MaxHeapOrder<int>> BuildFrequencyHeap(HashMap<char, int> counts)
    {
        var heap = new Heap<int, MaxHeapOrder<int>>();

        foreach (var task in counts.Keys)
        {
            counts.TryGetValue(task, out var frequency);
            heap.Push(frequency);
        }

        return heap;
    }

    private static int SimulateCooldownSchedule(Heap<int, MaxHeapOrder<int>> heap)
    {
        var cooldown = new RepoQueue();
        var time = 0;

        while (heap.Count > 0 || cooldown.Count > 0)
        {
            time++;
            AdvanceTick(heap, cooldown, time);
        }

        return time;
    }

    private static void AdvanceTick(Heap<int, MaxHeapOrder<int>> heap, RepoQueue cooldown, int time)
    {
        if (heap.TryPop(out var remaining))
        {
            remaining--;
            if (remaining > 0)
            {
                cooldown.Enqueue((remaining, time + Cooldown));
            }
        }

        if (cooldown.TryPeek(out var next) && next.AvailableAt == time)
        {
            cooldown.TryDequeue(out var ready);
            heap.Push(ready.Remaining);
        }
    }
}
