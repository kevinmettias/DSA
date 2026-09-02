using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Query Kth Smallest Trimmed Number (LC 2343): a naive per-query "repeatedly scan for the current
// smallest trimmed suffix, k times" baseline (O(n*k) worst case, the approach most people reach
// for before reaching for a sort) vs. this repo's own MergeSort over an index array
// (QueryKthSmallestTrimmedNumberTests' exact composition), sorting once per query in O(n log n)
// and reading off index k-1 directly. Both share the same allocation-free trimmed-suffix
// comparison and replay the identical query batch.
[MemoryDiagnoser]
public class QueryKthSmallestTrimmedNumberBenchmarks
{
    private const int DigitLength = 5;
    private const int QueryCount = 8;
    private const int RandomSeed = 2343;

    [Params(200, 3_000)]
    public int Length;

    private string[] _nums = null!;
    private (int K, int Trim)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length)
            .Select(_ => string.Concat(Enumerable.Range(0, DigitLength).Select(_ => random.Next(0, 10))))
            .ToArray();

        _queries = Enumerable.Range(0, QueryCount)
            .Select(_ => (K: random.Next(1, Length + 1), Trim: random.Next(1, DigitLength + 1)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long SelectionScanPerQuery()
    {
        var checksum = 0L;

        foreach (var (k, trim) in _queries)
        {
            checksum += SelectionScan(k, trim);
        }

        return checksum;
    }

    [Benchmark]
    public long MergeSortPerQuery()
    {
        var checksum = 0L;

        foreach (var (k, trim) in _queries)
        {
            checksum += MergeSortAnswer(k, trim);
        }

        return checksum;
    }

    private int SelectionScan(int k, int trim)
    {
        var taken = new bool[_nums.Length];
        var answer = -1;

        for (var round = 0; round < k; round++)
        {
            var bestIndex = -1;

            for (var i = 0; i < _nums.Length; i++)
            {
                if (taken[i])
                {
                    continue;
                }

                if (bestIndex == -1 || CompareTrimmed(_nums[i], _nums[bestIndex], trim) < 0)
                {
                    bestIndex = i;
                }
            }

            taken[bestIndex] = true;
            answer = bestIndex;
        }

        return answer;
    }

    private int MergeSortAnswer(int k, int trim)
    {
        var indices = Enumerable.Range(0, _nums.Length).ToArray();
        var comparer = Comparer<int>.Create((a, b) => CompareTrimmed(_nums[a], _nums[b], trim));

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(indices), comparer);

        return indices[k - 1];
    }

    private static int CompareTrimmed(string first, string second, int trim)
    {
        var startFirst = first.Length - trim;
        var startSecond = second.Length - trim;

        for (var i = 0; i < trim; i++)
        {
            var comparison = first[startFirst + i].CompareTo(second[startSecond + i]);
            if (comparison != 0)
            {
                return comparison;
            }
        }

        return 0;
    }
}
