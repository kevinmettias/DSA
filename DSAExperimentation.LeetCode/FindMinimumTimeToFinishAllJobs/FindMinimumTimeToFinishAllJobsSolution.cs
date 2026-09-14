using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindMinimumTimeToFinishAllJobs;

// LeetCode 1723. Find Minimum Time to Finish All Jobs: hand every job to one of k
// workers so that the busiest worker's total time is as small as possible.
//
// Both strategies answer the same question - the minimum achievable maximum load -
// and differ only in how they search for it: enumerate every assignment and keep
// the best, or binary-search the answer itself and ask a feasibility question at
// each candidate.
internal static class FindMinimumTimeToFinishAllJobsSolution
{
    // Candidate time limits run over [maxJob, total] inclusive, so the virtual
    // sequence is one longer than the difference between its endpoints.
    private const int InclusiveSpan = 1;

    // The textbook answer: try all k^n worker assignments depth-first, tracking the
    // smallest max-load seen. Deliberately written without this repo's primitives -
    // plain arrays and a hand-rolled recursion - because it is the arm the composed
    // strategy below has to justify itself against.
    public static int MinimumTimeByExhaustiveAssignment(int[] jobs, int workerCount)
    {
        var workload = new Workload(jobs, new int[workerCount]);

        return AssignJobs(workload, index: 0, best: int.MaxValue);
    }

    // This repo's own answer: BinarySearch.LowerBound anchors the smallest feasible
    // per-worker time limit over a monotone virtual sequence - the same "candidate
    // answer" binary-search shape ClosestDivisors/TheKthFactorOfN use, just with a
    // feasibility predicate instead of a square comparison - and that predicate
    // reuses PartitionToKEqualSumSubsets' exact k-bucket Backtrack.TrySearch shape,
    // relaxed from "== target" to "<= limit" since workers here need not all land on
    // the same sum. Binary search collapses a linear scan over every candidate limit
    // down to O(log(sum)) feasibility checks, each itself pruned far below k^n by
    // Backtrack's Choose/Unchoose.
    public static int MinimumTimeByFeasibilityBinarySearch(int[] jobs, int workerCount)
    {
        var sorted = (int[])jobs.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);

        var maxJob = sorted[0];
        var total = sorted.Sum();

        var sequence = new FeasibleTimeSequence(sorted, workerCount, maxJob, total - maxJob + InclusiveSpan);
        var offset = BinarySearch.LowerBound<int, FeasibleTimeSequence>(sequence, 1);

        return maxJob + offset;
    }

    // The feasibility question the binary search asks at each candidate limit: can
    // every job be placed with no worker exceeding maxTime?
    private static bool CanFinishWithin(int[] jobs, int workerCount, int maxTime)
    {
        var state = new WorkerState(jobs, maxTime, workerCount);

        return Backtrack.TrySearch<WorkerState, int>(state, new BacktrackingSteps<WorkerState, int>(
            IsSolution: s => s.IsComplete,
            Candidates: s => PlaceableWorkers(s, workerCount),
            Choose: (s, worker) => s.Place(worker),
            Unchoose: (s, worker) => s.Remove(worker),
            OnSolution: _ => true));
    }

    // The workers that could still take the next unplaced job without breaching the
    // limit - none once every job is placed, which is what halts the search.
    private static IEnumerable<int> PlaceableWorkers(WorkerState state, int workerCount)
    {
        if (state.IsComplete)
        {
            return [];
        }

        return Enumerable.Range(0, workerCount).Where(state.CanPlace);
    }

    // The exhaustive walk's recursive step: place job `index` on each worker in turn
    // and keep the best max load any completed assignment reaches.
    private static int AssignJobs(Workload workload, int index, int best)
    {
        if (index == workload.Jobs.Length)
        {
            return Math.Min(best, workload.Loads.Max());
        }

        for (var worker = 0; worker < workload.Loads.Length; worker++)
        {
            workload.Loads[worker] += workload.Jobs[index];
            best = AssignJobs(workload, index + 1, best);
            workload.Loads[worker] -= workload.Jobs[index];
        }

        return best;
    }

    // The jobs and the per-worker running totals one exhaustive search threads
    // through its whole recursion, bundled so AssignJobs stays within the parameter
    // budget.
    private readonly record struct Workload(int[] Jobs, int[] Loads);

    // Virtual sequence over candidate time limits [maxJob, total]: 0 while
    // infeasible, 1 from the first feasible limit onward - CanFinishWithin only gets
    // easier as the limit grows, so this is monotone and LowerBound(sequence, 1)
    // lands on the smallest feasible limit directly. Meaningless outside this
    // problem, so it stays here rather than in DataStructures/Sequence.
    private readonly struct FeasibleTimeSequence(int[] jobs, int workerCount, int maxJob, int length)
        : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int index) => CanFinishWithin(jobs, workerCount, maxJob + index) ? 1 : 0;
    }

    // One feasibility probe's mutable bucket loads, exposed as the Choose/Unchoose
    // pair Backtrack drives.
    private sealed class WorkerState(int[] jobs, int maxTime, int workerCount)
    {
        private readonly int[] _loads = new int[workerCount];

        public int Index { get; private set; }

        public bool IsComplete => Index == jobs.Length;

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
