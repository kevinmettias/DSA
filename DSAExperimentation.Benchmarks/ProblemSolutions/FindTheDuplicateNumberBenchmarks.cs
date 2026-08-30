using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find the Duplicate Number (LC 287): the canonical O(n^2) all-pairs brute force vs.
// treating nums[i] as a pointer from node i to node nums[i] and handing the resulting
// implicit linked list to this repo's own Floyd's-algorithm CycleDetection.FindCycleStart
// - O(n) time, O(1) extra space beyond the materialized nodes, no sorting or hashing.
// _values is 1..Length with Length itself appended again, so the only matching pair is
// the very last one the brute force reaches, forcing its full O(n^2) scan.
[MemoryDiagnoser]
public class FindTheDuplicateNumberBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        _values = new int[Length + 1];

        for (var i = 0; i < Length; i++)
        {
            _values[i] = i + 1;
        }

        _values[Length] = Length;
    }

    [Benchmark(Baseline = true)]
    public int NestedLoopBruteForce()
    {
        for (var i = 0; i < _values.Length; i++)
        {
            for (var j = i + 1; j < _values.Length; j++)
            {
                if (_values[i] == _values[j])
                {
                    return _values[i];
                }
            }
        }

        return -1;
    }

    [Benchmark]
    public int LinkedListCycleDetection()
    {
        var nodes = new SinglyLinkedListNode<int>[_values.Length];

        for (var i = 0; i < _values.Length; i++)
        {
            nodes[i] = new SinglyLinkedListNode<int>(i);
        }

        for (var i = 0; i < _values.Length; i++)
        {
            nodes[i].Next = nodes[_values[i]];
        }

        return CycleDetection.FindCycleStart(nodes[0])!.Value;
    }
}
