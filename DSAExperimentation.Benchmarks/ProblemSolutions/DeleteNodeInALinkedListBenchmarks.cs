using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.DeleteNodeInALinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is DeleteNodeInALinkedListSolution's, the same
// method DeleteNodeInALinkedListTests proves correct. [GlobalSetup] hoists the
// workload values, but the list itself is rebuilt fresh inside the benchmark
// method rather than cached, because DeleteByNextValueCopy mutates the node it is
// handed and splices its successor out - a cached list would only be valid for the
// first measured iteration (mirrors RemoveLinkedListElementsBenchmarks' identical
// rebuild-per-call precedent).
[MemoryDiagnoser]
public class DeleteNodeInALinkedListBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).ToArray();

    [Benchmark]
    public void NextValueCopy()
    {
        var head = BuildList(_values);
        var target = NodeAt(head, Length / 2);

        DeleteNodeInALinkedListSolution.DeleteByNextValueCopy(target);
    }

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        for (var i = 1; i < values.Length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(values[i]);
            tail = tail.Next;
        }

        return head;
    }

    private static SinglyLinkedListNode<int> NodeAt(SinglyLinkedListNode<int> head, int index)
    {
        var node = head;

        for (var i = 0; i < index; i++)
        {
            node = node.Next!;
        }

        return node;
    }
}
