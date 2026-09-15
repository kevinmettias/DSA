using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.ConvertBinaryNumberInALinkedListToInteger;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ConvertBinaryNumberInALinkedListToIntegerSolution's,
// the same methods ConvertBinaryNumberInALinkedListToIntegerTests proves correct.
// CollectThenFold is the naive two-pass approach (collect every bit into a buffer,
// then fold positional weights right-to-left); SinglePassShift folds
// value = (value << 1) | bit in one walk. Both are O(n), but the baseline
// allocates an intermediate buffer and touches the list and the buffer separately.
[MemoryDiagnoser]
public class ConvertBinaryNumberInALinkedListToIntegerBenchmarks
{
    private const int RandomSeed = 1290; // LeetCode problem number
    private const int BitValueUpperBoundExclusive = 2;

    private SinglyLinkedListNode<int> _head = null!;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _head = BuildRandomBitList(random, Length);
    }

    private static SinglyLinkedListNode<int> BuildRandomBitList(Random random, int length)
    {
        var head = new SinglyLinkedListNode<int>(random.Next(0, BitValueUpperBoundExclusive));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(random.Next(0, BitValueUpperBoundExclusive));
            tail = tail.Next;
        }

        return head;
    }

    [Benchmark(Baseline = true)]
    public int CollectThenFold() =>
        ConvertBinaryNumberInALinkedListToIntegerSolution.GetDecimalValueByCollectThenFold(_head);

    [Benchmark]
    public int SinglePassShift() =>
        ConvertBinaryNumberInALinkedListToIntegerSolution.GetDecimalValueBySinglePassShift(_head);
}
