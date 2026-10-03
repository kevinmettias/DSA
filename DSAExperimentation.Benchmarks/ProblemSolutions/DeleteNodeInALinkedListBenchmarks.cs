using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.DeleteNodeInALinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is DeleteNodeInALinkedListSolution's, the same
// method DeleteNodeInALinkedListSolutionTests proves correct. [GlobalSetup] hoists the
// workload values, but the list itself is rebuilt fresh inside the benchmark
// method rather than cached, because DeleteByNextValueCopy mutates the node it is
// handed and splices its successor out - a cached list would only be valid for the
// first measured iteration (mirrors RemoveLinkedListElementsBenchmarks' identical
// rebuild-per-call precedent). The rebuild and the walk to the target are timed on
// purpose because the strategy mutates its input, and they are most of what the arm
// costs: the deletion itself is O(1). The arm returns the rebuilt list's head as object?,
// since the node type is internal (CS0050), so what the deletion left is its answer
// rather than something the arm builds and drops.
public class DeleteNodeInALinkedListBenchmarks
{
    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).ToArray();

    [Benchmark]
    public object? NextValueCopy()
    {
        var head = LeetCodeWireFormat.ToLinkedList(_values)!;
        var target = NodeAt(head, Length / 2);

        DeleteNodeInALinkedListSolution.DeleteByNextValueCopy(target);

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
