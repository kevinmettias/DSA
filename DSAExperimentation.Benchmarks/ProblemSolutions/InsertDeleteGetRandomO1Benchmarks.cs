using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Insert Delete GetRandom O(1) (LC 380): a plain List<int> baseline (Contains-scan on
// every Insert, an indexed Remove that shifts every element after the removed slot) vs.
// this repo's HashMap<int,int> (value -> index) + DynamicArray<int> composition, whose
// Remove swaps the removed slot with the tail before popping it - DynamicArray.RemoveAt
// only ever runs on the LAST index, its O(1) path, never the O(n) shifting one.
[MemoryDiagnoser]
public class InsertDeleteGetRandomO1Benchmarks
{
    [Params(200, 20_000)]
    public int Count;

    private int[] _insertOrder = null!;
    private int[] _removalOrder = null!;

    [GlobalSetup]
    public void Setup()
    {
        _insertOrder = Enumerable.Range(0, Count).ToArray();

        var random = new Random(1);
        _removalOrder = _insertOrder.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ListBased()
    {
        var values = new List<int>();

        foreach (var value in _insertOrder)
        {
            if (!values.Contains(value))
            {
                values.Add(value);
            }
        }

        foreach (var value in _removalOrder)
        {
            values.Remove(value);
        }

        return values.Count;
    }

    [Benchmark]
    public int HashMapDynamicArrayComposed()
    {
        var indexByValue = new HashMap<int, int>();
        var values = new DynamicArray<int>();

        foreach (var value in _insertOrder)
        {
            if (indexByValue.HasKey(value))
            {
                continue;
            }

            values.Add(value);
            indexByValue.Set(value, values.Count - 1);
        }

        foreach (var value in _removalOrder)
        {
            if (!indexByValue.TryGetValue(value, out var index))
            {
                continue;
            }

            var lastIndex = values.Count - 1;
            var lastValue = values.Get(lastIndex);

            values.Set(index, lastValue);
            indexByValue.Set(lastValue, index);

            values.RemoveAt(lastIndex);
            indexByValue.TryRemove(value);
        }

        return values.Count;
    }
}
