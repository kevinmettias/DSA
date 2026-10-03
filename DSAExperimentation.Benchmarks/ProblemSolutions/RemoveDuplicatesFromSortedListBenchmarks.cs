using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveDuplicatesFromSortedListSolution's, the same
// methods RemoveDuplicatesFromSortedListSolutionTests proves correct. [GlobalSetup] hoists
// the workload values, but the list itself is rebuilt fresh inside each benchmark
// method rather than cached, because the in-place-scan strategy splices nodes out
// of the list it is handed - a cached list would only be valid for the first
// measured iteration. The rebuild is timed on purpose, and DistinctFilter, which only
// reads its list, pays it too so both arms carry the same cost.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
//
// Length stops at LC 83's 300 nodes, whose values, climbing in threes from 0, reach 99,
// inside its [-100, 100].
public class RemoveDuplicatesFromSortedListBenchmarks
{
    private const int DuplicateRunLength = 3;

    private int[] _values = [];

    [Params(200, 300)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i / DuplicateRunLength).ToArray();

    [Benchmark(Baseline = true)]
    public object? DistinctFilter() =>
        RemoveDuplicatesFromSortedListSolution.DeleteDuplicatesByDistinctFilter(
            LeetCodeWireFormat.ToLinkedList(_values));

    [Benchmark]
    public object? InPlaceScan() =>
        RemoveDuplicatesFromSortedListSolution.DeleteDuplicatesByInPlaceScan(
            LeetCodeWireFormat.ToLinkedList(_values));
}
