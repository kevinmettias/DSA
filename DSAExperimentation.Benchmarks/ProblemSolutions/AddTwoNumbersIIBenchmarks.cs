using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.AddTwoNumbersII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AddTwoNumbersIISolution's, the same methods
// AddTwoNumbersIISolutionTests proves correct. Each arm returns the sum list itself
// as object?, since a public [Benchmark] method cannot name the internal
// SinglyLinkedListNode<int> (CS0050). Digit values only
// need to be in [0, 10) to exercise both strategies' carry handling under load -
// LC 445's "no leading zero" constraint is a correctness concern already covered by
// AddTwoNumbersIISolutionTests, not a perf-harness one.
public class AddTwoNumbersIIBenchmarks
{
    private const int RandomSeed = 445; // LC problem number
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

    [Benchmark(Baseline = true)]
    public object? BigIntegerConvertAndBack() => AddTwoNumbersIISolution.AddByBigInteger(_first, _second);

    [Benchmark]
    public object? TwoStacksDigitwiseAdd() => AddTwoNumbersIISolution.AddByTwoStacks(_first, _second);
}
