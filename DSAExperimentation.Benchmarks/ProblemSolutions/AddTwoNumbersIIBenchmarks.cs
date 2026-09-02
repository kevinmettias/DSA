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
    private const int RandomSeed = 445; // LC problem number
    private const int DecimalBase = 10;

    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _first = null!;
    private SinglyLinkedListNode<int> _second = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _first = BuildRandomDigitList(random, Length);
        _second = BuildRandomDigitList(random, Length);
    }

    [Benchmark(Baseline = true)]
    public int BigIntegerConvertAndBack()
    {
        var sum = ToBigInteger(_first) + ToBigInteger(_second);
        var digitCount = 0;

        for (var remaining = sum; remaining > 0; remaining /= DecimalBase)
        {
            digitCount++;
        }

        return digitCount;
    }

    [Benchmark]
    public int TwoStacksDigitwiseAdd()
    {
        var firstDigits = new NumberStack();
        PushDigits(firstDigits, _first);

        var secondDigits = new NumberStack();
        PushDigits(secondDigits, _second);

        var state = new StackAddState(null, 0);

        while (firstDigits.Count > 0 || secondDigits.Count > 0 || state.Carry != 0)
        {
            state = AdvanceStackAdd(firstDigits, secondDigits, state);
        }

        var length = 0;
        for (var node = state.Head; node is not null; node = node.Next)
        {
            length++;
        }

        return length;
    }

    private static void PushDigits(NumberStack stack, SinglyLinkedListNode<int>? node)
    {
        for (; node is not null; node = node.Next)
        {
            stack.Push(node.Value);
        }
    }

    private static StackAddState AdvanceStackAdd(NumberStack firstDigits, NumberStack secondDigits, StackAddState state)
    {
        var a = firstDigits.TryPop(out var firstDigit) ? firstDigit : 0;
        var b = secondDigits.TryPop(out var secondDigit) ? secondDigit : 0;
        var digitSum = state.Carry + a + b;
        var carry = digitSum / DecimalBase;
        var head = new SinglyLinkedListNode<int>(digitSum % DecimalBase) { Next = state.Head };

        return new StackAddState(head, carry);
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

    // Digit values only need to be in [0, 10) to exercise both strategies' carry
    // handling under load - LC 445's "no leading zero" constraint is a correctness
    // concern already covered by AddTwoNumbersIITests, not a perf-harness one.
    private static SinglyLinkedListNode<int> BuildRandomDigitList(Random random, int length)
    {
        var head = new SinglyLinkedListNode<int>(random.Next(0, DecimalBase));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(random.Next(0, DecimalBase));
            tail = tail.Next;
        }

        return head;
    }

    private readonly record struct StackAddState(SinglyLinkedListNode<int>? Head, int Carry);
}
