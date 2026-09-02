using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Task Scheduler II (LC 2365): the naive approach re-scans backward through every
// prior task on each step looking for the same task type (O(n) per task, O(n^2)
// overall); the repo-primitive approach tracks each type's last-used day in a
// HashMap<int,long> (O(1) average per task). Tasks are all distinct, so the naive
// backward scan always runs to the very start without finding a match - the same
// "force the real worst case" trick TwoSumBenchmarks' unreachable target uses.
[MemoryDiagnoser]
public class TaskSchedulerIIBenchmarks
{
    private const int Space = 1;

    [Params(200, 5_000)]
    public int Length;

    private int[] _tasks = null!;

    [GlobalSetup]
    public void Setup() => _tasks = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public long BruteForce()
    {
        var dayOfIndex = new long[_tasks.Length];
        long currentDay = 0;

        for (var i = 0; i < _tasks.Length; i++)
        {
            var previousDay = FindPreviousDay(i, dayOfIndex);

            currentDay = previousDay >= 0 && currentDay - previousDay <= Space
                ? previousDay + Space + 1
                : currentDay + 1;

            dayOfIndex[i] = currentDay;
        }

        return currentDay;
    }

    private long FindPreviousDay(int index, long[] dayOfIndex)
    {
        for (var j = index - 1; j >= 0; j--)
        {
            if (_tasks[j] == _tasks[index])
            {
                return dayOfIndex[j];
            }
        }

        return -1;
    }

    [Benchmark]
    public long HashMapOnePass()
    {
        var lastDay = new HashMap<int, long>();
        long currentDay = 0;

        foreach (var task in _tasks)
        {
            if (lastDay.TryGetValue(task, out var previousDay) && currentDay - previousDay <= Space)
            {
                currentDay = previousDay + Space + 1;
            }
            else
            {
                currentDay++;
            }

            lastDay.Set(task, currentDay);
        }

        return currentDay;
    }
}
