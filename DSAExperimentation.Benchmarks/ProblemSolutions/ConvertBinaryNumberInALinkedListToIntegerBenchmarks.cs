using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Convert Binary Number in a Linked List to Integer (LC 1290): a naive
// two-pass approach (collect every bit into a buffer, then fold positional
// weights right-to-left) vs. a single left-to-right walk over this repo's own
// SinglyLinkedListNode<int>.Next that folds value = (value << 1) | bit as it
// goes (ConvertBinaryNumberInALinkedListToIntegerTests precedent,
// MiddleOfTheLinkedListBenchmarks shape). Both are O(n), but CollectThenFold
// allocates an intermediate buffer and touches the list and the buffer
// separately while SinglePassShift touches the list once with no extra
// allocation.
[MemoryDiagnoser]
public class ConvertBinaryNumberInALinkedListToIntegerBenchmarks
{
    private const int RandomSeed = 1290; // LeetCode problem number
    private const int BitValueUpperBoundExclusive = 2;

    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _head = BuildRandomBitList(random, Length);
    }

    [Benchmark(Baseline = true)]
    public long CollectThenFold()
    {
        var bits = new List<int>();

        for (var node = _head; node is not null; node = node.Next)
        {
            bits.Add(node.Value);
        }

        var value = 0L;
        var weight = 1L;

        for (var i = bits.Count - 1; i >= 0; i--)
        {
            value += bits[i] * weight;
            weight <<= 1;
        }

        return value;
    }

    [Benchmark]
    public long SinglePassShift()
    {
        var value = 0L;

        for (var node = _head; node is not null; node = node.Next)
        {
            value = (value << 1) | (uint)node.Value;
        }

        return value;
    }

    private static SinglyLinkedListNode<int> BuildRandomBitList(Random random, int length)
    {
        var head = new SinglyLinkedListNode<int>(random.Next(0, BitValueUpperBoundExclusive));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(random.Next(0, BitValueUpperBoundExclusive));
            tail = tail.Next;
        }

        return head;
    }
}
