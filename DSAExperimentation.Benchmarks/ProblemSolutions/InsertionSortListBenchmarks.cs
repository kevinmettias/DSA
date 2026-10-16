using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.InsertionSortList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is InsertionSortListSolution's, the same method
// InsertionSortListSolutionTests proves correct. Pre-migration this class was an
// untested compile-smoke placeholder (`Baseline() => 1`, `PrimitiveComposed() =>
// 1`) rather than a second strategy to reconcile. [GlobalSetup] hoists the
// shuffled workload values, but the list itself is rebuilt fresh inside the
// benchmark method rather than cached, because insertion sort relinks the input
// list's own nodes in place - a cached list would only be valid for the first
// measured iteration (same shape RotateListBenchmarks already uses). The rebuild
// is timed on purpose because the strategy mutates its input.
public class InsertionSortListBenchmarks
{
    private const int ShuffleSeed = 147;

    private int[] _values = [];

    [Params(100, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = SeededSequences.ShuffledOneTo(Length, ShuffleSeed);

    [Benchmark(Baseline = true)]
    public SinglyLinkedListNode<int>? DummyHeadInsertion() =>
        InsertionSortListSolution.SortByDummyHeadInsertion(LeetCodeWireFormat.ToLinkedList(_values));
}
