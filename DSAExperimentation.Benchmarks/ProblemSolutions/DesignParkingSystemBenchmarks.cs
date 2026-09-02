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
    private const int CarTypeUpperBoundExclusive = 4;
    private const int MediumCarType = 2;
    private const int SmallCarType = 3;

    [Params(1_000, 50_000)]
    public int Calls;

    private int[] _requestedTypes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _requestedTypes = Enumerable.Range(0, Calls).Select(_ => random.Next(1, CarTypeUpperBoundExclusive)).ToArray();
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
            if (TryAdmit(carType, ref big, ref medium, ref small))
            {
                accepted++;
            }
        }

        return accepted;
    }

    private static bool TryAdmit(int carType, ref int big, ref int medium, ref int small)
    {
        if (carType == 1)
        {
            return TryTakeSlot(ref big);
        }

        if (carType == MediumCarType)
        {
            return TryTakeSlot(ref medium);
        }

        return TryTakeSlot(ref small);
    }

    private static bool TryTakeSlot(ref int remaining)
    {
        if (remaining <= 0)
        {
            return false;
        }

        remaining--;
        return true;
    }

    [Benchmark]
    public int HashMapDispatch()
    {
        var remaining = new HashMap<int, int>();
        remaining.Set(1, Calls);
        remaining.Set(MediumCarType, Calls);
        remaining.Set(SmallCarType, Calls);
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
