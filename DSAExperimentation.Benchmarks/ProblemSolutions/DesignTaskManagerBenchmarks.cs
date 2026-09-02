using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignTaskManager.DesignTaskManagerSolution;

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

    [Params(200, 2_000)]
    public int InitialTaskCount;

    private (int UserId, int TaskId, int Priority)[] _initialTasks = null!;
    private List<Func<ITaskManagerStrategy, int?>> _script = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _initialTasks = BuildInitialTasks(InitialTaskCount, random);
        _script = BuildScript(InitialTaskCount, random);
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => Replay(new TaskManagerByLinearScan(_initialTasks));

    [Benchmark]
    public long LazyDeletionHeap() => Replay(new TaskManagerByLazyDeletionHeap(_initialTasks));

    // Sums every executed userId rather than discarding it, so the JIT can't
    // eliminate the replay as dead code - the same "return the real answer, not a
    // weaker proxy" shape OpenTheLockBenchmarks/TwoSumBenchmarks already follow.
    private long Replay(ITaskManagerStrategy strategy)
    {
        var executedUserIdSum = 0L;

        foreach (var op in _script)
        {
            executedUserIdSum += op(strategy) ?? 0;
        }

        return executedUserIdSum;
    }

    private static (int UserId, int TaskId, int Priority)[] BuildInitialTasks(int count, Random random)
        => Enumerable.Range(0, count)
            .Select(taskId => (UserId: random.Next(0, count), TaskId: taskId, Priority: random.Next(0, PriorityUpperBound)))
            .ToArray();

    private static List<Func<ITaskManagerStrategy, int?>> BuildScript(int initialTaskCount, Random random)
    {
        var script = new List<Func<ITaskManagerStrategy, int?>>();
        var removeCount = initialTaskCount / 10;
        var editCount = initialTaskCount / 10;

        // Removed/edited first, before the round loop below ever adds or executes
        // anything - these taskIds are guaranteed still live.
        foreach (var taskId in Enumerable.Range(0, removeCount))
        {
            script.Add(strategy =>
            {
                strategy.Rmv(taskId);
                return null;
            });
        }

        foreach (var taskId in Enumerable.Range(removeCount, editCount))
        {
            var newPriority = random.Next(0, PriorityUpperBound);
            script.Add(strategy =>
            {
                strategy.Edit(taskId, newPriority);
                return null;
            });
        }

        AppendGrowthRounds(script, initialTaskCount, random);
        return script;
    }

    // Two fresh Add calls per ExecTop: the live task count only grows round over
    // round, so every ExecTop always has something to execute regardless of which
    // priorities the random draws produced.
    private static void AppendGrowthRounds(List<Func<ITaskManagerStrategy, int?>> script, int roundCount, Random random)
    {
        var nextTaskId = roundCount;

        for (var round = 0; round < roundCount; round++)
        {
            AppendAdd(script, random, roundCount, nextTaskId++);
            AppendAdd(script, random, roundCount, nextTaskId++);
            script.Add(strategy => strategy.ExecTop());
        }
    }

    private static void AppendAdd(List<Func<ITaskManagerStrategy, int?>> script, Random random, int userIdUpperBound, int taskId)
    {
        var userId = random.Next(0, userIdUpperBound);
        var priority = random.Next(0, PriorityUpperBound);
        script.Add(strategy =>
        {
            strategy.Add(userId, taskId, priority);
            return null;
        });
    }
}
