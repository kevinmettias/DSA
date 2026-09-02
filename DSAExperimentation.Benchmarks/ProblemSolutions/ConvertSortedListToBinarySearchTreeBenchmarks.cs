using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.ConvertSortedListToBinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is
// ConvertSortedListToBinarySearchTreeSolution's, the same method
// ConvertSortedListToBinarySearchTreeTests proves correct. The original
// benchmark's two [Benchmark] arms were an unimplemented compile-smoke
// placeholder (both returned the literal 1) rather than a second real
// strategy, so there is only one arm here too, mirroring
// ConvertSortedArrayToBinarySearchTreeBenchmarks for LC 108.
//
// Returns object, not BinaryTreeNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class ConvertSortedListToBinarySearchTreeBenchmarks
{
    private const int ValueStep = 3;

    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int>? _head;

    [GlobalSetup]
    public void Setup() => _head = BuildAscendingList(Length);

    [Benchmark(Baseline = true)]
    public object? MidpointRecursion() =>
        ConvertSortedListToBinarySearchTreeSolution.BuildByMidpointRecursion(_head);

    private static SinglyLinkedListNode<int>? BuildAscendingList(int length)
    {
        if (length == 0)
        {
            return null;
        }

        var head = new SinglyLinkedListNode<int>(0);
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(i * ValueStep);
            tail = tail.Next;
        }

        return head;
    }
}
