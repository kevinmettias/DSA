using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MajorityElement;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is MajorityElementSolution's, the same method
// MajorityElementTests proves correct. The majority value fills just over half
// the array and the rest is random noise disjoint from it, shuffled together, so
// the HashMap count has to walk most of the array before any key crosses the
// n/2 threshold.
[MemoryDiagnoser]
public class MajorityElementBenchmarks
{
    private const int Seed = 169;
    private const int MajorityValue = -1;
    private const int NoiseLowerBound = 1;
    private const int NoiseUpperBound = 1_000_000;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var majorityCount = (Length / 2) + 1;
        var values = new List<int>(Length);

        var majorityValues = Enumerable.Repeat(MajorityValue, majorityCount);
        values.AddRange(majorityValues);

        for (var i = majorityCount; i < Length; i++)
        {
            var noise = random.Next(NoiseLowerBound, NoiseUpperBound);
            values.Add(noise);
        }

        _nums = [.. values.OrderBy(_ => random.Next())];
    }

    [Benchmark]
    public int HashMapCount() => MajorityElementSolution.MajorityByHashMap(_nums);
}
