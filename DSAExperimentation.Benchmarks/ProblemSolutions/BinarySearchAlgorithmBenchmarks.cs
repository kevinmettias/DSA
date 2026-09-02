using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Binary Search (LC 704): the canonical O(n) linear scan vs. this repo's own
// O(log n) BinarySearch.Find over an ArraySequence<int>. _target is deliberately
// the last element in the sorted array so LinearScan is forced through its full
// worst-case pass instead of an early exit near the start making it look
// artificially competitive.
[MemoryDiagnoser]
public class BinarySearchAlgorithmBenchmarks
{
    // Spacing between consecutive generated values, so every value is even.
    private const int ValueStride = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;
    private int _target;

    [GlobalSetup]
    public void Setup()
    {
        _values = Enumerable.Range(0, Length).Select(i => i * ValueStride).ToArray();
        _target = (Length - 1) * ValueStride;
    }

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        for (var i = 0; i < _values.Length; i++)
        {
            if (_values[i] == _target)
            {
                return i;
            }
        }

        return -1;
    }

    [Benchmark]
    public int BinarySearchFind()
        => BinarySearch.Find(new ArraySequence<int>(_values), _target) ?? -1;
}
