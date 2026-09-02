using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Minimum Time to Finish All Jobs (LC 1723): exhaustively trying every one of
// the K^JobCount worker assignments and tracking the best max-load seen vs. this
// repo's own BinarySearch.LowerBound anchoring the smallest feasible per-worker time
// limit over a monotone virtual sequence, each candidate limit checked by
// Backtrack.TrySearch's k-bucket feasibility search (PartitionToKEqualSumSubsets'
// exact shape, "==" relaxed to "<="). Binary search collapses what would otherwise
// be a linear scan over every candidate time limit down to O(log(sum)) feasibility
// checks, each itself pruned far below K^JobCount by Backtrack's Choose/Unchoose.
[MemoryDiagnoser]
public class FindMinimumTimeToFinishAllJobsBenchmarks
{
    private const int WorkerCount = 3;
    private const int RandomSeed = 5;
    private const int MaxJobDuration = 50;

    [Params(8, 10)]
    public int JobCount;

    private int[] _jobs = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _jobs = Enumerable.Range(0, JobCount).Select(_ => random.Next(1, MaxJobDuration)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ExhaustiveAssignment()
    {
        var loads = new int[WorkerCount];
        return AssignJobs(index: 0, loads, best: int.MaxValue);
    }

    private int AssignJobs(int index, int[] loads, int best)
    {
        if (index == _jobs.Length)
        {
            return Math.Min(best, loads.Max());
        }

        for (var worker = 0; worker < WorkerCount; worker++)
        {
            loads[worker] += _jobs[index];
            best = AssignJobs(index + 1, loads, best);
            loads[worker] -= _jobs[index];
        }

        return best;
    }

    [Benchmark]
    public int BinarySearchWithBacktracking()
    {
        var sorted = (int[])_jobs.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);

        var maxJob = sorted[0];
        var total = sorted.Sum();

        var sequence = new FeasibleTimeSequence(sorted, WorkerCount, maxJob, total - maxJob + 1);
        var offset = BinarySearch.LowerBound<int, FeasibleTimeSequence>(sequence, 1);

        return maxJob + offset;
    }

    private static bool CanFinishWithin(int[] jobs, int k, int maxTime)
    {
        var state = new WorkerState(jobs, maxTime, k);

        return Backtrack.TrySearch<WorkerState, int>(state, new BacktrackingSteps<WorkerState, int>(
            IsSolution: s => s.Index == jobs.Length,
            Candidates: s => s.Index == jobs.Length ? [] : Enumerable.Range(0, k).Where(s.CanPlace),
            Choose: (s, worker) => s.Place(worker),
            Unchoose: (s, worker) => s.Remove(worker),
            OnSolution: _ => true));
    }

    private readonly struct FeasibleTimeSequence(int[] jobs, int k, int maxJob, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int value) => CanFinishWithin(jobs, k, maxJob + value) ? 1 : 0;
    }

    private sealed class WorkerState(int[] jobs, int maxTime, int k)
    {
        private readonly int[] _loads = new int[k];

        public int Index { get; private set; }

        public bool CanPlace(int worker) => _loads[worker] + jobs[Index] <= maxTime;

        public void Place(int worker)
        {
            _loads[worker] += jobs[Index];
            Index++;
        }

        public void Remove(int worker)
        {
            Index--;
            _loads[worker] -= jobs[Index];
        }
    }
}
