using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MajorityElementII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is MajorityElementIISolution's, the same method
// MajorityElementIITests proves correct. The original benchmark's two
// [Benchmark] arms were unwired stubs (each just returned the literal 1), so
// there was nothing to preserve from them beyond the fact that this benchmark
// exists. Two disjoint majority values each fill just over a third of the array,
// with the remainder random noise disjoint from both, all shuffled together - so
// the HashMap count has to walk most of the array before either winner crosses
// the n/3 threshold.
[MemoryDiagnoser]
public class MajorityElementIIBenchmarks
{
    private const int Seed = 229;
    private const int FirstMajorityValue = -1;
    private const int SecondMajorityValue = -2;
    private const int NoiseLowerBound = 1;
    private const int NoiseUpperBound = 1_000_000;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = BuildMajoritySeededValues();
        _nums = FillNoiseAndShuffle(values, Length);
    }

    // The seeded run of each majority value: a little over a third of Length apiece.
    private List<int> BuildMajoritySeededValues()
    {
        var majorityCount = (Length / 3) + 1;
        var values = new List<int>(Length);

        var firstMajority = Enumerable.Repeat(FirstMajorityValue, majorityCount);
        var secondMajority = Enumerable.Repeat(SecondMajorityValue, majorityCount);
        values.AddRange(firstMajority);
        values.AddRange(secondMajority);

        return values;
    }

    // The remaining slots as random noise, then the whole list shuffled with draws
    // taken from the same seeded sequence the workload is pinned to.
    private static int[] FillNoiseAndShuffle(List<int> values, int length)
    {
        var random = new Random(Seed);

        for (var i = values.Count; i < length; i++)
        {
            var noise = random.Next(NoiseLowerBound, NoiseUpperBound);
            values.Add(noise);
        }

        return [.. values.OrderBy(_ => random.Next())];
    }

    [Benchmark]
    public List<int> HashMapCount() => MajorityElementIISolution.MajorityByHashMap(_nums);
}
