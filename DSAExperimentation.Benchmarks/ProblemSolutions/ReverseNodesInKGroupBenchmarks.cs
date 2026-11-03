using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode;
using DSAExperimentation.LeetCode.Solutions.ReverseNodesInKGroup;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReverseNodesInKGroupSolution's, the same methods
// ReverseNodesInKGroupSolutionTests proves correct. The pointer strategy mutates the
// list it is handed, so each call rebuilds its own list from _values rather than
// reusing one shared list a later iteration would find already reversed. The rebuild
// is timed on purpose, and the array arm, which only reads its list, pays it too so
// both arms carry the same cost.
//
// The node at position i holds i + 1, wrapping back to 1 after 1,000, the highest
// value LC 25 allows.
public class ReverseNodesInKGroupBenchmarks
{
    private const int GroupSize = 4;
    private const int HighestNodeValue = 1_000;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _values = Enumerable.Range(0, Length).Select(position => (position % HighestNodeValue) + 1).ToArray();

    [Benchmark(Baseline = true)]
    public SinglyLinkedListNode<int>? ArrayGroupReverse() =>
        ReverseNodesInKGroupSolution.ReverseKGroupByArrayReverse(
            LeetCodeWireFormat.ToLinkedList(_values), GroupSize);

    [Benchmark]
    public SinglyLinkedListNode<int>? LinkedListGroupReverse() =>
        ReverseNodesInKGroupSolution.ReverseKGroupByPointerReversal(
            LeetCodeWireFormat.ToLinkedList(_values), GroupSize);
}
