using static DSAExperimentation.LeetCode.DesignTaskManager.DesignTaskManagerSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignTaskManager;

// Harness only. Both strategies are DesignTaskManagerSolution's - this file replays
// LeetCode's published call sequence against each ITaskManagerStrategy
// implementation via a small operation script, so a failure still names the
// strategy that broke even though the "input" here is a sequence of mutating calls
// rather than a single argument tuple. TaskManagerOp.Apply is pure dispatch (which
// method to call with which arguments) - no priority/ordering logic of its own.
public sealed class DesignTaskManagerTests
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
        RunScript(new TaskManagerByLinearScan(initialTasks), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void TaskManagerByLazyDeletionHeap_LeetCodeExample_ExecutesHighestPriorityTaskFirst(
        (int UserId, int TaskId, int Priority)[] initialTasks, TaskManagerOp[] operations, int?[] expected) =>
        RunScript(new TaskManagerByLazyDeletionHeap(initialTasks), operations, expected);

    private static void RunScript(ITaskManagerStrategy strategy, TaskManagerOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(strategy));
        }
    }
}

// One call in a TaskManager script: which method to invoke and with what arguments.
// Pure dispatch, built via the named factories below so a script (like Examples
// above) reads like the LeetCode call sequence it replays.
public readonly record struct TaskManagerOp
{
    private readonly Kind _kind;
    private readonly int _userId;
    private readonly int _taskId;
    private readonly int _value;

    private TaskManagerOp(Kind kind, int userId, int taskId, int value)
    {
        _kind = kind;
        _userId = userId;
        _taskId = taskId;
        _value = value;
    }

    public static TaskManagerOp Add(int userId, int taskId, int priority) => new(Kind.Add, userId, taskId, priority);

    public static TaskManagerOp Edit(int taskId, int newPriority) => new(Kind.Edit, 0, taskId, newPriority);

    public static TaskManagerOp Rmv(int taskId) => new(Kind.Rmv, 0, taskId, 0);

    public static TaskManagerOp ExecTop() => new(Kind.ExecTop, 0, 0, 0);

    // null for the three void calls, the executed userId for ExecTop - so a script
    // runner can assert against one expected value per operation uniformly.
    // Internal, not public: ITaskManagerStrategy is internal to
    // DesignTaskManagerSolution, and only this same assembly's RunScript ever
    // calls Apply.
    internal int? Apply(ITaskManagerStrategy strategy)
    {
        switch (_kind)
        {
            case Kind.Add:
                strategy.Add(_userId, _taskId, _value);
                return null;
            case Kind.Edit:
                strategy.Edit(_taskId, _value);
                return null;
            case Kind.Rmv:
                strategy.Rmv(_taskId);
                return null;
            default:
                return strategy.ExecTop();
        }
    }

    private enum Kind
    {
        Add,
        Edit,
        Rmv,
        ExecTop,
    }
}
