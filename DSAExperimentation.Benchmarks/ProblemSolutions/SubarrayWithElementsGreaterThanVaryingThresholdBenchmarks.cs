using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Subarray With Elements Greater Than Varying Threshold (LC 2334): Threshold is set
// deliberately unreachable (larger than Length * MaxValue, the biggest length*value
// product either strategy could ever see) so both benchmarks are forced through
// their full worst case instead of an early return on the first qualifying window
// making one look artificially fast - the same "force the real worst case"
// convention TwoSumBenchmarks/GraphConnectivityWithThresholdBenchmarks already use.
// SlidingWindowMin is the textbook O(n^2) brute force: for every start index, extend
// the window right while tracking a running minimum. UnionFindByValue instead sorts
// indices by value descending (this repo's own MergeSort.Sort over an
// ArrayIndexedSequence<int>) and grows each index's window via this repo's own
// DisjointSet, unioning only already-processed (so already >= current value)
// neighbors - O(n log n) for the sort plus O(n * alpha(n)) for the union sweep.
[MemoryDiagnoser]
public class SubarrayWithElementsGreaterThanVaryingThresholdBenchmarks
{
    private const int MaxValueExclusive = 1_000;

    [Params(500, 3_000)]
    public int Length;

    private int[] _nums = null!;
    private long _threshold;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(2334); // LC problem number
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _threshold = (long)Length * MaxValueExclusive + 1; // unreachable: no window can qualify
    }

    [Benchmark(Baseline = true)]
    public int SlidingWindowMin()
    {
        for (var start = 0; start < _nums.Length; start++)
        {
            var min = int.MaxValue;

            for (var end = start; end < _nums.Length; end++)
            {
                min = Math.Min(min, _nums[end]);
                var length = end - start + 1;

                if ((long)length * min > _threshold)
                {
                    return length;
                }
            }
        }

        return -1;
    }

    [Benchmark]
    public int UnionFindByValue()
    {
        var n = _nums.Length;
        var order = Enumerable.Range(0, n).ToArray();
        var byDescendingValueThenIndex = Comparer<int>.Create(
            (a, b) => _nums[a] != _nums[b] ? _nums[b].CompareTo(_nums[a]) : a.CompareTo(b));

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(order), byDescendingValueThenIndex);

        var components = new DisjointSet(n);
        var size = new int[n];
        var visited = new bool[n];

        foreach (var index in order)
        {
            visited[index] = true;
            size[index] = 1;

            if (index > 0 && visited[index - 1])
            {
                MergeInto(components, size, index, index - 1);
            }

            if (index < n - 1 && visited[index + 1])
            {
                MergeInto(components, size, index, index + 1);
            }

            var root = components.Find(index);
            if ((long)size[root] * _nums[index] > _threshold)
            {
                return size[root];
            }
        }

        return -1;
    }

    private static void MergeInto(DisjointSet components, int[] size, int index, int neighbor)
    {
        var combined = size[components.Find(index)] + size[components.Find(neighbor)];
        components.Union(index, neighbor);
        size[components.Find(index)] = combined;
    }
}
