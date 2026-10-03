using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.ReverseNodesInKGroup;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReverseNodesInKGroupSolution's, the same methods
// ReverseNodesInKGroupSolutionTests proves correct. Both strategies mutate the list they
// are handed, so each call rebuilds its own list from _values rather than
// reusing one shared list a later iteration would find already reversed.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
public class ReverseNodesInKGroupBenchmarks
{
    private const int GroupSize = 4;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayGroupReverse() =>
        ReverseNodesInKGroupSolution.ReverseKGroupByArrayReverse(
            LeetCodeWireFormat.ToLinkedList(_values), GroupSize);

    [Benchmark]
    public object? LinkedListGroupReverse() =>
        ReverseNodesInKGroupSolution.ReverseKGroupByPointerReversal(
            LeetCodeWireFormat.ToLinkedList(_values), GroupSize);
}
