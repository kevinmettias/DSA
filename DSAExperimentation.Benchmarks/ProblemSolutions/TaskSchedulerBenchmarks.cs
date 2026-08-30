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

    [Params(2_000, 40_000)]
    public int TaskCount;

    private char[] _tasks = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(621);
        _tasks = Enumerable.Range(0, TaskCount)
            .Select(_ => (char)('A' + random.Next(AlphabetSize)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int TickByTickArrayScan()
    {
        var counts = new int[26];
        foreach (var task in _tasks)
        {
            counts[task - 'A']++;
        }

        var lastUsed = new int[26];
        Array.Fill(lastUsed, int.MinValue / 2);

        var remaining = _tasks.Length;
        var time = 0;

        while (remaining > 0)
        {
            var best = -1;

            for (var i = 0; i < 26; i++)
            {
                if (counts[i] > 0 && time - lastUsed[i] > Cooldown && (best == -1 || counts[i] > counts[best]))
                {
                    best = i;
                }
            }

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

    [Benchmark]
    public int HashMapHeapAndCooldownQueue()
    {
        var counts = new HashMap<char, int>();

        foreach (var task in _tasks)
        {
            counts.TryGetValue(task, out var count);
            counts.Set(task, count + 1);
        }

        var heap = new Heap<int, MaxHeapOrder<int>>();

        foreach (var task in counts.Keys)
        {
            counts.TryGetValue(task, out var frequency);
            heap.Push(frequency);
        }

        var cooldown = new RepoQueue();
        var time = 0;

        while (heap.Count > 0 || cooldown.Count > 0)
        {
            time++;

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

        return time;
    }
}
