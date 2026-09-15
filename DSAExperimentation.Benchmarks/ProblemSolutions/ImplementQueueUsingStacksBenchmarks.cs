using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ImplementQueueUsingStacks;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is ImplementQueueUsingStacksSolution's
// two-stack queue, the same class ImplementQueueUsingStacksTests proves
// correct against. [GlobalSetup] builds one fixed call script of interleaved
// push/pop/peek calls (never popping/peeking past what has actually been
// pushed), so script construction is not charged to the measured replay -
// the same "return the real answer, not a weaker proxy" shape
// LRUCacheBenchmarks/BinarySearchTreeIteratorBenchmarks already follow.
[MemoryDiagnoser]
public class ImplementQueueUsingStacksBenchmarks
{
    private const int Seed = 232;

    private List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int>> _script = new();

    [Params(200, 2_000)]
    public int OperationCount { get; set; }

    [GlobalSetup]
    public void Setup() => _script = BuildScript(OperationCount, new Random(Seed));

    private static List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int>> BuildScript(
        int operationCount, Random random)
    {
        var script = new List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int>>();
        var pending = 0;

        for (var i = 0; i < operationCount; i++)
        {
            pending = AppendNextOperation(script, pending, random, operationCount);
        }

        return script;
    }

    // One script entry, plus the pending-push count it leaves behind. roll == 0
    // always pushes; rolls 1/2 pop or peek, but only once something has actually
    // been pushed and not yet popped.
    private static int AppendNextOperation(
        List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int>> script,
        int pending, Random random, int operationCount)
    {
        var roll = pending > 0 ? random.Next(0, 3) : 0;
        var nextPending = pending;

        if (roll == 0)
        {
            AppendPush(script, random, operationCount);
            nextPending++;
        }
        else if (roll == 1)
        {
            script.Add(queue => queue.Peek());
        }
        else
        {
            script.Add(queue => queue.Pop());
            nextPending--;
        }

        return nextPending;
    }

    // A push entry carrying the value it pushes, so the replay exercises real
    // values rather than one constant.
    private static void AppendPush(
        List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int>> script, Random random, int operationCount)
    {
        var value = random.Next(0, operationCount + 1);
        script.Add(queue =>
        {
            queue.Push(value);
            return 0;
        });
    }

    [Benchmark(Baseline = true)]
    public int TwoStackTransfer()
    {
        var queue = ImplementQueueUsingStacksSolution.CreateByTwoStacks();
        var sum = 0;

        foreach (var op in _script)
        {
            sum += op(queue);
        }

        return sum;
    }
}
