using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.ImplementStackUsingQueues.ImplementStackUsingQueuesSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ImplementStackUsingQueuesSolution's, the same
// factories ImplementStackUsingQueuesTests proves correct. [GlobalSetup]
// builds one fixed call script - Count pushes filling the stack, then Count
// rounds of alternating pop/push - so every run replays identical LIFO
// traffic and only the queue backing the rotation differs between arms, the
// same "script construction charged to setup, replay is what gets measured"
// shape LRUCacheBenchmarks/DesignTaskManagerBenchmarks already use for their
// own instance-API problems.
[MemoryDiagnoser]
public class ImplementStackUsingQueuesBenchmarks
{
    private const int Seed = 225;

    [Params(200, 2_000)]
    public int Count;

    private List<Func<IStackOperations, int>> _script = null!;

    [GlobalSetup]
    public void Setup() => _script = BuildScript(Count, new Random(Seed));

    [Benchmark(Baseline = true)]
    public long BuiltInQueue() => Replay(CreateByBuiltInQueue());

    [Benchmark]
    public long QueuePrimitive() => Replay(CreateByQueuePrimitive());

    // Sums every returned value rather than discarding it, so the JIT can't
    // eliminate the replay as dead code - the same "return the real answer,
    // not a weaker proxy" shape OpenTheLockBenchmarks/LRUCacheBenchmarks
    // already follow.
    private long Replay(IStackOperations stack)
    {
        var sum = 0L;

        foreach (var op in _script)
        {
            sum += op(stack);
        }

        return sum;
    }

    // Count pushes fill the stack from empty, then every round pops one
    // element and pushes a fresh one - net-zero size change, so the stack
    // never underflows and never needs to grow past Count.
    private static List<Func<IStackOperations, int>> BuildScript(int count, Random random)
    {
        var script = new List<Func<IStackOperations, int>>();

        for (var i = 0; i < count; i++)
        {
            AppendPush(script, random.Next(0, count));
        }

        for (var round = 0; round < count; round++)
        {
            script.Add(stack => stack.Pop());
            AppendPush(script, random.Next(0, count));
        }

        return script;
    }

    private static void AppendPush(List<Func<IStackOperations, int>> script, int value) =>
        script.Add(stack =>
        {
            stack.Push(value);
            return 0;
        });
}
