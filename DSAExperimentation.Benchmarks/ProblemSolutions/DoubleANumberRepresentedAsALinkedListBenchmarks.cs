using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.DoubleANumberRepresentedAsALinkedList;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DoubleANumberRepresentedAsALinkedListSolution's, the
// same methods DoubleANumberRepresentedAsALinkedListTests proves correct. Neither
// strategy mutates or reverses the input, so the digit list is built once in
// [GlobalSetup] and shared by both arms. The first digit is kept in [1, 10) so the
// workload never carries a leading zero, LC 2816's own input guarantee.
//
// The arms return object, not SinglyLinkedListNode<int> - the node type is
// internal, so a public [Benchmark] method cannot name it as a return type
// (CS0050). Before this migration both arms instead walked their result to a digit
// count and returned that; returning the answer itself is what makes them the
// methods the tests assert.
[MemoryDiagnoser]
public class DoubleANumberRepresentedAsALinkedListBenchmarks
{
    private const int RandomSeed = 2816; // LC problem number
    private const int DecimalBase = 10;

    private SinglyLinkedListNode<int>? _head;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var digits = new int[Length];
        digits[0] = random.Next(1, DecimalBase);

        for (var index = 1; index < Length; index++)
        {
            digits[index] = random.Next(0, DecimalBase);
        }

        _head = LeetCodeWireFormat.ToLinkedList(digits);
    }

    [Benchmark(Baseline = true)]
    public object? BigIntegerConvertDoubleAndBack() =>
        DoubleANumberRepresentedAsALinkedListSolution.DoubleNumberByBigInteger(_head);

    [Benchmark]
    public object? StackDigitwiseDouble() =>
        DoubleANumberRepresentedAsALinkedListSolution.DoubleNumberByDigitStack(_head);
}
