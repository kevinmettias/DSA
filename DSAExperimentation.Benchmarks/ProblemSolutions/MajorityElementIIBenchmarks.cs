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

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var majorityCount = (Length / 3) + 1;
        var values = new List<int>(Length);

        values.AddRange(Enumerable.Repeat(FirstMajorityValue, majorityCount));
        values.AddRange(Enumerable.Repeat(SecondMajorityValue, majorityCount));

        for (var i = values.Count; i < Length; i++)
        {
            values.Add(random.Next(NoiseLowerBound, NoiseUpperBound));
        }

        _nums = [.. values.OrderBy(_ => random.Next())];
    }

    [Benchmark]
    public List<int> HashMapCount() => MajorityElementIISolution.MajorityByHashMap(_nums);
}
