using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.InsertGreatestCommonDivisorsInLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are InsertGreatestCommonDivisorsInLinkedListSolution's,
// the same methods InsertGreatestCommonDivisorsInLinkedListTests proves correct -
// a whole fresh sequence rebuilt through a List<int> buffer
// (OddEvenLinkedListBenchmarks' "extra buffer" contrast) against one gcd node
// spliced between each original pair. Each arm clones the [GlobalSetup] list
// inside the measured call rather than sharing it, because the splice rewrites
// .Next in place: reusing one pre-built list would let the first iteration's
// insertions make every later iteration measure an already-expanded list
// (SetMatrixZeroesBenchmarks precedent).
[MemoryDiagnoser]
public class InsertGreatestCommonDivisorsInLinkedListBenchmarks
{
    private SinglyLinkedListNode<int> _head = null!;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _head = Build(Enumerable.Range(1, Length).ToArray());

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

    // Returns object, not SinglyLinkedListNode<int> - the node type is internal,
    // so a public [Benchmark] method cannot name it as a return type (CS0050).
    [Benchmark(Baseline = true)]
    public object? ValueRebuild() =>
        InsertGreatestCommonDivisorsInLinkedListSolution.InsertGreatestCommonDivisorsByValueRebuild(Clone(_head));

    [Benchmark]
    public object? NodeSplice() =>
        InsertGreatestCommonDivisorsInLinkedListSolution.InsertGreatestCommonDivisorsByNodeSplice(Clone(_head));

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
