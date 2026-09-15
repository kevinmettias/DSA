using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindMinimumTimeToFinishAllJobs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMinimumTimeToFinishAllJobsSolution's, the same
// methods FindMinimumTimeToFinishAllJobsTests proves correct. Exhaustively trying
// every one of the WorkerCount^JobCount assignments and tracking the best max load
// vs. binary-searching the answer itself, each candidate time limit checked by a
// pruned k-bucket feasibility search. Job generation is charged to [GlobalSetup];
// the measured methods take LeetCode's own input shape, so no hoisted overload is
// needed.
[MemoryDiagnoser]
public class FindMinimumTimeToFinishAllJobsBenchmarks
{
    private const int WorkerCount = 3;
    private const int RandomSeed = 5;
    private const int MaxJobDuration = 50;

    private int[] _jobs = [];

    [Params(8, 10)]
    public int JobCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _jobs = Enumerable.Range(0, JobCount).Select(_ => random.Next(1, MaxJobDuration)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ExhaustiveAssignment() =>
        FindMinimumTimeToFinishAllJobsSolution.MinimumTimeByExhaustiveAssignment(_jobs, WorkerCount);

    [Benchmark]
    public int BinarySearchWithBacktracking() =>
        FindMinimumTimeToFinishAllJobsSolution.MinimumTimeByFeasibilityBinarySearch(_jobs, WorkerCount);
}
