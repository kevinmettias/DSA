using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.OddEvenLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OddEvenLinkedListSolution's, the same methods
// OddEvenLinkedListSolutionTests proves correct. Each iteration clones the pristine list
// first (SetMatrixZeroesBenchmarks precedent), since [GlobalSetup] runs once per
// benchmark, not once per invocation, and the in-place strategy mutates its input.
public class OddEvenLinkedListBenchmarks
{
    private SinglyLinkedListNode<int> _head = null!;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _head = Build(Enumerable.Range(0, Length).ToArray());

    private static SinglyLinkedListNode<int> Build(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next!;
    }

    // Each arm returns the regrouped list's head, as object because SinglyLinkedListNode<int>
    // is internal and a public [Benchmark] method cannot name it.
    [Benchmark(Baseline = true)]
    public object? TwoListRebuild() => OddEvenLinkedListSolution.GroupOddEvenByTwoListRebuild(Clone(_head));

    [Benchmark]
    public object? InPlaceRewire() => OddEvenLinkedListSolution.GroupOddEvenByInPlaceRewire(Clone(_head));

    private static SinglyLinkedListNode<int> Clone(SinglyLinkedListNode<int> head)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        for (var node = head; node is not null; node = node.Next)
        {
            tail.Next = new SinglyLinkedListNode<int>(node.Value);
            tail = tail.Next;
        }

        return dummy.Next!;
    }
}
