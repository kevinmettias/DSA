using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignParkingSystem.DesignParkingSystemSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignParkingSystemSolution's, the same classes
// DesignParkingSystemTests proves correct - the naive three-field if/else dispatch
// against this repo's own HashMap<int, int> keyed by car type. Both are O(1) per call
// regardless of key-space size; the point here is confirming the HashMap-backed
// version pays no meaningful overhead over three raw fields for what is, in this
// problem, always exactly three keys. [GlobalSetup] materializes the request script so
// random generation is charged to setup rather than to the replay, and capacity is
// seeded to Calls so every AddCar succeeds, keeping both arms' per-call work identical.
[MemoryDiagnoser]
public class DesignParkingSystemBenchmarks
{
    private const int CarTypeUpperBoundExclusive = 4;
    private const int RandomSeed = 1;

    [Params(1_000, 50_000)]
    public int Calls;

    private int[] _requestedTypes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _requestedTypes = Enumerable.Range(0, Calls).Select(_ => random.Next(1, CarTypeUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ThreeFieldDispatch() => Replay(new ParkingSystemByThreeFields(Calls, Calls, Calls));

    [Benchmark]
    public int HashMapDispatch() => Replay(new ParkingSystemByHashMap(Calls, Calls, Calls));

    private int Replay(IParkingSystem system)
    {
        var accepted = 0;

        foreach (var carType in _requestedTypes)
        {
            if (system.AddCar(carType))
            {
                accepted++;
            }
        }

        return accepted;
    }
}
