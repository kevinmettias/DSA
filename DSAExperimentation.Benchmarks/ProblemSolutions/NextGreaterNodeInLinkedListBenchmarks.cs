using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.NextGreaterNodeInLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NextGreaterNodeInLinkedListSolution's, the same
// methods NextGreaterNodeInLinkedListTests proves correct - the canonical O(n^2)
// per-node forward scan vs. a single O(n) monotonic-decreasing sweep through this
// repo's own Stack<int> of pending indices. Values are a random permutation so no
// node's answer short-circuits the brute-force scan early; list construction is
// charged to [GlobalSetup].
[MemoryDiagnoser]
public class NextGreaterNodeInLinkedListBenchmarks
{
    private const int RandomSeed = 1019; private SinglyLinkedListNode<int> _head = null!;

    // LC problem number

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var values = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
        _head = Build(values);
    }

    private static SinglyLinkedListNode<int> Build(int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        foreach (var value in values[1..])
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return head;
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan() =>
        NextGreaterNodeInLinkedListSolution.NextLargerNodesByBruteForceScan(_head);

    [Benchmark]
    public int[] MonotonicStackSweep() =>
        NextGreaterNodeInLinkedListSolution.NextLargerNodesByMonotonicStackSweep(_head);
}
