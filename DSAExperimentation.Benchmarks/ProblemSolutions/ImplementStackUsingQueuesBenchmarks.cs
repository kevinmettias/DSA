using DSAExperimentation.LeetCode.ImplementStackUsingQueues;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ImplementStackUsingQueuesSolution's, the same
// factories ImplementStackUsingQueuesSolutionTests proves correct. [GlobalSetup]
// builds one fixed call script - Count pushes filling the stack, then Count
// rounds of alternating pop/push - so every run replays identical LIFO
// traffic and only the queue backing the rotation differs between arms, the
// same "script construction charged to setup, replay is what gets measured"
// shape LRUCacheBenchmarks/DesignTaskManagerBenchmarks already use for their
// own instance-API problems. The script makes 3 * Count calls, so Count stops at 33
// to stay inside LC 225's 100-call cap, and pushed values are drawn from its [1, 9].
public class ImplementStackUsingQueuesBenchmarks
{
    private const int Seed = 225;
    private const int MinPushedValue = 1;
    private const int MaxPushedValueExclusive = 10;

    private List<Func<ImplementStackUsingQueuesSolution.IStackOperations, int?>> _script = new();

    // Every value Pop answers, in order - one per round; sized in setup so the replay allocates nothing.
    private int[] _popped = [];

    [Params(3, 33)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _script = BuildScript(Count, new Random(Seed));
        _popped = new int[Count];
    }

    // Count pushes fill the stack from empty, then every round pops one
    // element and pushes a fresh one - net-zero size change, so the stack
    // never underflows and never needs to grow past Count.
    private static List<Func<ImplementStackUsingQueuesSolution.IStackOperations, int?>> BuildScript(int count, Random random)
    {
        var script = new List<Func<ImplementStackUsingQueuesSolution.IStackOperations, int?>>();

        for (var i = 0; i < count; i++)
        {
            var value = random.Next(MinPushedValue, MaxPushedValueExclusive);
            AppendPush(script, value);
        }

        for (var round = 0; round < count; round++)
        {
            script.Add(stack => stack.Pop());
            var value = random.Next(MinPushedValue, MaxPushedValueExclusive);
            AppendPush(script, value);
        }

        return script;
    }

    private static void AppendPush(List<Func<ImplementStackUsingQueuesSolution.IStackOperations, int?>> script, int value) =>
        script.Add(stack =>
        {
            stack.Push(value);
            return null;
        });

    [Benchmark(Baseline = true)]
    public int[] BuiltInQueue() => Replay(ImplementStackUsingQueuesSolution.CreateByBuiltInQueue());

    [Benchmark]
    public int[] QueuePrimitive() => Replay(ImplementStackUsingQueuesSolution.CreateByQueuePrimitive());

    // Returns every popped value, in order, rather than discarding it, so the JIT
    // can't eliminate the replay as dead code - the same "return the real answer,
    // not a weaker proxy" shape OpenTheLockBenchmarks/LRUCacheBenchmarks already
    // follow. A push answers nothing, which its script entry reports as null.
    private int[] Replay(ImplementStackUsingQueuesSolution.IStackOperations stack)
    {
        var next = 0;

        foreach (var op in _script)
        {
            if (op(stack) is { } popped)
            {
                _popped[next++] = popped;
            }
        }

        return _popped;
    }
}
