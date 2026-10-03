using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.MergeKSortedLists;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MergeKSortedListsSolution's, the same methods
// MergeKSortedListsSolutionTests proves correct. MergeListsByHeap splices the input
// nodes' own Next pointers into the merged chain rather than allocating new
// ones, so - unlike AddTwoNumbersBenchmarks' non-destructive reads - the list
// array cannot be hoisted into [GlobalSetup] and reused across iterations: a
// merge from one iteration would consume the very structure the next iteration
// needs. [GlobalSetup] therefore only seeds the raw per-list values, and each
// [Benchmark] call rebuilds a fresh SinglyLinkedListNode<int>?[] from them. The
// rebuild is timed on purpose, and FlattenSort, which only reads the lists, pays it
// too so both arms carry the same cost.
//
// Returns object, not SinglyLinkedListNode<int>? - the node type is internal, so
// a public [Benchmark] method cannot name it as a return type (CS0050).
public class MergeKSortedListsBenchmarks
{
    private const int ValuesPerList = 64;

    private int[][] _values = [];

    [Params(8, 64)]
    public int ListCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _values = Enumerable.Range(0, ListCount)
            .Select(offset => Enumerable.Range(0, ValuesPerList).Select(i => (i * ListCount) + offset).ToArray())
            .ToArray();

    [Benchmark(Baseline = true)]
    public object? FlattenSort() =>
        MergeKSortedListsSolution.MergeListsByFlattenSort(BuildLists());

    [Benchmark]
    public object? MergeByHeap() =>
        MergeKSortedListsSolution.MergeListsByHeap(BuildLists());

    private SinglyLinkedListNode<int>?[] BuildLists() => _values.Select(LeetCodeWireFormat.ToLinkedList).ToArray();
}
