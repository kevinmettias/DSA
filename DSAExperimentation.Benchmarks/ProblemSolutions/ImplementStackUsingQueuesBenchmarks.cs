using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ImplementStackUsingQueues;

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

    private List<Func<ImplementStackUsingQueuesSolution.IStackOperations, int>> _script = new();

    [Params(200, 2_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup() => _script = BuildScript(Count, new Random(Seed));

    // Count pushes fill the stack from empty, then every round pops one
    // element and pushes a fresh one - net-zero size change, so the stack
    // never underflows and never needs to grow past Count.
    private static List<Func<ImplementStackUsingQueuesSolution.IStackOperations, int>> BuildScript(int count, Random random)
    {
        var script = new List<Func<ImplementStackUsingQueuesSolution.IStackOperations, int>>();

        for (var i = 0; i < count; i++)
        {
            var value = random.Next(0, count);
            AppendPush(script, value);
        }

        for (var round = 0; round < count; round++)
        {
            script.Add(stack => stack.Pop());
            var value = random.Next(0, count);
            AppendPush(script, value);
        }

        return script;
    }

    private static void AppendPush(List<Func<ImplementStackUsingQueuesSolution.IStackOperations, int>> script, int value) =>
        script.Add(stack =>
        {
            stack.Push(value);
            return 0;
        });

    [Benchmark(Baseline = true)]
    public long BuiltInQueue() => Replay(ImplementStackUsingQueuesSolution.CreateByBuiltInQueue());

    [Benchmark]
    public long QueuePrimitive() => Replay(ImplementStackUsingQueuesSolution.CreateByQueuePrimitive());

    // Sums every returned value rather than discarding it, so the JIT can't
    // eliminate the replay as dead code - the same "return the real answer,
    // not a weaker proxy" shape OpenTheLockBenchmarks/LRUCacheBenchmarks
    // already follow.
    private long Replay(ImplementStackUsingQueuesSolution.IStackOperations stack)
    {
        var sum = 0L;

        foreach (var op in _script)
        {
            sum += op(stack);
        }

        return sum;
    }
}
