using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.ReverseNodesInKGroup;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReverseNodesInKGroupSolution's, the same methods
// ReverseNodesInKGroupTests proves correct. Both strategies mutate the list they
// are handed, so each call rebuilds its own list from _values rather than
// reusing one shared list a later iteration would find already reversed.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
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
        ReverseNodesInKGroupSolution.ReverseKGroupByArrayReverse(BuildList(_values), GroupSize);

    [Benchmark]
    public object? LinkedListGroupReverse() =>
        ReverseNodesInKGroupSolution.ReverseKGroupByPointerReversal(BuildList(_values), GroupSize);

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }
}
