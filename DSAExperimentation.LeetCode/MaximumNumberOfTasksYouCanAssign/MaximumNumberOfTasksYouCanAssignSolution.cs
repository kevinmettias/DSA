using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.MaximumNumberOfTasksYouCanAssign;

// LeetCode 2071. Maximum Number of Tasks You Can Assign: how many tasks can be
// completed when each task needs a worker of at least its strength requirement, each
// worker takes at most one task, and a limited number of pills each add a fixed
// strength boost to one worker.
//
// Assigning k tasks is always done with the k easiest tasks and the k strongest
// workers, and "can k be assigned" is monotone - feasible for k implies feasible for
// every smaller k - so the answer is the last feasible k. Both strategies check one
// k the same way, hardest-task-first: admit every worker who could reach the task
// with a pill, let the strongest available worker take it unaided if it can, and
// spend a pill on the weakest available worker only when it must. They differ in how
// they search for k: the baseline walks k downwards one at a time, the composed
// strategy bisects it with this repo's own BinarySearch.LowerBound over an on-demand
// IRandomAccessSequence<bool>, the same search-on-the-answer shape
// KokoEatingBananasSolution uses.
internal static class MaximumNumberOfTasksYouCanAssignSolution
{
    // The textbook answer: try k = maxK, maxK - 1, ... and stop at the first k that
    // works, with a BCL LinkedList<int> standing in for the double-ended pool of
    // currently-assignable workers. Deliberately without this repo's primitives - it
    // is the arm the composed strategy below has to justify itself against.
    public static int MaxTaskAssignmentByLinearScan(int[] tasks, int[] workers, int pills, int strength)
    {
        var assignment = SortedTaskAssignment.From(tasks, workers, pills, strength);

        return MaxTaskAssignmentByLinearScan(assignment);
    }

    public static int MaxTaskAssignmentByLinearScan(SortedTaskAssignment assignment)
    {
        for (var taskCount = assignment.MaxAssignable; taskCount > 0; taskCount--)
        {
            if (CanAssignByLinkedListPool(taskCount, assignment))
            {
                return taskCount;
            }
        }

        return 0;
    }

    // This repo's own BinarySearch.LowerBound over the infeasibility sequence: the
    // candidate counts are never materialized, each probe just reruns the greedy
    // check, and the leftmost infeasible k sits one past the answer.
    public static int MaxTaskAssignmentBySequenceLowerBound(int[] tasks, int[] workers, int pills, int strength)
    {
        var assignment = SortedTaskAssignment.From(tasks, workers, pills, strength);

        return MaxTaskAssignmentBySequenceLowerBound(assignment);
    }

    public static int MaxTaskAssignmentBySequenceLowerBound(SortedTaskAssignment assignment)
    {
        var sequence = new InfeasibleAssignmentSequence(assignment);

        return BinarySearch.LowerBound<bool, InfeasibleAssignmentSequence>(sequence, true) - 1;
    }

    // The k easiest tasks handed out hardest first, which is what makes the greedy
    // choice safe: the hardest remaining task has the fewest workers who could take
    // it, so it is the one whose options must be spent first. The two pools below
    // answer the identical question, so each strategy runs its own copy of this
    // three-line walk rather than sharing one through a delegate the measured arms
    // would both pay for.
    private static bool CanAssignByLinkedListPool(int taskCount, SortedTaskAssignment assignment)
    {
        var pool = new LinkedListWorkerPool(assignment, taskCount);

        for (var index = taskCount - 1; index >= 0; index--)
        {
            if (!pool.TryAssign(assignment.TasksAscending[index]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool CanAssignByDequePool(int taskCount, SortedTaskAssignment assignment)
    {
        var pool = new DequeWorkerPool(assignment, taskCount);

        for (var index = taskCount - 1; index >= 0; index--)
        {
            if (!pool.TryAssign(assignment.TasksAscending[index]))
            {
                return false;
            }
        }

        return true;
    }

    // The prepared input both hoisted overloads take (ARCHITECTURE.md section 17.4):
    // every strategy needs the tasks and the workers ascending, so a benchmark can
    // sort once in [GlobalSetup] instead of paying for it inside the measured call.
    // A record struct, so it can never be confused with the raw-array overload.
    internal readonly record struct SortedTaskAssignment(
        int[] TasksAscending, int[] WorkersAscending, int Pills, int Strength)
    {
        // Only the k easiest tasks and the k strongest workers are ever paired, so
        // no k beyond this is worth asking about.
        public int MaxAssignable => Math.Min(TasksAscending.Length, WorkersAscending.Length);

        public static SortedTaskAssignment From(int[] tasks, int[] workers, int pills, int strength) =>
            new([.. tasks.Order()], [.. workers.Order()], pills, strength);

        // Where the k strongest workers begin in the ascending worker array.
        public int StrongestWorkerOffset(int taskCount) => WorkersAscending.Length - taskCount;
    }

    // Get(index) is "k = index cannot be assigned" - false up to the answer and true
    // from there on, the monotonicity BinarySearch.LowerBound assumes but never
    // checks. A witness for this problem alone: the feasibility rule is LC 2071's own
    // content, not a general monotone-predicate shape.
    private readonly struct InfeasibleAssignmentSequence(SortedTaskAssignment assignment)
        : IRandomAccessSequence<bool>
    {
        public int Length => assignment.MaxAssignable + 1;

        public bool Get(int index) => !CanAssignByDequePool(index, assignment);
    }

    // The composed pool: this repo's own Deque<int>, kept sorted ascending because
    // the admission threshold only ever relaxes, so a newly-qualifying worker is
    // always weaker than everyone already in the pool and goes to the front. Each
    // worker is admitted at most once across a whole feasibility check.
    private sealed class DequeWorkerPool(SortedTaskAssignment assignment, int taskCount)
    {
        private readonly RepoDeque _available = new();
        private readonly int _offset = assignment.StrongestWorkerOffset(taskCount);
        private int _unadmitted = taskCount;
        private int _remainingPills = assignment.Pills;

        public bool TryAssign(int task)
        {
            Admit(task);

            if (!_available.TryPeekBack(out var strongest))
            {
                return false;
            }

            if (strongest >= task)
            {
                _available.TryPopBack(out _);

                return true;
            }

            return TryTakeWeakestWithPill();
        }

        private void Admit(int task)
        {
            var workers = assignment.WorkersAscending;

            while (_unadmitted > 0 && workers[_offset + _unadmitted - 1] + (long)assignment.Strength >= task)
            {
                _unadmitted--;
                _available.PushFront(workers[_offset + _unadmitted]);
            }
        }

        // The task outruns every unaided worker, so it can only be taken by spending a
        // pill on the weakest worker still in the pool.
        private bool TryTakeWeakestWithPill()
        {
            if (_remainingPills == 0)
            {
                return false;
            }

            _available.TryPopFront(out _);
            _remainingPills--;

            return true;
        }
    }

    // The baseline's pool, doing the identical bookkeeping over a BCL
    // LinkedList<int> - AddFirst/RemoveFirst/RemoveLast are the double-ended
    // operations you reach for without this repo's Deque<int>.
    private sealed class LinkedListWorkerPool(SortedTaskAssignment assignment, int taskCount)
    {
        private readonly LinkedList<int> _available = new();
        private readonly int _offset = assignment.StrongestWorkerOffset(taskCount);
        private int _unadmitted = taskCount;
        private int _remainingPills = assignment.Pills;

        public bool TryAssign(int task)
        {
            Admit(task);

            if (_available.Last is not { } strongest)
            {
                return false;
            }

            if (strongest.Value >= task)
            {
                _available.RemoveLast();

                return true;
            }

            return TryTakeWeakestWithPill();
        }

        private void Admit(int task)
        {
            var workers = assignment.WorkersAscending;

            while (_unadmitted > 0 && workers[_offset + _unadmitted - 1] + (long)assignment.Strength >= task)
            {
                _unadmitted--;
                _available.AddFirst(workers[_offset + _unadmitted]);
            }
        }

        // The task outruns every unaided worker, so it can only be taken by spending a
        // pill on the weakest worker still in the pool.
        private bool TryTakeWeakestWithPill()
        {
            if (_remainingPills == 0)
            {
                return false;
            }

            _available.RemoveFirst();
            _remainingPills--;

            return true;
        }
    }
}
