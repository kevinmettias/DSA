using DSAExperimentation.LeetCode.DesignTaskManager;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignTaskManager;

// Harness only. Both strategies are DesignTaskManagerSolution's - this file replays
// LeetCode's published call sequence against each ITaskManagerStrategy
// implementation via a small operation script, so a failure still names the
// strategy that broke even though the "input" here is a sequence of mutating calls
// rather than a single argument tuple. TaskManagerOp.Apply is pure dispatch (which
// method to call with which arguments) - no priority/ordering logic of its own.
public sealed partial class DesignTaskManagerTests
{
    public static TheoryData<(int UserId, int TaskId, int Priority)[], TaskManagerOp[], int?[]> Examples =>
        new()
        {
            {
                [(1, 101, 10), (2, 102, 20), (3, 103, 15)],
                [
                    TaskManagerOp.Add(4, 104, 5),
                    TaskManagerOp.Edit(102, 8),
                    TaskManagerOp.ExecTop(),
                    TaskManagerOp.Rmv(101),
                    TaskManagerOp.Add(5, 105, 15),
                    TaskManagerOp.ExecTop(),
                ],
                [null, null, 3, null, null, 5]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TaskManagerByLinearScan_LeetCodeExample_ExecutesHighestPriorityTaskFirst(
        (int UserId, int TaskId, int Priority)[] initialTasks, TaskManagerOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(
            new DesignTaskManagerSolution.TaskManagerByLinearScan(initialTasks), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TaskManagerByLazyDeletionHeap_LeetCodeExample_ExecutesHighestPriorityTaskFirst(
        (int UserId, int TaskId, int Priority)[] initialTasks, TaskManagerOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(
            new DesignTaskManagerSolution.TaskManagerByLazyDeletionHeap(initialTasks), operations));

    private static int?[] RunScript(
        DesignTaskManagerSolution.ITaskManagerStrategy strategy, TaskManagerOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(strategy))];

    // One call in a TaskManager script: which method to invoke and with what arguments.
    // Pure dispatch, built via the named factories below so a script (like Examples
    // above) reads like the LeetCode call sequence it replays. Nested because it is
    // only ever used inside this test class and has no independent identity: it is
    // this harness's own vocabulary, not a type another file would import.
    public readonly record struct TaskManagerOp(TaskManagerOp.OpKind kind, int userId, int taskId, int value)
    {
        public static TaskManagerOp Add(int userId, int taskId, int priority) => new(OpKind.Add, userId, taskId, priority);

        public static TaskManagerOp Edit(int taskId, int newPriority) => new(OpKind.Edit, 0, taskId, newPriority);

        public static TaskManagerOp Rmv(int taskId) => new(OpKind.Rmv, 0, taskId, 0);

        public static TaskManagerOp ExecTop() => new(OpKind.ExecTop, 0, 0, 0);

        // null for the three void calls, the executed userId for ExecTop - so a script
        // runner can assert against one expected value per operation uniformly.
        // Internal, not public: ITaskManagerStrategy is internal to
        // DesignTaskManagerSolution, and only this same assembly's RunScript ever
        // calls Apply.
        internal int? Apply(DesignTaskManagerSolution.ITaskManagerStrategy strategy)
        {
            switch (kind)
            {
                case OpKind.Add:
                    strategy.Add(userId, taskId, value);
                    return null;
                case OpKind.Edit:
                    strategy.Edit(taskId, value);
                    return null;
                case OpKind.Rmv:
                    strategy.Rmv(taskId);
                    return null;
                default:
                    return strategy.ExecTop();
            }
        }

        public enum OpKind
        {
            Add,
            Edit,
            Rmv,
            ExecTop,
        }
    }
}
