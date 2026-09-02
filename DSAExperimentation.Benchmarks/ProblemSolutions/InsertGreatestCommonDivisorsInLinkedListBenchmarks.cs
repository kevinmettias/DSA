using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Insert Greatest Common Divisors in Linked List (LC 2807): materializing the interleaved
// result into a fresh List<int> (OddEvenLinkedListBenchmarks' "extra buffer" contrast)
// vs. this repo's SinglyLinkedListNode<int> spliced in place, one new gcd node stitched
// between each original pair instead of a whole new sequence being built up. Both arms
// clone the shared fixture first so mutating one iteration's result never corrupts the
// next (SetMatrixZeroesBenchmarks precedent).
[MemoryDiagnoser]
public class InsertGreatestCommonDivisorsInLinkedListBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup() => _head = Build(Enumerable.Range(1, Length).ToArray());

    [Benchmark(Baseline = true)]
    public int RebuildViaValueList() => BuildInterleavedArray(Clone(_head)).Length;

    private static int[] BuildInterleavedArray(SinglyLinkedListNode<int> head)
    {
        var original = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            original.Add(node.Value);
        }

        var interleaved = new List<int>((original.Count * 2) - 1);

        for (var i = 0; i < original.Count; i++)
        {
            if (i > 0)
            {
                interleaved.Add(Gcd(original[i - 1], original[i]));
            }

            interleaved.Add(original[i]);
        }

        return interleaved.ToArray();
    }

    [Benchmark]
    public int InPlaceNodeInsertion() => Count(InsertGreatestCommonDivisors(Clone(_head)));

    private static SinglyLinkedListNode<int> InsertGreatestCommonDivisors(SinglyLinkedListNode<int> head)
    {
        var current = head;

        while (current.Next is not null)
        {
            var gcdNode = new SinglyLinkedListNode<int>(Gcd(current.Value, current.Next.Value)) { Next = current.Next };
            current.Next = gcdNode;
            current = gcdNode.Next;
        }

        return head;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

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

    private static int Count(SinglyLinkedListNode<int>? head)
    {
        var count = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            count++;
        }

        return count;
    }
}
