using System.Numerics;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Add Two Numbers (LC 2): converting each digit list to a BigInteger and back is
// the naive approach many first reach for - each `* 10` grows the running total's
// limb count by one, so accumulating an n-digit number this way costs O(n^2)
// overall. DigitwiseListWalk instead walks both lists exactly once with a running
// carry, using this repo's own SinglyLinkedListNode<int>.Next the same way
// MergeTwoSortedListsTests splices nodes - O(n) with a single pass building the
// output list as it goes.
[MemoryDiagnoser]
public class AddTwoNumbersBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _first = null!;
    private SinglyLinkedListNode<int> _second = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(11);
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
    public int DigitwiseListWalk()
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        var carry = 0;
        SinglyLinkedListNode<int>? first = _first;
        SinglyLinkedListNode<int>? second = _second;

        while (first is not null || second is not null || carry != 0)
        {
            var digitSum = carry + (first?.Value ?? 0) + (second?.Value ?? 0);
            carry = digitSum / 10;

            tail.Next = new SinglyLinkedListNode<int>(digitSum % 10);
            tail = tail.Next;

            first = first?.Next;
            second = second?.Next;
        }

        var length = 0;
        for (var node = dummy.Next; node is not null; node = node.Next)
        {
            length++;
        }

        return length;
    }

    private static BigInteger ToBigInteger(SinglyLinkedListNode<int>? head)
    {
        BigInteger value = 0;
        BigInteger placeValue = 1;

        for (var node = head; node is not null; node = node.Next)
        {
            value += node.Value * placeValue;
            placeValue *= 10;
        }

        return value;
    }

    // Digit values only need to be in [0, 10) to exercise both strategies' carry
    // handling under load - LC 2's "no leading zero" constraint is a correctness
    // concern already covered by AddTwoNumbersTests, not a perf-harness one.
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
