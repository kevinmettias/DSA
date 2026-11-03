using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.Domain.Mathematics.Numerals;
using DSAExperimentation.LeetCode;
using DSAExperimentation.LeetCode.Solutions.DoubleANumberRepresentedAsALinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DoubleANumberRepresentedAsALinkedListSolution's, the
// same methods DoubleANumberRepresentedAsALinkedListSolutionTests proves correct. Neither
// strategy mutates or reverses the input, so the digit list is built once in
// [GlobalSetup] and shared by both arms. The first digit is kept in [1, 10) so the
// workload never carries a leading zero, LC 2816's own input guarantee.
//
// Before this migration both arms instead walked their result to a digit count
// and returned that; returning the answer itself is what makes them the
// methods the tests assert.
public class DoubleANumberRepresentedAsALinkedListBenchmarks
{
    private const int RandomSeed = 2816; // LC problem number

    private SinglyLinkedListNode<int>? _head;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var digits = new int[Length];
        digits[0] = random.Next(1, DecimalDigits.Size);

        for (var index = 1; index < Length; index++)
        {
            digits[index] = random.Next(0, DecimalDigits.Size);
        }

        _head = LeetCodeWireFormat.ToLinkedList(digits);
    }

    [Benchmark(Baseline = true)]
    public SinglyLinkedListNode<int>? BigIntegerConvertDoubleAndBack() =>
        DoubleANumberRepresentedAsALinkedListSolution.DoubleNumberByBigInteger(_head);

    [Benchmark]
    public SinglyLinkedListNode<int>? StackDigitwiseDouble() =>
        DoubleANumberRepresentedAsALinkedListSolution.DoubleNumberByDigitStack(_head);
}
