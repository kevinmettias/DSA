using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Remaining, int AvailableAt)>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TaskScheduler;

// LeetCode 621. Task Scheduler: a HashMap<char,int> counts each task's remaining
// occurrences, a max-heap (this repo's own Heap<int,MaxHeapOrder<int>>) always
// offers up the most-frequent remaining task, and a Queue<(int,int)> holds a task
// that just ran until its cooldown expires before it's pushed back onto the heap -
// one CPU tick per loop iteration instead of the O(n^2) "rebuild frequency table and
// rescan every tick" brute force.
public sealed partial class TaskSchedulerTests
{
    [Fact]
    public void LeastInterval_ClassicExample_ReturnsEightTicks()
    {
        char[] tasks = ['A', 'A', 'A', 'B', 'B', 'B'];

        var ticks = LeastInterval(tasks, n: 2);
        Assert.Equal(8, ticks);
    }

    [Fact]
    public void LeastInterval_NoCooldown_ReturnsTaskCount()
    {
        char[] tasks = ['A', 'A', 'A', 'B', 'B', 'B'];

        var ticks = LeastInterval(tasks, n: 0);
        Assert.Equal(6, ticks);
    }

    [Fact]
    public void LeastInterval_ManyDistinctTasksFillTheCooldown_ReturnsTaskCount()
    {
        char[] tasks = ['A', 'A', 'A', 'B', 'B', 'B', 'C', 'C', 'C', 'D', 'D', 'E'];

        var ticks = LeastInterval(tasks, n: 2);
        Assert.Equal(12, ticks);
    }

    private static int LeastInterval(char[] tasks, int n)
    {
        var counts = BuildTaskCounts(tasks);
        var heap = BuildFrequencyHeap(counts);

        return RunSimulation(heap, n);
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

    private static int RunSimulation(Heap<int, MaxHeapOrder<int>> heap, int n)
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
}
