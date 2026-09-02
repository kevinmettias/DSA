using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Number of Tasks You Can Assign (LC 2071): a linear "try k = maxK,
// maxK-1, ..." scan vs. this repo's own BinarySearch.LowerBound over an on-demand
// IRandomAccessSequence<bool> feasibility sequence (MaximumNumberOfTasksYouCanAssignTests
// precedent, same shape as KokoEatingBananasBenchmarks) - both call the identical
// Deque<int>-backed hardest-task-first greedy feasibility check, just O(maxK) times
// for the linear scan vs. O(log maxK) times for the binary search. Tasks are drawn
// from a strength range far above what any worker can ever reach even with every
// pill, so the true answer is always 0 - the worst case for the linear scan, which
// must walk all the way down from maxK before stopping.
[MemoryDiagnoser]
public class MaximumNumberOfTasksYouCanAssignBenchmarks
{
    private const int RandomSeed = 2071; // LC problem number
    private const int MinWorkerStrength = 1;
    private const int MaxWorkerStrengthExclusive = 100;
    private const int MinTaskRequirement = 500_000;
    private const int MaxTaskRequirementExclusive = 1_000_000;
    private const int Pills = 5;
    private const int Strength = 100;

    [Params(2_000, 50_000)]
    public int Length;

    private int[] _sortedTasks = null!;
    private int[] _sortedWorkers = null!;
    private int _maxK;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _sortedTasks = Enumerable.Range(0, Length)
            .Select(_ => random.Next(MinTaskRequirement, MaxTaskRequirementExclusive))
            .OrderBy(t => t)
            .ToArray();
        _sortedWorkers = Enumerable.Range(0, Length)
            .Select(_ => random.Next(MinWorkerStrength, MaxWorkerStrengthExclusive))
            .OrderBy(w => w)
            .ToArray();
        _maxK = Length;
    }

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        var boost = new WorkerBoost(Pills, Strength);

        for (var k = _maxK; k >= 0; k--)
        {
            if (CanAssign(k, _sortedTasks, _sortedWorkers, boost))
            {
                return k;
            }
        }

        return 0;
    }

    [Benchmark]
    public int SequenceLowerBound()
    {
        var sequence = new InfeasibleSequence(_sortedTasks, _sortedWorkers, new WorkerBoost(Pills, Strength), _maxK);
        return BinarySearch.LowerBound(sequence, true) - 1;
    }

    private static bool CanAssign(int k, int[] tasksAscending, int[] workersAscending, WorkerBoost boost)
    {
        var pool = new WorkerPool(workersAscending, workersAscending.Length - k, boost.Strength);
        var state = new AssignmentState { Low = k, RemainingPills = boost.Pills };

        for (var i = k - 1; i >= 0; i--)
        {
            if (!TryAssignTask(tasksAscending[i], pool, state))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryAssignTask(int task, WorkerPool pool, AssignmentState state)
    {
        while (state.Low > 0 && pool.Workers[pool.Offset + state.Low - 1] + (long)pool.Strength >= task)
        {
            state.Low--;
            state.Available.PushFront(pool.Workers[pool.Offset + state.Low]);
        }

        if (!state.Available.TryPeekBack(out var strongest))
        {
            return false;
        }

        if (strongest >= task)
        {
            state.Available.TryPopBack(out _);
            return true;
        }

        if (state.RemainingPills > 0)
        {
            state.Available.TryPopFront(out _);
            state.RemainingPills--;
            return true;
        }

        return false;
    }

    private readonly record struct WorkerBoost(int Pills, int Strength);

    private readonly record struct WorkerPool(int[] Workers, int Offset, int Strength);

    private sealed class AssignmentState
    {
        public RepoDeque Available { get; } = new();

        public int Low { get; set; }

        public int RemainingPills { get; set; }
    }

    private readonly struct InfeasibleSequence(int[] tasks, int[] workers, WorkerBoost boost, int maxK)
        : IRandomAccessSequence<bool>
    {
        public int Length => maxK + 1;

        public bool Get(int index) => !CanAssign(index, tasks, workers, boost);
    }
}
