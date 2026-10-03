using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.SplitLinkedListInParts;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SplitLinkedListInPartsSolution's, the same
// methods SplitLinkedListInPartsSolutionTests proves correct. Each benchmark method
// clones the shared fixture first (OddEvenLinkedListBenchmarks precedent)
// since [GlobalSetup] runs once per benchmark, not once per invocation, and
// InPlaceRewire mutates the chain it walks. Both arms return the
// SinglyLinkedListNode<int>?[] of parts itself, as object: that type is
// internal, and a public [Benchmark] method on this public class cannot name
// it (CS0050).
public class SplitLinkedListInPartsBenchmarks
{
    private const int Parts = 7;

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

    [Benchmark(Baseline = true)]
    public object? ArrayRebuild() =>
        SplitLinkedListInPartsSolution.SplitListToPartsByArrayRebuild(Clone(_head), Parts);

    [Benchmark]
    public object? InPlaceRewire() =>
        SplitLinkedListInPartsSolution.SplitListToPartsByInPlaceRewire(Clone(_head), Parts);

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
