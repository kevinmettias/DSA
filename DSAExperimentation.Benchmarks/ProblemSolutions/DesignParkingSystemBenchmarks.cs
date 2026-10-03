using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DesignParkingSystem;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignParkingSystemSolution's, the same classes
// DesignParkingSystemSolutionTests proves correct - the naive three-field if/else dispatch
// against this repo's own HashMap<int, int> keyed by car type. Both are O(1) per call
// regardless of key-space size; the point here is confirming the HashMap-backed
// version pays no meaningful overhead over three raw fields for what is, in this
// problem, always exactly three keys. [GlobalSetup] materializes the request script so
// random generation is charged to setup rather than to the replay, and capacity is
// seeded to Calls so every AddCar succeeds, keeping both arms' per-call work identical.
// LC 1603 caps both a lot's capacity and the addCar calls at 1000, so the larger
// Calls is 1000.
public class DesignParkingSystemBenchmarks
{
    private const int CarTypeUpperBoundExclusive = 4;
    private const int RandomSeed = 1;

    private int[] _requestedTypes = [];

    [Params(100, 1_000)]
    public int Calls { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _requestedTypes = SeededDraws.Values(Calls, 1, CarTypeUpperBoundExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int ThreeFieldDispatch() => Replay(new DesignParkingSystemSolution.ParkingSystemByThreeFields(Calls, Calls, Calls));

    [Benchmark]
    public int HashMapDispatch() => Replay(new DesignParkingSystemSolution.ParkingSystemByHashMap(Calls, Calls, Calls));

    private int Replay(DesignParkingSystemSolution.IParkingSystem system)
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
