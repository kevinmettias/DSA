using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.AddTwoNumbersII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AddTwoNumbersIISolution's, the same methods
// AddTwoNumbersIITests proves correct. CountDigits only exists to give a
// [Benchmark] method (which must be public) a public return value for an internal
// SinglyLinkedListNode<int>, the same technique
// SerializeAndDeserializeBinaryTreeBenchmarks' CountNodes uses. Digit values only
// need to be in [0, 10) to exercise both strategies' carry handling under load -
// LC 445's "no leading zero" constraint is a correctness concern already covered by
// AddTwoNumbersIITests, not a perf-harness one.
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
    public int BigIntegerConvertAndBack() =>
        CountDigits(AddTwoNumbersIISolution.AddByBigInteger(_first, _second));

    [Benchmark]
    public int TwoStacksDigitwiseAdd() =>
        CountDigits(AddTwoNumbersIISolution.AddByTwoStacks(_first, _second));

    private static int CountDigits(SinglyLinkedListNode<int>? head)
    {
        var count = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            count++;
        }

        return count;
    }

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
}
