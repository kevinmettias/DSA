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

        Assert.Equal(8, LeastInterval(tasks, n: 2));
    }

    [Fact]
    public void LeastInterval_NoCooldown_ReturnsTaskCount()
    {
        char[] tasks = ['A', 'A', 'A', 'B', 'B', 'B'];

        Assert.Equal(6, LeastInterval(tasks, n: 0));
    }

    [Fact]
    public void LeastInterval_ManyDistinctTasksFillTheCooldown_ReturnsTaskCount()
    {
        char[] tasks = ['A', 'A', 'A', 'B', 'B', 'B', 'C', 'C', 'C', 'D', 'D', 'E'];

        Assert.Equal(12, LeastInterval(tasks, n: 2));
    }

    private static int LeastInterval(char[] tasks, int n)
    {
        var counts = new HashMap<char, int>();

        foreach (var task in tasks)
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
                    cooldown.Enqueue((remaining, time + n));
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
