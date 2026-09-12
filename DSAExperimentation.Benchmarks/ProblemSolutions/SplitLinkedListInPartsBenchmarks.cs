using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.SplitLinkedListInParts;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SplitLinkedListInPartsSolution's, the same
// methods SplitLinkedListInPartsTests proves correct. Each benchmark method
// clones the shared fixture first (OddEvenLinkedListBenchmarks precedent)
// since [GlobalSetup] runs once per benchmark, not once per invocation, and
// InPlaceRewire mutates the chain it walks. Both arms report the non-null
// part count rather than the SinglyLinkedListNode<int>?[] itself: that type
// is internal, and a public [Benchmark] method on this public class cannot
// return it (CS0050) - the same constraint OddEvenLinkedListBenchmarks
// resolves the same way, by counting instead of returning the chain.
[MemoryDiagnoser]
public class SplitLinkedListInPartsBenchmarks
{
    private const int Parts = 7;

    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup() => _head = Build(Enumerable.Range(0, Length).ToArray());

    [Benchmark(Baseline = true)]
    public int ArrayRebuild() =>
        CountNonNullParts(SplitLinkedListInPartsSolution.SplitListToPartsByArrayRebuild(Clone(_head), Parts));

    [Benchmark]
    public int InPlaceRewire() =>
        CountNonNullParts(SplitLinkedListInPartsSolution.SplitListToPartsByInPlaceRewire(Clone(_head), Parts));

    private static int CountNonNullParts(SinglyLinkedListNode<int>?[] parts) => parts.Count(p => p is not null);

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
