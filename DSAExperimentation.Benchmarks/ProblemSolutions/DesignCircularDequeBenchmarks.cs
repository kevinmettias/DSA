using DSAExperimentation.LeetCode.DesignCircularDeque;
using ChurnStep = (bool? DeletedFront, bool InsertedLast, bool? DeletedLast, bool InsertedFront, int Front, int Rear);

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignCircularDequeSolution's, the same classes
// DesignCircularDequeSolutionTests proves correct. Each [Benchmark] churns an insert-last/
// insert-front/delete-front/delete-last cycle that settles into a steady state at
// (near) capacity, forcing every operation through the wraparound path on both
// ends - the same shape DesignCircularQueueBenchmarks already uses for its own
// instance-API problem, extended to both ends since this problem allows both.
//
// LC 641 allows 2000 calls and a churn step makes at most eight, so the churn runs
// 250 steps and inserts only values inside LC 641's 0..1000. The 8-slot deque is
// full within four steps; the 512-slot one fills just 500 slots in those 250, so it
// is timed on the insert path without the deletes.
public class DesignCircularDequeBenchmarks
{
    private const int OperationCount = 250;

    // Every answer each churn step observed, in call order. A Delete is null on a step where
    // IsFull answered that the deque still had room, so IsFull's answers are recorded too.
    // Sized in setup so the churn allocates nothing.
    private ChurnStep[] _steps = [];

    [Params(8, 512)]
    public int Capacity { get; set; }

    [GlobalSetup]
    public void Setup() => _steps = new ChurnStep[OperationCount];

    [Benchmark(Baseline = true)]
    public ChurnStep[] ArrayBacked() =>
        RunChurnCycle(new DesignCircularDequeSolution.CircularDequeByArrayBacked(Capacity));

    [Benchmark]
    public ChurnStep[] DequeBacked() =>
        RunChurnCycle(new DesignCircularDequeSolution.CircularDequeByDequeBacked(Capacity));

    // Returns every answer the churn observed rather than discarding it, so the JIT
    // can't eliminate the churn as dead code and the arms are compared on all of it.
    private ChurnStep[] RunChurnCycle(DesignCircularDequeSolution.ICircularDeque deque)
    {
        for (var i = 0; i < OperationCount; i++)
        {
            bool? deletedFront = deque.IsFull() ? deque.DeleteFront() : null;
            var insertedLast = deque.InsertLast(i);
            bool? deletedLast = deque.IsFull() ? deque.DeleteLast() : null;
            var insertedFront = deque.InsertFront(i);
            _steps[i] = (deletedFront, insertedLast, deletedLast, insertedFront, deque.GetFront(), deque.GetRear());
        }

        return _steps;
    }
}
