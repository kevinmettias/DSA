using System.Numerics;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using NumberStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Double a Number Represented as a Linked List (LC 2816): digits are stored
// most-significant-first, so converting the list to a BigInteger first is the naive
// approach many reach for - each `value = value * 10 + digit` step multiplies a
// growing n-digit BigInteger by 10, costing O(n) per digit and O(n^2) overall (the
// same limb-growth cost AddTwoNumbersIIBenchmarks' baseline pays). StackDigitwiseDouble
// instead pushes every digit onto this repo's own Stack<T> while walking .Next once,
// then pops them least-significant-first with a running carry, prepending each
// doubled digit as it's produced - O(n), with the input list never reversed or
// mutated.
[MemoryDiagnoser]
public class DoubleANumberRepresentedAsALinkedListBenchmarks
{
    private const int RandomSeed = 2816; // LeetCode problem number
    private const int DecimalBase = 10;

    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _head = BuildRandomDigitList(random, Length);
    }

    [Benchmark(Baseline = true)]
    public int BigIntegerConvertDoubleAndBack()
    {
        var doubled = ToBigInteger(_head) * 2;
        var digitCount = 0;

        for (var remaining = doubled; remaining > 0; remaining /= DecimalBase)
        {
            digitCount++;
        }

        return digitCount;
    }

    [Benchmark]
    public int StackDigitwiseDouble()
    {
        var digits = new NumberStack();
        for (var node = _head; node is not null; node = node.Next)
        {
            digits.Push(node.Value);
        }

        SinglyLinkedListNode<int>? result = null;
        var carry = 0;

        while (digits.Count > 0 || carry != 0)
        {
            var digit = digits.TryPop(out var value) ? value : 0;
            var doubled = (digit * 2) + carry;
            carry = doubled / DecimalBase;
            result = new SinglyLinkedListNode<int>(doubled % DecimalBase) { Next = result };
        }

        var length = 0;
        for (var node = result; node is not null; node = node.Next)
        {
            length++;
        }

        return length;
    }

    private static BigInteger ToBigInteger(SinglyLinkedListNode<int>? head)
    {
        BigInteger value = 0;
        for (var node = head; node is not null; node = node.Next)
        {
            value = (value * DecimalBase) + node.Value;
        }

        return value;
    }

    // First digit deliberately kept in [1, 10) so the list never carries a leading
    // zero (LC 2816's own input guarantee) - a benchmark-only concern the correctness
    // tests don't need to enforce since they use fixed, already-valid examples.
    private static SinglyLinkedListNode<int> BuildRandomDigitList(Random random, int length)
    {
        var head = new SinglyLinkedListNode<int>(random.Next(1, DecimalBase));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(random.Next(0, DecimalBase));
            tail = tail.Next;
        }

        return head;
    }
}
