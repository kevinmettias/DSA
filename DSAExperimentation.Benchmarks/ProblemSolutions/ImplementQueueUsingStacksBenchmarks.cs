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

    [Params(200, 2_000)]
    public int OperationCount;

    private List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int>> _script = null!;

    [GlobalSetup]
    public void Setup() => _script = BuildScript(OperationCount, new Random(Seed));

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

    private static List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int>> BuildScript(
        int operationCount, Random random)
    {
        var script = new List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int>>();
        var pending = 0;

        for (var i = 0; i < operationCount; i++)
        {
            // roll == 0 always pushes; rolls 1/2 pop or peek, but only once
            // something has actually been pushed and not yet popped.
            var roll = pending > 0 ? random.Next(0, 3) : 0;

            if (roll == 0)
            {
                var value = random.Next(0, operationCount + 1);
                script.Add(queue =>
                {
                    queue.Push(value);
                    return 0;
                });
                pending++;
            }
            else if (roll == 1)
            {
                script.Add(queue => queue.Peek());
            }
            else
            {
                script.Add(queue => queue.Pop());
                pending--;
            }
        }

        return script;
    }
}
