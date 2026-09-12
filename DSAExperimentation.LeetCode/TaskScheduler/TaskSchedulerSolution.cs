using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Remaining, int AvailableAt)>;

namespace DSAExperimentation.LeetCode.TaskScheduler;

// LeetCode 621. Task Scheduler: fewest CPU ticks to run every task at least once,
// where the same task must wait at least n ticks between occurrences (idle ticks
// allowed while waiting).
internal static class TaskSchedulerSolution
{
    // Fixed A-Z array width the baseline uses for direct-index frequency/last-used
    // tracking - LC's tasks are always uppercase letters.
    private const int EnglishAlphabetSize = 26;

    // A safely-in-the-past "never used" sentinel, halved to avoid overflow when
    // computing `time - lastUsed[i]`.
    private const int NeverUsedSentinel = int.MinValue / 2;

    // This repo's own HashMap<char,int> (frequency counting) + Heap<int,MaxHeapOrder<int>>
    // (always offers the most-frequent remaining task) + Queue<(int,int)> (holds a
    // just-run task until its cooldown expires before it's pushed back onto the
    // heap) - one CPU tick per loop iteration instead of the baseline's O(ticks *
    // 26) rescan every tick.
    public static int LeastIntervalByCooldownHeap(char[] tasks, int n)
    {
        var counts = BuildTaskCounts(tasks);
        var heap = BuildFrequencyHeap(counts);

        return RunCooldownSimulation(heap, n);
    }

    private static HashMap<char, int> BuildTaskCounts(char[] tasks)
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

    private static int RunCooldownSimulation(Heap<int, MaxHeapOrder<int>> heap, int n)
    {
        var cooldown = new RepoQueue();
        var time = 0;

        while (heap.Count > 0 || cooldown.Count > 0)
        {
            time++;
            AdvanceTick(heap, cooldown, time, n);
        }

        return time;
    }

    private static void AdvanceTick(Heap<int, MaxHeapOrder<int>> heap, RepoQueue cooldown, int time, int n)
    {
        if (heap.TryPop(out var remaining))
        {
            remaining--;
            if (remaining > 0)
            {
                cooldown.Enqueue((remaining, time + n));
            }
        }

        if (cooldown.TryPeek(out var next) && next.AvailableAt == time)
        {
            cooldown.TryDequeue(out var ready);
            heap.Push(ready.Remaining);
        }
    }

    // The textbook answer: rescan a flat 26-slot frequency/last-used array every
    // CPU tick to find the most-frequent available task - O(ticks * 26) instead of
    // O(ticks * log distinctTasks). Deliberately written without this repo's
    // primitives - it is the arm the composed solution above has to justify itself
    // against.
    public static int LeastIntervalByArrayScan(char[] tasks, int n)
    {
        var counts = BuildFrequencyCounts(tasks);

        return SimulateTickByTick(counts, tasks.Length, n);
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

    private static int SimulateTickByTick(int[] counts, int taskCount, int n)
    {
        var lastUsed = new int[EnglishAlphabetSize];
        Array.Fill(lastUsed, NeverUsedSentinel);

        var remaining = taskCount;
        var time = 0;

        while (remaining > 0)
        {
            var best = FindBestAvailableTask(counts, lastUsed, time, n);

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

    private static int FindBestAvailableTask(int[] counts, int[] lastUsed, int time, int n)
    {
        var best = -1;

        for (var i = 0; i < EnglishAlphabetSize; i++)
        {
            if (counts[i] > 0 && time - lastUsed[i] > n && (best == -1 || counts[i] > counts[best]))
            {
                best = i;
            }
        }

        return best;
    }
}
