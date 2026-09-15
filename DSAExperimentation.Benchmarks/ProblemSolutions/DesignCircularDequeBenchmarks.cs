using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignCircularDeque;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignCircularDequeSolution's, the same classes
// DesignCircularDequeTests proves correct. Each [Benchmark] churns an insert-last/
// insert-front/delete-front/delete-last cycle that settles into a steady state at
// (near) capacity, forcing every operation through the wraparound path on both
// ends - the same shape DesignCircularQueueBenchmarks already uses for its own
// instance-API problem, extended to both ends since this problem allows both.
[MemoryDiagnoser]
public class DesignCircularDequeBenchmarks
{
    private const int OperationCount = 50_000;

    [Params(8, 512)]
    public int Capacity { get; set; }

    [Benchmark(Baseline = true)]
    public long ArrayBacked() => RunChurnCycle(new DesignCircularDequeSolution.CircularDequeByArrayBacked(Capacity));

    [Benchmark]
    public long DequeBacked() => RunChurnCycle(new DesignCircularDequeSolution.CircularDequeByDequeBacked(Capacity));

    // Sums every returned value rather than discarding it, so the JIT can't
    // eliminate the churn as dead code - the same "return the real answer, not a
    // weaker proxy" shape OpenTheLockBenchmarks/DesignCircularQueueBenchmarks
    // already follow.
    private static long RunChurnCycle(DesignCircularDequeSolution.ICircularDeque deque)
    {
        var sum = 0L;

        for (var i = 0; i < OperationCount; i++)
        {
            if (deque.IsFull())
            {
                sum += deque.DeleteFront() ? 1 : 0;
            }

            sum += deque.InsertLast(i) ? 1 : 0;

            if (deque.IsFull())
            {
                sum += deque.DeleteLast() ? 1 : 0;
            }

            sum += deque.InsertFront(i) ? 1 : 0;
            sum += deque.GetFront() + deque.GetRear();
        }

        return sum;
    }
}
