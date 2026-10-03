using DSAExperimentation.LeetCode.DesignFrontMiddleBackQueue;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignFrontMiddleBackQueueSolution's, the same
// classes DesignFrontMiddleBackQueueSolutionTests proves correct. The cycle pushes at all
// three positions in turn, so the baseline's List<int> pays an O(n) shift on two
// of every three calls while the two-deque split pays O(1) at each - the same
// DesignLinkedListBenchmarks precedent (array-shift baseline vs. O(1)
// primitive-based insert), just with a third position.
//
// The script then drains the queue from the back, and the popped values are the arm's
// answer: the pushes return nothing, and the arrangement they build - above all where
// each PushMiddle landed - is what the two strategies could disagree on. PopBack is
// O(1) for both, so the drain adds the same linear cost to each arm while the
// baseline's quadratic shifting still dominates. The output buffer is allocated in
// [GlobalSetup], outside the timed region. LC 1670 allows 1000 calls and the script
// makes two per value, a push and a pop, so the larger Calls is 500; the values
// pushed count from 1, LC 1670's smallest.
public class DesignFrontMiddleBackQueueBenchmarks
{
    private const int OperationCycleLength = 3; // cycles push front/middle/back

    private int[] _popped = [];

    [Params(50, 500)]
    public int Calls { get; set; }

    [GlobalSetup]
    public void Setup() => _popped = new int[Calls];

    [Benchmark(Baseline = true)]
    public int[] ArrayListInsertAtPosition() => RunPushCycle(new DesignFrontMiddleBackQueueSolution.FrontMiddleBackQueueByListInsert());

    [Benchmark]
    public int[] TwoDequeFrontMiddleBackQueue() => RunPushCycle(new DesignFrontMiddleBackQueueSolution.FrontMiddleBackQueueByTwoDeques());

    private int[] RunPushCycle(DesignFrontMiddleBackQueueSolution.IFrontMiddleBackQueue queue)
    {
        for (var i = 0; i < Calls; i++)
        {
            var value = i + 1;

            switch (i % OperationCycleLength)
            {
                case 0: queue.PushFront(value); break;
                case 1: queue.PushMiddle(value); break;
                default: queue.PushBack(value); break;
            }
        }

        for (var i = 0; i < _popped.Length; i++)
        {
            _popped[i] = queue.PopBack();
        }

        return _popped;
    }
}
