using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TaskSchedulerII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TaskSchedulerIISolution's, the same methods
// TaskSchedulerIITests proves correct.
//
// Every task id is distinct, so the backward scan never finds a match and always runs
// all the way to the start - the same "force the real worst case" trick
// TwoSumBenchmarks' unreachable target uses - while the HashMap arm still pays one
// lookup and one write per task.
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
    public long BruteForce() => TaskSchedulerIISolution.CountDaysByBackwardScan(_tasks, Space);

    [Benchmark]
    public long HashMapOnePass() => TaskSchedulerIISolution.CountDaysByHashMapOnePass(_tasks, Space);
}
