using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FormArrayByConcatenatingSubarraysOfAnotherArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// FormArrayByConcatenatingSubarraysOfAnotherArraySolution's, the same methods
// FormArrayByConcatenatingSubarraysOfAnotherArrayTests proves correct.
// NaiveSubarrayScan re-compares from scratch at every start position, O(n*m),
// against compressing both int arrays into a shared char alphabet (this repo's own
// HashMap<int,char>) and searching with PrefixFunctionSearch (KMP), O(n+m)
// guaranteed. nums is almost entirely zeros with a single distinguishing 1 at the
// very end, and the single group is the same shape at half the length - the classic
// KMP-worst-case adversarial input, since the naive scan re-walks almost the whole
// group at nearly every start position before failing on its last element.
[MemoryDiagnoser]
public class FormArrayByConcatenatingSubarraysOfAnotherArrayBenchmarks
{
    private const int GroupLengthDivisor = 2; private int[] _nums = [];

    private int[][] _groups = [];
    // the group is built at half the length of nums

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _nums = BuildAlmostAllZeros(Length);
        _groups = [BuildAlmostAllZeros(Length / GroupLengthDivisor)];
    }

    private static int[] BuildAlmostAllZeros(int length)
    {
        var values = new int[length];
        values[^1] = 1;
        return values;
    }

    [Benchmark(Baseline = true)]
    public bool NaiveSubarrayScan()
        => FormArrayByConcatenatingSubarraysOfAnotherArraySolution.CanChooseByNaiveSubarrayScan(_groups, _nums);

    [Benchmark]
    public bool CharCompressedKmpSearch()
        => FormArrayByConcatenatingSubarraysOfAnotherArraySolution.CanChooseByCharCompressedKmpSearch(_groups, _nums);
}
