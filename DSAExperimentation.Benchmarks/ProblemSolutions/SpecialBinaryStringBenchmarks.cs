using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SpecialBinaryString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SpecialBinaryStringSolution's, the same methods
// SpecialBinaryStringSolutionTests proves correct. PairCount controls how many special
// substrings (1/0 pairs) the generated input packs in, scaling both recursion depth
// and per-level sort width. PairCount stops at 25, whose 50 characters are LC 761's
// longest string.
public class SpecialBinaryStringBenchmarks
{
    private const int RandomSeed = 761;

    private string _input = "";

    [Params(5, 25)]
    public int PairCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _input = SpecialBinaryStringWorkloads.GenerateSpecial(PairCount, random);
    }

    [Benchmark(Baseline = true)]
    public string ArraySort() => SpecialBinaryStringSolution.MakeLargestSpecialByArraySort(_input);

    [Benchmark]
    public string MergeSort() => SpecialBinaryStringSolution.MakeLargestSpecialByMergeSort(_input);
}
