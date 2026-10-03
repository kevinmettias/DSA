using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.AddTwoNumbersII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AddTwoNumbersIISolution's, the same methods
// AddTwoNumbersIISolutionTests proves correct. Each arm returns the sum list itself
// as object?, since a public [Benchmark] method cannot name the internal
// SinglyLinkedListNode<int> (CS0050). LC 445 stores a number most-significant digit
// first and promises it has no leading zero, so each list's head is drawn from 1-9
// and every later digit from 0-9.
public class AddTwoNumbersIIBenchmarks
{
    private const int RandomSeed = 445; // LC problem number
    private const int DecimalBase = 10;

    private SinglyLinkedListNode<int> _first = null!;

    private SinglyLinkedListNode<int> _second = null!;
    // LC 445 gives each list 1 to 100 nodes.
    [Params(10, 100)]
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
        var head = new SinglyLinkedListNode<int>(random.Next(1, DecimalBase));
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
