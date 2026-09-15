using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignTaskManager;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignTaskManagerSolution's, the same classes
// DesignTaskManagerTests proves correct. [GlobalSetup] builds one fixed, valid call
// script - a handful of edits/removals against initial taskIds nothing else has
// touched yet, then repeated add/add/execTop rounds so the system only grows and
// every execTop always has a target - so script construction, including tracking
// which taskIds are still safe to reference, is charged to setup rather than to the
// replay each [Benchmark] arm measures.
[MemoryDiagnoser]
public class DesignTaskManagerBenchmarks
{
    private const int Seed = 3408;
    private const int PriorityUpperBound = 1_000_000_000;

    private (int UserId, int TaskId, int Priority)[] _initialTasks = [];

    private List<Func<DesignTaskManagerSolution.ITaskManagerStrategy, int?>> _script = new();
    [Params(200, 2_000)]
    public int InitialTaskCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _initialTasks = BuildInitialTasks(InitialTaskCount, random);
        _script = BuildScript(InitialTaskCount, random);
    }

    private static (int UserId, int TaskId, int Priority)[] BuildInitialTasks(int count, Random random)
        => Enumerable.Range(0, count)
            .Select(taskId => (UserId: random.Next(0, count), TaskId: taskId, Priority: random.Next(0, PriorityUpperBound)))
            .ToArray();

    private static List<Func<DesignTaskManagerSolution.ITaskManagerStrategy, int?>> BuildScript(int initialTaskCount, Random random)
    {
        var script = new List<Func<DesignTaskManagerSolution.ITaskManagerStrategy, int?>>();
        var removeCount = initialTaskCount / 10;
        var editCount = initialTaskCount / 10;

        AppendRemovals(script, removeCount);
        AppendEdits(script, removeCount, editCount, random);
        AppendGrowthRounds(script, initialTaskCount, random);

        return script;
    }

    // Removed first, before the growth rounds below ever add or execute anything -
    // every taskId in this range is guaranteed still live.
    private static void AppendRemovals(List<Func<DesignTaskManagerSolution.ITaskManagerStrategy, int?>> script, int removeCount)
    {
        foreach (var taskId in Enumerable.Range(0, removeCount))
        {
            script.Add(strategy =>
            {
                strategy.Rmv(taskId);
                return null;
            });
        }
    }

    // Edited next, for the same reason: this range starts where the removals stopped,
    // so it is still untouched by anything the round loop later appends.
    private static void AppendEdits(
        List<Func<DesignTaskManagerSolution.ITaskManagerStrategy, int?>> script, int editStart, int editCount, Random random)
    {
        foreach (var taskId in Enumerable.Range(editStart, editCount))
        {
            var newPriority = random.Next(0, PriorityUpperBound);
            script.Add(strategy =>
            {
                strategy.Edit(taskId, newPriority);
                return null;
            });
        }
    }

    // Two fresh Add calls per ExecTop: the live task count only grows round over
    // round, so every ExecTop always has something to execute regardless of which
    // priorities the random draws produced.
    private static void AppendGrowthRounds(List<Func<DesignTaskManagerSolution.ITaskManagerStrategy, int?>> script, int roundCount, Random random)
    {
        var nextTaskId = roundCount;

        for (var round = 0; round < roundCount; round++)
        {
            AppendAdd(script, random, roundCount, nextTaskId++);
            AppendAdd(script, random, roundCount, nextTaskId++);
            script.Add(strategy => strategy.ExecTop());
        }
    }

    private static void AppendAdd(List<Func<DesignTaskManagerSolution.ITaskManagerStrategy, int?>> script, Random random, int userIdUpperBound, int taskId)
    {
        var userId = random.Next(0, userIdUpperBound);
        var priority = random.Next(0, PriorityUpperBound);
        script.Add(strategy =>
        {
            strategy.Add(userId, taskId, priority);
            return null;
        });
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => Replay(new DesignTaskManagerSolution.TaskManagerByLinearScan(_initialTasks));

    [Benchmark]
    public long LazyDeletionHeap() => Replay(new DesignTaskManagerSolution.TaskManagerByLazyDeletionHeap(_initialTasks));

    // Sums every executed userId rather than discarding it, so the JIT can't
    // eliminate the replay as dead code - the same "return the real answer, not a
    // weaker proxy" shape OpenTheLockBenchmarks/TwoSumBenchmarks already follow.
    private long Replay(DesignTaskManagerSolution.ITaskManagerStrategy strategy)
    {
        var executedUserIdSum = 0L;

        foreach (var op in _script)
        {
            executedUserIdSum += op(strategy) ?? 0;
        }

        return executedUserIdSum;
    }
}
