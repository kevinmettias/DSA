using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Remaining, int AvailableAt)>;

namespace DSAExperimentation.LeetCode.TaskScheduler;

// LeetCode 621. Task Scheduler: fewest CPU ticks to run every task at least once,
// where the same task must wait at least `cooldownTicks` ticks between occurrences
// (idle ticks allowed while waiting).
internal static class TaskSchedulerSolution
{
    // Fixed A-Z array width the baseline uses for direct-index frequency/last-used
    // tracking - LC's tasks are always uppercase letters.
    private const int EnglishAlphabetSize = 26;

    // A safely-in-the-past "never used" sentinel, halved to avoid overflow when
    // computing `time - lastUsed[candidateIndex]`.
    private const int NeverUsedSentinel = int.MinValue / 2;

    // This repo's own HashMap<char,int> (frequency counting) + Heap<int,MaxHeapOrder<int>>
    // (always offers the most-frequent remaining task) + Queue<(int,int)> (holds a
    // just-run task until its cooldown expires before it's pushed back onto the
    // heap) - one CPU tick per loop iteration instead of the baseline's O(ticks *
    // 26) rescan every tick.
    public static int LeastIntervalByCooldownHeap(char[] tasks, int cooldownTicks)
    {
        var counts = BuildTaskCounts(tasks);
        var heap = BuildFrequencyHeap(counts);

        return RunCooldownSimulation(heap, cooldownTicks);
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

    private static int RunCooldownSimulation(Heap<int, MaxHeapOrder<int>> heap, int cooldownTicks)
    {
        var cooldown = new RepoQueue();
        var time = 0;

        while (heap.Count > 0 || cooldown.Count > 0)
        {
            time++;
            AdvanceTick(heap, cooldown, time, cooldownTicks);
        }

        return time;
    }

    private static void AdvanceTick(
        Heap<int, MaxHeapOrder<int>> heap, RepoQueue cooldown, int time, int cooldownTicks)
    {
        if (heap.TryPop(out var remaining))
        {
            remaining--;
            if (remaining > 0)
            {
                cooldown.Enqueue((remaining, time + cooldownTicks));
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
    public static int LeastIntervalByArrayScan(char[] tasks, int cooldownTicks)
    {
        var counts = BuildFrequencyCounts(tasks);

        return SimulateTickByTick(counts, tasks.Length, cooldownTicks);
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

    private static int SimulateTickByTick(int[] counts, int taskCount, int cooldownTicks)
    {
        var lastUsed = new int[EnglishAlphabetSize];
        Array.Fill(lastUsed, NeverUsedSentinel);

        var remaining = taskCount;
        var time = 0;

        while (remaining > 0)
        {
            var best = FindBestAvailableTask(counts, lastUsed, time, cooldownTicks);

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

    private static int FindBestAvailableTask(int[] counts, int[] lastUsed, int time, int cooldownTicks)
    {
        var best = -1;

        for (var candidateIndex = 0; candidateIndex < EnglishAlphabetSize; candidateIndex++)
        {
            if (IsReadyToRun(counts[candidateIndex], time - lastUsed[candidateIndex], cooldownTicks)
                && IsBetterCandidate(counts, candidateIndex, best))
            {
                best = candidateIndex;
            }
        }

        return best;
    }

    // A task is ready to run when it still has occurrences left and enough ticks
    // have passed since it last ran.
    private static bool IsReadyToRun(int remainingCount, int cooldownAge, int cooldownTicks)
        => remainingCount > 0 && cooldownAge > cooldownTicks;

    // Among the ready tasks the one with the most occurrences left wins, and the
    // first ready task wins while none has been chosen yet.
    private static bool IsBetterCandidate(int[] counts, int candidateIndex, int best)
        => best == -1 || counts[candidateIndex] > counts[best];
}
