using DSAExperimentation.LeetCode.ImplementQueueUsingStacks;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is ImplementQueueUsingStacksSolution's
// two-stack queue, the same class ImplementQueueUsingStacksSolutionTests proves
// correct against. [GlobalSetup] builds one fixed call script of interleaved
// push/pop/peek calls (never popping/peeking past what has actually been
// pushed), so script construction is not charged to the measured replay. The
// arm returns every value pop and peek answered, in order - the same "return the
// real answer, not a weaker proxy" shape LRUCacheBenchmarks/
// BinarySearchTreeIteratorBenchmarks already follow. A push answers nothing,
// which its script entry reports as null. OperationCount stops at LC 232's 100-call
// cap, and pushed values are drawn from its [1, 9].
public class ImplementQueueUsingStacksBenchmarks
{
    private const int Seed = 232;
    private const int MinPushedValue = 1;
    private const int MaxPushedValueExclusive = 10;

    private List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int?>> _script = new();

    // Every value pop and peek answer, in order. How many the random script holds is
    // not known up front, so this is a list given the most it can need in setup and
    // cleared by each replay.
    private List<int> _answers = [];

    [Params(10, 100)]
    public int OperationCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _script = BuildScript(OperationCount, new Random(Seed));
        _answers = new List<int>(OperationCount);
    }

    private static List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int?>> BuildScript(
        int operationCount, Random random)
    {
        var script = new List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int?>>();
        var pending = 0;

        for (var i = 0; i < operationCount; i++)
        {
            pending = AppendNextOperation(script, pending, random);
        }

        return script;
    }

    // One script entry, plus the pending-push count it leaves behind. roll == 0
    // always pushes; rolls 1/2 pop or peek, but only once something has actually
    // been pushed and not yet popped.
    private static int AppendNextOperation(
        List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int?>> script,
        int pending, Random random)
    {
        var roll = pending > 0 ? random.Next(0, 3) : 0;
        var nextPending = pending;

        if (roll == 0)
        {
            AppendPush(script, random);
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
        List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int?>> script, Random random)
    {
        var value = random.Next(MinPushedValue, MaxPushedValueExclusive);
        script.Add(queue =>
        {
            queue.Push(value);
            return null;
        });
    }

    [Benchmark(Baseline = true)]
    public List<int> TwoStackTransfer()
    {
        var queue = ImplementQueueUsingStacksSolution.CreateByTwoStacks();
        _answers.Clear();

        foreach (var op in _script)
        {
            if (op(queue) is { } answer)
            {
                _answers.Add(answer);
            }
        }

        return _answers;
    }
}
