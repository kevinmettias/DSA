using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Queries on a Permutation With Key (LC 1409): move-to-front simulation of the
// permutation P, once over a plain BCL List<int> and once over this repo's own
// DynamicArray<int> (PermutationSequenceBenchmarks precedent for reusing
// DynamicArray as a shrinking/growing sequence). Both are O(Queries * M) -
// finding and re-inserting an element is a linear scan/shift either way - so
// this measures the repo primitive's overhead against the BCL type doing the
// identical job, not a different complexity class.
[MemoryDiagnoser]
public class QueriesOnAPermutationWithKeyBenchmarks
{
    [Params(200, 1_000)]
    public int M;

    private int[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _queries = Enumerable.Range(0, M).Select(_ => random.Next(1, M + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ListMoveToFront()
    {
        var permutation = new List<int>(M);
        for (var value = 1; value <= M; value++)
        {
            permutation.Add(value);
        }

        var lastIndex = 0;

        foreach (var query in _queries)
        {
            var index = permutation.IndexOf(query);
            lastIndex = index;
            permutation.RemoveAt(index);
            permutation.Insert(0, query);
        }

        return lastIndex;
    }

    [Benchmark]
    public int DynamicArrayMoveToFront()
    {
        var permutation = new DynamicArray<int>();
        for (var value = 1; value <= M; value++)
        {
            permutation.Add(value);
        }

        var lastIndex = 0;

        foreach (var query in _queries)
        {
            var index = IndexOf(permutation, query);
            lastIndex = index;
            permutation.RemoveAt(index);
            permutation.Insert(0, query);
        }

        return lastIndex;
    }

    private static int IndexOf(DynamicArray<int> permutation, int value)
    {
        for (var i = 0; i < permutation.Count; i++)
        {
            if (permutation.Get(i) == value)
            {
                return i;
            }
        }

        return -1;
    }
}
