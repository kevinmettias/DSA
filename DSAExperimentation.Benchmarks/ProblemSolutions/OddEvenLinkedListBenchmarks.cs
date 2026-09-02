using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.OddEvenLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OddEvenLinkedListSolution's, the same methods
// OddEvenLinkedListTests proves correct. Each iteration clones the pristine list
// first (SetMatrixZeroesBenchmarks precedent), since [GlobalSetup] runs once per
// benchmark, not once per invocation, and the in-place strategy mutates its input.
[MemoryDiagnoser]
public class OddEvenLinkedListBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup() => _head = Build(Enumerable.Range(0, Length).ToArray());

    [Benchmark(Baseline = true)]
    public int TwoListRebuild() => Count(OddEvenLinkedListSolution.GroupOddEvenByTwoListRebuild(Clone(_head)));

    [Benchmark]
    public int InPlaceRewire() => Count(OddEvenLinkedListSolution.GroupOddEvenByInPlaceRewire(Clone(_head)));

    private static int Count(SinglyLinkedListNode<int>? head)
    {
        var count = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            count++;
        }

        return count;
    }

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
