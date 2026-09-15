using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.LinkedListComponents;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LinkedListComponentsSolution's, the same methods
// LinkedListComponentsTests proves correct - an O(n*k) baseline that linearly scans
// the nums array for every list node against this repo's own Set<int> (HashMap-
// backed, the same ContainsDuplicateBenchmarks precedent) giving O(1) membership
// per node, O(n+k) overall including seeding the set. Every other value is included
// in nums, in descending order, so the baseline's linear scan never gets to
// short-circuit early on a hit near the front of the array.
[MemoryDiagnoser]
public class LinkedListComponentsBenchmarks
{
    private const int EvenModulus = 2;

    private SinglyLinkedListNode<int> _head = null!;

    private int[] _nums = [];
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _head = Build(Enumerable.Range(0, Length).ToArray());
        _nums = Enumerable.Range(0, Length).Where(v => v % EvenModulus == 0).Reverse().ToArray();
    }

    private static SinglyLinkedListNode<int> Build(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next!;
    }

    [Benchmark(Baseline = true)]
    public int LinearScanPerNode() => LinkedListComponentsSolution.NumComponentsByLinearScan(_head, _nums);

    [Benchmark]
    public int SetMembership() => LinkedListComponentsSolution.NumComponentsBySetMembership(_head, _nums);
}
