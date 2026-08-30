using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum XOR With an Element From Array (LC 1707): a per-query linear scan
// baseline (for every query, scan every num <= mi and XOR it against xi) vs.
// this repo's own offline sweep - MergeSort (Algorithms/Sorting/MergeSort.cs)
// orders nums and query indices by limit, then this repo's own BitTrie
// (DataStructures/Graph/Engines/Dags/Trees/BitTrie.cs - the same MaxXor-family
// primitive MaximumXOROfTwoNumbersInAnArrayBenchmarks already composes)
// answers each query in O(32) once its eligible nums have been inserted.
// O(n*q) baseline vs. O((n+q) log(n+q)) offline.
[MemoryDiagnoser]
public class MaximumXORWithAnElementFromArrayBenchmarks
{
    [Params(200, 3_000)]
    public int Length;

    private int[] _nums = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1707);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, 1_000_000)).ToArray();
        _queries = Enumerable.Range(0, Length)
            .Select(_ => new[] { random.Next(0, 1_000_000), random.Next(0, 1_000_000) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] LinearScanPerQuery()
    {
        var answers = new int[_queries.Length];

        for (var i = 0; i < _queries.Length; i++)
        {
            var xi = _queries[i][0];
            var mi = _queries[i][1];
            var best = -1;

            foreach (var num in _nums)
            {
                if (num <= mi)
                {
                    best = Math.Max(best, xi ^ num);
                }
            }

            answers[i] = best;
        }

        return answers;
    }

    [Benchmark]
    public int[] OfflineSortedBitTrieSweep()
    {
        var sortedNums = _nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedNums));

        var queryOrder = Enumerable.Range(0, _queries.Length).ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(
            new ArrayIndexedSequence<int>(queryOrder),
            Comparer<int>.Create((a, b) => _queries[a][1].CompareTo(_queries[b][1])));

        var answers = new int[_queries.Length];
        var trie = new BitTrie();
        var numIndex = 0;

        foreach (var queryIndex in queryOrder)
        {
            var xi = _queries[queryIndex][0];
            var mi = _queries[queryIndex][1];

            while (numIndex < sortedNums.Length && sortedNums[numIndex] <= mi)
            {
                trie.Insert(sortedNums[numIndex]);
                numIndex++;
            }

            answers[queryIndex] = trie.TryMaxXor(xi, out var candidate) ? candidate : -1;
        }

        return answers;
    }
}
