using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindMinimumTimeToFinishAllJobs;

// LeetCode 1723. Find Minimum Time to Finish All Jobs: BinarySearch.LowerBound
// anchors the smallest feasible per-worker time limit over a monotone virtual
// sequence - the same "candidate answer" binary-search shape ClosestDivisorsTests/
// TheKthFactorOfNTests already use, just with a feasibility predicate instead of a
// square comparison - and that predicate reuses PartitionToKEqualSumSubsetsTests'
// exact k-bucket Backtrack.TrySearch shape, relaxed from "== target" to "<= limit"
// since workers here need not all land on the same sum.
public sealed partial class FindMinimumTimeToFinishAllJobsTests
{
    [Fact]
    public void MinimumTime_LeetCodeExampleOne_ReturnsThree()
    {
        var actual = FindMinimumTime([3, 2, 3], k: 3);
        Assert.Equal(3, actual);
    }

    [Fact]
    public void MinimumTime_LeetCodeExampleTwo_ReturnsEleven()
    {
        var actual = FindMinimumTime([1, 2, 4, 7, 8], k: 2);
        Assert.Equal(11, actual);
    }

    [Fact]
    public void MinimumTime_SingleWorkerTakesEverything_ReturnsSum()
    {
        var actual = FindMinimumTime([1, 2, 3, 4], k: 1);
        Assert.Equal(10, actual);
    }

    private static int FindMinimumTime(int[] jobs, int k)
    {
        var sorted = (int[])jobs.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);

        var maxJob = sorted[0];
        var total = sorted.Sum();

        var sequence = new FeasibleTimeSequence(sorted, k, maxJob, total - maxJob + 1);
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

    // Virtual sequence over candidate time limits [maxJob, total]: 0 while
    // infeasible, 1 from the first feasible limit onward - CanFinishWithin only
    // gets easier as the limit grows, so this is monotone and LowerBound(sequence, 1)
    // lands on the smallest feasible limit directly.
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
