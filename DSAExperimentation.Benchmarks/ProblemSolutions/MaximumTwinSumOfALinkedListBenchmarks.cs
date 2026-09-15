using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.MaximumTwinSumOfALinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumTwinSumOfALinkedListSolution's, the same
// methods MaximumTwinSumOfALinkedListTests proves correct. [GlobalSetup] builds
// the chain (workload sizing), so each measured call is only the walk and the
// pairing - LeetCode's own input shape is already the prepared repo object here,
// and neither strategy mutates it, so neither needs a hoisted overload.
[MemoryDiagnoser]
public class MaximumTwinSumOfALinkedListBenchmarks
{
    private const int MaxValueExclusive = 100_000;

    // The deterministic value seed this benchmark has always used.
    private const int ValueSeed = 1;

    private SinglyLinkedListNode<int> _head = null!;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _head = BuildRandomList(Length, seed: ValueSeed);

    private static SinglyLinkedListNode<int> BuildRandomList(int length, int seed)
    {
        var random = new Random(seed);
        var head = new SinglyLinkedListNode<int>(random.Next(1, MaxValueExclusive));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(random.Next(1, MaxValueExclusive));
            tail = tail.Next;
        }

        return head;
    }

    [Benchmark(Baseline = true)]
    public int ArrayIndexTwoPointer() =>
        MaximumTwinSumOfALinkedListSolution.PairSumByArrayIndexTwoPointer(_head);

    [Benchmark]
    public int DequeFrontBackDrain() =>
        MaximumTwinSumOfALinkedListSolution.PairSumByDequeFrontBackDrain(_head);
}
