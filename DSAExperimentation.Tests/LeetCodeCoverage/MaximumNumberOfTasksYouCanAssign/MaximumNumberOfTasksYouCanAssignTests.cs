using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfTasksYouCanAssign;

// LeetCode 2071. Maximum Number of Tasks You Can Assign: "can the k easiest tasks
// be perfectly matched against the k strongest workers (each with at most one
// pill)" is monotone in k (feasible for every smaller k once it's feasible for
// some k), so the answer is found via this repo's own BinarySearch.LowerBound over
// an on-demand IRandomAccessSequence<bool> feasibility sequence - the same
// "binary search on the answer" shape KokoEatingBananasTests already uses. Each
// feasibility check is a hardest-task-first greedy sweep backed by this repo's
// Deque<int> (kept sorted ascending by always pushing newly-qualifying, weaker
// workers to the front as the task threshold relaxes): the strongest available
// worker takes a task unaided when it can, and a pill is spent on the weakest
// available worker only when it must be.
public sealed partial class MaximumNumberOfTasksYouCanAssignTests
{
    public static TheoryData<int[], int[], TaskAssignmentQuery, int> TasksWorkersAndQueryToExpectedCount => new()
    {
        { new[] { 3, 2, 1 }, new[] { 0, 3, 3 }, new TaskAssignmentQuery(1, 1), 3 },
        { new[] { 5, 4 }, new[] { 0, 0, 0 }, new TaskAssignmentQuery(1, 5), 1 },
    };

    [Theory]
    [MemberData(nameof(TasksWorkersAndQueryToExpectedCount))]
    public void MaxTaskAssignment_LeetCodeExamples_ReturnsExpectedCount(
        int[] tasks, int[] workers, TaskAssignmentQuery query, int expected)
    {
        var actual = MaxTaskAssignment(tasks, workers, query.Pills, query.Strength);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MaxTaskAssignment_NoPillsAndWorkersTooWeakForEveryTask_ReturnsZero()
    {
        var actual = MaxTaskAssignment([5, 5], [1, 1], pills: 0, strength: 3);
        Assert.Equal(0, actual);
    }

    private static int MaxTaskAssignment(int[] tasks, int[] workers, int pills, int strength)
    {
        var sortedTasks = tasks.OrderBy(t => t).ToArray();
        var sortedWorkers = workers.OrderBy(w => w).ToArray();
        var maxK = Math.Min(sortedTasks.Length, sortedWorkers.Length);

        var sequence = new InfeasibleSequence(sortedTasks, sortedWorkers, pills, strength, maxK);
        return BinarySearch.LowerBound(sequence, true) - 1;
    }

    // Hardest-task-first greedy feasibility check for assigning exactly the k
    // smallest tasks to the k strongest workers. `low` only ever decreases across
    // the k iterations (the pill-assist threshold task-strength only shrinks as
    // tasks get easier), so each worker enters the Deque at most once - O(k) total
    // across the whole call, not O(k) per iteration.
    private static bool CanAssign(int k, int[] tasksAscending, int[] workersAscending, TaskAssignmentQuery query)
    {
        var context = new AssignmentContext(workersAscending, workersAscending.Length - k, query.Strength);
        var available = new RepoDeque();
        var state = new AssignmentState(k, query.Pills);

        for (var i = k - 1; i >= 0; i--)
        {
            if (!TryAssignTask(tasksAscending[i], context, available, state))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryAssignTask(int task, AssignmentContext context, RepoDeque available, AssignmentState state)
    {
        AdmitQualifyingWorkers(task, context, available, state);

        if (!available.TryPeekBack(out var strongest))
        {
            return false;
        }

        return AssignStrongestOrSpendPill(task, strongest, available, state);
    }

    // Pushes every worker who now qualifies (with the pill assist, at this task's
    // threshold) to the front of the deque, weakest-qualifying-first.
    private static void AdmitQualifyingWorkers(int task, AssignmentContext context, RepoDeque available, AssignmentState state)
    {
        while (state.Low > 0 && context.WorkersAscending[context.WorkerOffset + state.Low - 1] + (long)context.Strength >= task)
        {
            state.Low--;
            available.PushFront(context.WorkersAscending[context.WorkerOffset + state.Low]);
        }
    }

    // Assigns the strongest available worker unaided when possible, falling back to
    // spending a pill on the weakest available worker.
    private static bool AssignStrongestOrSpendPill(int task, int strongest, RepoDeque available, AssignmentState state)
    {
        if (strongest >= task)
        {
            available.TryPopBack(out _);
            return true;
        }

        if (state.RemainingPills > 0)
        {
            available.TryPopFront(out _);
            state.RemainingPills--;
            return true;
        }

        return false;
    }

    public readonly record struct TaskAssignmentQuery(int Pills, int Strength);

    private readonly record struct AssignmentContext(int[] WorkersAscending, int WorkerOffset, int Strength);

    private sealed class AssignmentState(int low, int remainingPills)
    {
        public int Low { get; set; } = low;

        public int RemainingPills { get; set; } = remainingPills;
    }

    private readonly struct InfeasibleSequence(int[] tasks, int[] workers, int pills, int strength, int maxK)
        : IRandomAccessSequence<bool>
    {
        public int Length => maxK + 1;

        public bool Get(int index) => !CanAssign(index, tasks, workers, new TaskAssignmentQuery(pills, strength));
    }
}
