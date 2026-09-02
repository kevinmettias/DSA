using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Kth Largest XOR Coordinate Value (LC 1738): both approaches share the
// same O(rows*cols) 2D prefix-XOR pass and differ only in how they pick the
// kth largest value out of it afterward - a full O(nm log nm) sort-then-index
// vs. an O(nm log k) size-k min-heap (this repo's own
// Heap<int,MinHeapOrder<int>>, the KthLargestElement precedent), discarding
// the smaller root whenever a bigger candidate arrives so its own log factor
// is on K, not on the coordinate count.
[MemoryDiagnoser]
public class FindKthLargestXorCoordinateValueBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 1738; // LC problem number
    private const int MaxCoordinateValueExclusive = 1_000_000;

    [Params(50, 300)]
    public int Side;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _matrix = Enumerable.Range(0, Side)
            .Select(_ => Enumerable.Range(0, Side).Select(_ => random.Next(1, MaxCoordinateValueExclusive)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullSort()
    {
        var values = PrefixXorValues();
        Array.Sort(values);
        return values[^K];
    }

    [Benchmark]
    public int SizeKMinHeap()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var prefixXor = new int[rows + 1, cols + 1];
        var heap = new Heap<int, MinHeapOrder<int>>();

        for (var r = 1; r <= rows; r++)
        {
            for (var c = 1; c <= cols; c++)
            {
                prefixXor[r, c] = _matrix[r - 1][c - 1] ^ prefixXor[r - 1, c] ^ prefixXor[r, c - 1] ^ prefixXor[r - 1, c - 1];

                heap.Push(prefixXor[r, c]);

                if (heap.Count > K)
                {
                    heap.TryPop(out _);
                }
            }
        }

        heap.TryPeek(out var kthLargest);
        return kthLargest;
    }

    private int[] PrefixXorValues()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var prefixXor = new int[rows + 1, cols + 1];
        var values = new int[rows * cols];
        var index = 0;

        for (var r = 1; r <= rows; r++)
        {
            for (var c = 1; c <= cols; c++)
            {
                prefixXor[r, c] = _matrix[r - 1][c - 1] ^ prefixXor[r - 1, c] ^ prefixXor[r, c - 1] ^ prefixXor[r - 1, c - 1];
                values[index++] = prefixXor[r, c];
            }
        }

        return values;
    }
}
