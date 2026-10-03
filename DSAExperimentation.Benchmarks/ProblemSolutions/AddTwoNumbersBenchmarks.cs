using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.AddTwoNumbers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AddTwoNumbersSolution's, the same methods
// AddTwoNumbersSolutionTests proves correct. Converting each digit list to a BigInteger
// and back is the naive approach many first reach for - each `* 10` grows the
// running total's limb count by one, so accumulating an n-digit number this way
// costs O(n^2) overall. DigitwiseListWalk instead walks both lists exactly once
// with a running carry - O(n) with a single pass building the output list as it
// goes.
public class AddTwoNumbersBenchmarks
{
    private const int RandomSeed = 11;
    private const int DecimalBase = 10;

    private SinglyLinkedListNode<int> _first = null!;

    private SinglyLinkedListNode<int> _second = null!;
    // LC 2 gives each list 1 to 100 nodes.
    [Params(10, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _first = BuildRandomDigitList(random, Length);
        _second = BuildRandomDigitList(random, Length);
    }

    // LC 2 stores a number least-significant digit first and promises it has no leading
    // zero, so every digit but the last is drawn from 0-9 and the last - the most
    // significant - from 1-9.
    private static SinglyLinkedListNode<int> BuildRandomDigitList(Random random, int length)
    {
        var lowerDigits = SeededDraws.Values(length - 1, 0, DecimalBase, random);
        var head = new SinglyLinkedListNode<int>(random.Next(1, DecimalBase));

        for (var i = lowerDigits.Length - 1; i >= 0; i--)
        {
            head = new SinglyLinkedListNode<int>(lowerDigits[i]) { Next = head };
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
