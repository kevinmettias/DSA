using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Random Pick Index (LC 398): reservoir sampling re-scans the whole array on every
// single Pick call (the textbook no-extra-memory baseline) vs. this repo's
// HashMap<int, DynamicArray<int>> - build the value -> indices grouping once, then
// answer every Pick with one O(1)-expected lookup and a single uniform draw over that
// value's own index list. Both strategies use the same seeded Random sequence so
// neither benefits from a luckier draw order.
[MemoryDiagnoser]
public class RandomPickIndexBenchmarks
{
    private const int Target = 7;
    private const int PickCalls = 500;

    [Params(2_000, 50_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, 50)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long ReservoirSamplingPerPick()
    {
        var random = new Random(1);
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            var seenCount = 0;
            var chosen = -1;

            for (var i = 0; i < _nums.Length; i++)
            {
                if (_nums[i] != Target)
                {
                    continue;
                }

                seenCount++;
                if (random.Next(seenCount) == 0)
                {
                    chosen = i;
                }
            }

            total += chosen;
        }

        return total;
    }

    [Benchmark]
    public long HashMapDynamicArrayIndexed()
    {
        var indicesByValue = new HashMap<int, DynamicArray<int>>();

        for (var i = 0; i < _nums.Length; i++)
        {
            if (!indicesByValue.TryGetValue(_nums[i], out var indices))
            {
                indices = new DynamicArray<int>();
                indicesByValue.Set(_nums[i], indices);
            }

            indices.Add(i);
        }

        indicesByValue.TryGetValue(Target, out var targetIndices);

        var random = new Random(1);
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            total += targetIndices.Get(random.Next(targetIndices.Count));
        }

        return total;
    }
}
