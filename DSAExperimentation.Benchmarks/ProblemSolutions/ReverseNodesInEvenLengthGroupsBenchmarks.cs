using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode;
using DSAExperimentation.LeetCode.Solutions.ReverseNodesInEvenLengthGroups;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReverseNodesInEvenLengthGroupsSolution's, the same
// methods ReverseNodesInEvenLengthGroupsSolutionTests proves correct - a plain int[]
// baseline that computes the same increasing-then-truncated group boundaries and
// reverses each even-length run with Array.Reverse, against this repo's own
// SinglyLinkedListNode<T> pointer splicing with no array materialization at all.
//
// The pointer arm mutates the list it is handed, so each call builds its own list
// from _values rather than reusing one a later iteration would find reversed. The
// rebuild is timed on purpose, and the array arm, which only reads its list, pays it
// too so both arms carry the same cost.
public class ReverseNodesInEvenLengthGroupsBenchmarks
{
    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public SinglyLinkedListNode<int>? ArrayEvenLengthGroupReverse() =>
        ReverseNodesInEvenLengthGroupsSolution.ReverseEvenLengthGroupsByArrayReverse(
            LeetCodeWireFormat.ToLinkedList(_values));

    [Benchmark]
    public SinglyLinkedListNode<int>? LinkedListEvenLengthGroupReverse() =>
        ReverseNodesInEvenLengthGroupsSolution.ReverseEvenLengthGroupsByPointerReversal(
            LeetCodeWireFormat.ToLinkedList(_values));
}
