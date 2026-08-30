using System.Numerics;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using NumberStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Add Two Numbers II (LC 445): digits are stored most-significant-first, so
// converting each list to a BigInteger is the naive approach many reach for first -
// each `value = value * 10 + digit` step multiplies a growing n-digit BigInteger by
// 10, costing O(n) per digit and O(n^2) overall (the same limb-growth cost
// AddTwoNumbersBenchmarks' BigIntegerConvertAndBack baseline pays for LC 2's
// least-significant-first variant). TwoStacksDigitwiseAdd instead pushes each list's
// digits onto this repo's own Stack<T> while walking .Next once, then pops both
// stacks in lockstep with a running carry, prepending each result digit as it's
// produced - O(n), with neither input list ever reversed or mutated.
[MemoryDiagnoser]
public class AddTwoNumbersIIBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _first = null!;
    private SinglyLinkedListNode<int> _second = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(445);
        _first = BuildRandomDigitList(random, Length);
        _second = BuildRandomDigitList(random, Length);
    }

    [Benchmark(Baseline = true)]
    public int BigIntegerConvertAndBack()
    {
        var sum = ToBigInteger(_first) + ToBigInteger(_second);
        var digitCount = 0;

        for (var remaining = sum; remaining > 0; remaining /= 10)
        {
            digitCount++;
        }

        return digitCount;
    }

    [Benchmark]
    public int TwoStacksDigitwiseAdd()
    {
        var firstDigits = new NumberStack();
        for (SinglyLinkedListNode<int>? node = _first; node is not null; node = node.Next)
        {
            firstDigits.Push(node.Value);
        }

        var secondDigits = new NumberStack();
        for (SinglyLinkedListNode<int>? node = _second; node is not null; node = node.Next)
        {
            secondDigits.Push(node.Value);
        }

        SinglyLinkedListNode<int>? head = null;
        var carry = 0;

        while (firstDigits.Count > 0 || secondDigits.Count > 0 || carry != 0)
        {
            var a = firstDigits.TryPop(out var firstDigit) ? firstDigit : 0;
            var b = secondDigits.TryPop(out var secondDigit) ? secondDigit : 0;
            var digitSum = carry + a + b;
            carry = digitSum / 10;

            head = new SinglyLinkedListNode<int>(digitSum % 10) { Next = head };
        }

        var length = 0;
        for (var node = head; node is not null; node = node.Next)
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
            value = (value * 10) + node.Value;
        }

        return value;
    }

    // Digit values only need to be in [0, 10) to exercise both strategies' carry
    // handling under load - LC 445's "no leading zero" constraint is a correctness
    // concern already covered by AddTwoNumbersIITests, not a perf-harness one.
    private static SinglyLinkedListNode<int> BuildRandomDigitList(Random random, int length)
    {
        var head = new SinglyLinkedListNode<int>(random.Next(0, 10));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(random.Next(0, 10));
            tail = tail.Next;
        }

        return head;
    }
}
