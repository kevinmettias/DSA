using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Linked List Components (LC 817): an O(n*k) baseline that linearly scans the nums
// array for every list node vs. this repo's own Set<int> (backed by HashMap, same
// ContainsDuplicateBenchmarks precedent) giving O(1) membership per node, O(n+k)
// overall. Every other value is included in nums, so the baseline's linear scan
// never gets to short-circuit early on a hit near the front of the array.
[MemoryDiagnoser]
public class LinkedListComponentsBenchmarks
{
    private const int EvenModulus = 2;

    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;
    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        _head = Build(Enumerable.Range(0, Length).ToArray());
        _nums = Enumerable.Range(0, Length).Where(v => v % EvenModulus == 0).Reverse().ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanPerNode()
    {
        var count = 0;
        var inComponent = false;

        for (var node = _head; node is not null; node = node.Next)
        {
            if (Array.IndexOf(_nums, node.Value) >= 0)
            {
                if (!inComponent)
                {
                    count++;
                }

                inComponent = true;
            }
            else
            {
                inComponent = false;
            }
        }

        return count;
    }

    [Benchmark]
    public int SetMembership()
    {
        var present = BuildPresentSet();
        return CountComponentsBySetMembership(present);
    }

    private Set<int> BuildPresentSet()
    {
        var present = new Set<int>();
        foreach (var n in _nums)
        {
            present.TryAdd(n);
        }

        return present;
    }

    private int CountComponentsBySetMembership(Set<int> present)
    {
        var count = 0;
        var inComponent = false;

        for (var node = _head; node is not null; node = node.Next)
        {
            if (present.Has(node.Value))
            {
                if (!inComponent)
                {
                    count++;
                }

                inComponent = true;
            }
            else
            {
                inComponent = false;
            }
        }

        return count;
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
}
