using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.AddTwoNumbers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AddTwoNumbersSolution's, the same methods
// AddTwoNumbersTests proves correct. Converting each digit list to a BigInteger
// and back is the naive approach many first reach for - each `* 10` grows the
// running total's limb count by one, so accumulating an n-digit number this way
// costs O(n^2) overall. DigitwiseListWalk instead walks both lists exactly once
// with a running carry - O(n) with a single pass building the output list as it
// goes.
[MemoryDiagnoser]
public class AddTwoNumbersBenchmarks
{
    private const int RandomSeed = 11;
    private const int DecimalBase = 10;

    private SinglyLinkedListNode<int> _first = null!;

    private SinglyLinkedListNode<int> _second = null!;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _first = BuildRandomDigitList(random, Length);
        _second = BuildRandomDigitList(random, Length);
    }

    // Digit values only need to be in [0, 10) to exercise both strategies' carry
    // handling under load - LC 2's "no leading zero" constraint is a correctness
    // concern already covered by AddTwoNumbersTests, not a perf-harness one.
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

    // Returns object, not SinglyLinkedListNode<int> - the node type is internal,
    // so a public [Benchmark] method cannot name it as a return type (CS0050).
    // Returning the built list itself as object still forces both strategies
    // through their full construction and keeps BenchmarkDotNet from treating the
    // call as dead code, which is the point: measuring a length or count instead
    // would be the same weaker-than-the-real-answer shortcut this migration
    // removes everywhere else.
    [Benchmark(Baseline = true)]
    public object? BigIntegerConvertAndBack() =>
        AddTwoNumbersSolution.AddByBigIntegerConvertAndBack(_first, _second);

    [Benchmark]
    public object? DigitwiseListWalk() =>
        AddTwoNumbersSolution.AddByDigitwiseListWalk(_first, _second);
}
