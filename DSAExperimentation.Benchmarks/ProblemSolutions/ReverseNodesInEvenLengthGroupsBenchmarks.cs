using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.ReverseNodesInEvenLengthGroups;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReverseNodesInEvenLengthGroupsSolution's, the same
// methods ReverseNodesInEvenLengthGroupsTests proves correct - a plain int[]
// baseline that computes the same increasing-then-truncated group boundaries and
// reverses each even-length run with Array.Reverse, against this repo's own
// SinglyLinkedListNode<T> pointer splicing with no array materialization at all.
//
// The pointer arm mutates the list it is handed, so each call builds its own list
// from _values rather than reusing one a later iteration would find reversed.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class ReverseNodesInEvenLengthGroupsBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayEvenLengthGroupReverse() =>
        ReverseNodesInEvenLengthGroupsSolution.ReverseEvenLengthGroupsByArrayReverse(
            LeetCodeWireFormat.ToLinkedList(_values));

    [Benchmark]
    public object? LinkedListEvenLengthGroupReverse() =>
        ReverseNodesInEvenLengthGroupsSolution.ReverseEvenLengthGroupsByPointerReversal(
            LeetCodeWireFormat.ToLinkedList(_values));
}
