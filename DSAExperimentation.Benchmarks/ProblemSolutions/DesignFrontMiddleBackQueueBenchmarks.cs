using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignFrontMiddleBackQueue;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignFrontMiddleBackQueueSolution's, the same
// classes DesignFrontMiddleBackQueueTests proves correct. The cycle pushes at all
// three positions in turn, so the baseline's List<int> pays an O(n) shift on two
// of every three calls while the two-deque split pays O(1) at each - the same
// DesignLinkedListBenchmarks precedent (array-shift baseline vs. O(1)
// primitive-based insert), just with a third position.
[MemoryDiagnoser]
public class DesignFrontMiddleBackQueueBenchmarks
{
    private const int OperationCycleLength = 3; // cycles push front/middle/back

    [Params(5_000, 50_000)]
    public int Calls { get; set; }

    [Benchmark(Baseline = true)]
    public int ArrayListInsertAtPosition() => RunPushCycle(new DesignFrontMiddleBackQueueSolution.FrontMiddleBackQueueByListInsert());

    [Benchmark]
    public int TwoDequeFrontMiddleBackQueue() => RunPushCycle(new DesignFrontMiddleBackQueueSolution.FrontMiddleBackQueueByTwoDeques());

    // Returns the queue's own Count, the same non-dead value both of this
    // problem's original arms returned, so the pushes cannot be optimized away.
    private int RunPushCycle(DesignFrontMiddleBackQueueSolution.IFrontMiddleBackQueue queue)
    {
        for (var i = 0; i < Calls; i++)
        {
            switch (i % OperationCycleLength)
            {
                case 0: queue.PushFront(i); break;
                case 1: queue.PushMiddle(i); break;
                default: queue.PushBack(i); break;
            }
        }

        return queue.Count;
    }
}
