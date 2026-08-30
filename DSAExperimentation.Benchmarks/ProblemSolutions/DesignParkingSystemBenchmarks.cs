using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Parking System (LC 1603): a naive three-field if/else dispatch vs. this
// repo's own HashMap<int,int> keyed by car type (DesignParkingSystemTests
// precedent). Both are O(1) per call regardless of car-type key space size - the
// point here is confirming the HashMap-backed version pays no meaningful overhead
// over three raw fields for what is, in this problem, always exactly three keys.
// Capacity is seeded to Calls so every AddCar call succeeds, keeping both variants'
// per-call work identical.
[MemoryDiagnoser]
public class DesignParkingSystemBenchmarks
{
    [Params(1_000, 50_000)]
    public int Calls;

    private int[] _requestedTypes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _requestedTypes = Enumerable.Range(0, Calls).Select(_ => random.Next(1, 4)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ThreeFieldDispatch()
    {
        var big = Calls;
        var medium = Calls;
        var small = Calls;
        var accepted = 0;

        foreach (var carType in _requestedTypes)
        {
            bool added;

            if (carType == 1)
            {
                added = big > 0;
                if (added)
                {
                    big--;
                }
            }
            else if (carType == 2)
            {
                added = medium > 0;
                if (added)
                {
                    medium--;
                }
            }
            else
            {
                added = small > 0;
                if (added)
                {
                    small--;
                }
            }

            if (added)
            {
                accepted++;
            }
        }

        return accepted;
    }

    [Benchmark]
    public int HashMapDispatch()
    {
        var remaining = new HashMap<int, int>();
        remaining.Set(1, Calls);
        remaining.Set(2, Calls);
        remaining.Set(3, Calls);
        var accepted = 0;

        foreach (var carType in _requestedTypes)
        {
            remaining.TryGetValue(carType, out var slots);

            if (slots <= 0)
            {
                continue;
            }

            remaining.Set(carType, slots - 1);
            accepted++;
        }

        return accepted;
    }
}
