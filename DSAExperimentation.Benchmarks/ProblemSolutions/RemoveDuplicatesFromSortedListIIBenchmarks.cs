using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedListII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveDuplicatesFromSortedListIISolution's, the same
// methods RemoveDuplicatesFromSortedListIISolutionTests proves correct. [GlobalSetup]
// hoists the workload values, but the list itself is rebuilt fresh inside each
// benchmark method rather than cached, because the two-pointer-scan strategy
// splices nodes out of the list it is handed - a cached list would only be valid
// for the first measured iteration. The rebuild is timed on purpose, and
// ArrayGroupFilter, which only reads its list, pays it too so both arms carry the
// same cost.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
//
// Length stops at LC 82's 300 nodes, and the values climb in pairs from -100, its lowest
// value, so 300 nodes reach 49, inside its [-100, 100].
public class RemoveDuplicatesFromSortedListIIBenchmarks
{
    private const int RunLengthDivisor = 2;
    private const int LowestNodeValue = -100;

    private int[] _values = [];

    [Params(200, 300)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _values = Enumerable.Range(0, Length).Select(i => LowestNodeValue + (i / RunLengthDivisor)).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayGroupFilter() =>
        RemoveDuplicatesFromSortedListIISolution.DeleteDuplicatesByArrayGroupFilter(
            LeetCodeWireFormat.ToLinkedList(_values));

    [Benchmark]
    public object? TwoPointerScan() =>
        RemoveDuplicatesFromSortedListIISolution.DeleteDuplicatesByTwoPointerScan(
            LeetCodeWireFormat.ToLinkedList(_values));
}
