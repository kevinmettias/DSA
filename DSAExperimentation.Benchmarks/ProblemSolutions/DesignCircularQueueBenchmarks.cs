using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignCircularQueue;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignCircularQueueSolution's, the same classes
// DesignCircularQueueTests proves correct. [GlobalSetup] builds one fixed,
// deterministic call script - enough EnQueues to fill the queue exactly, then
// repeated DeQueue/EnQueue/Rear rounds once it is at capacity, so every operation
// after the fill is forced through the wraparound path at both ends - the same
// "script construction charged to setup, replay is what gets measured" shape
// DesignTaskManagerBenchmarks already uses for its own instance-API problem.
[MemoryDiagnoser]
public class DesignCircularQueueBenchmarks
{
    private const int OperationCount = 50_000;

    private List<Func<DesignCircularQueueSolution.ICircularQueue, int>> _script = new();

    [Params(8, 512)]
    public int Capacity { get; set; }

    [GlobalSetup]
    public void Setup() => _script = BuildScript(Capacity, OperationCount);

    private static List<Func<DesignCircularQueueSolution.ICircularQueue, int>> BuildScript(int capacity, int operationCount)
    {
        var script = new List<Func<DesignCircularQueueSolution.ICircularQueue, int>>();

        for (var i = 0; i < operationCount; i++)
        {
            var value = i;

            // Once the queue has been filled to capacity, every further EnQueue
            // must first make room - churning both ends of the wraparound buffer
            // for the remainder of the script, exactly as the pre-migration
            // benchmark's IsFull()-guarded loop did.
            if (i >= capacity)
            {
                script.Add(queue => queue.DeQueue() ? 1 : 0);
            }

            script.Add(queue => queue.EnQueue(value) ? 1 : 0);
            script.Add(queue => queue.Rear());
        }

        return script;
    }

    [Benchmark(Baseline = true)]
    public long ArrayBacked() => Replay(new DesignCircularQueueSolution.CircularQueueByArrayBacked(Capacity));

    [Benchmark]
    public long DequeBacked() => Replay(new DesignCircularQueueSolution.CircularQueueByDequeBacked(Capacity));

    // Sums every returned value rather than discarding it, so the JIT can't
    // eliminate the replay as dead code - the same "return the real answer, not a
    // weaker proxy" shape OpenTheLockBenchmarks/DesignTaskManagerBenchmarks
    // already follow.
    private long Replay(DesignCircularQueueSolution.ICircularQueue queue)
    {
        var resultSum = 0L;

        foreach (var op in _script)
        {
            resultSum += op(queue);
        }

        return resultSum;
    }
}
