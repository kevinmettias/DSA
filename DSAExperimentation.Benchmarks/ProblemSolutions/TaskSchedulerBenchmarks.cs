using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TaskScheduler;

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

    private char[] _tasks = [];

    [Params(2_000, 40_000)]
    public int TaskCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _tasks = Enumerable.Range(0, TaskCount)
            .Select(_ => (char)('A' + random.Next(AlphabetSize)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ArrayScan() => TaskSchedulerSolution.LeastIntervalByArrayScan(_tasks, Cooldown);

    [Benchmark]
    public int CooldownHeap() => TaskSchedulerSolution.LeastIntervalByCooldownHeap(_tasks, Cooldown);
}
