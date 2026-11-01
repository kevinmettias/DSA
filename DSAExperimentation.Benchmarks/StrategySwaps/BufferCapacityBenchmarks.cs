using DSAExperimentation.DataStructures.CircularBuffer;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// One churn through the general buffer, configured unbounded and bounded (ARCHITECTURE 20). Each step
// pushes at both ends, pops the back and peeks the front, so the buffer gains one element a step,
// wrapping its head backwards and growing as it goes. The bound sits above anything the churn holds,
// so it never refuses: same work, same answer, and the ratio is the bound's check on each push - a
// compare against the capacity where UnboundedCapacity's constant answer folds away - and nothing
// else.
public class BufferCapacityBenchmarks
{
    // The churn holds OperationCount + 1 elements at its fullest, between a step's pushes and its pop.
    private const int Headroom = 1;

    [Params(1_000, 100_000)]
    public int OperationCount { get; set; }

    [Benchmark(Baseline = true)]
    public long Unbounded() => Churn(new CircularBuffer<int, UnboundedCapacity>(default));

    [Benchmark]
    public long Bounded() =>
        Churn(new CircularBuffer<int, BoundedCapacity>(new BoundedCapacity(OperationCount + Headroom)));

    // Step i pops back the i it pushed there and peeks the i it pushed at the front.
    private long Churn<TCapacity>(CircularBuffer<int, TCapacity> buffer)
        where TCapacity : struct, IBufferCapacity
    {
        long sum = 0;

        for (var i = 0; i < OperationCount; i++)
        {
            buffer.TryPushBack(i);
            buffer.TryPushFront(i);

            if (buffer.TryPopBack(out var back))
            {
                sum += back;
            }

            if (buffer.TryPeekFront(out var front))
            {
                sum += front;
            }
        }

        return sum;
    }
}
