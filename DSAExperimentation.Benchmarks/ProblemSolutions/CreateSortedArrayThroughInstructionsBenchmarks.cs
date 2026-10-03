using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CreateSortedArrayThroughInstructions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CreateSortedArrayThroughInstructionsSolution's, the
// same methods CreateSortedArrayThroughInstructionsSolutionTests proves correct - the
// textbook O(n^2) rescan of everything inserted so far against the O(n log maxValue)
// sweep through this repo's own FenwickTree<int, SumOperation<int>>. The workload is
// LeetCode's own input shape, so [GlobalSetup] only has to choose a length, a value
// range and a seed.
public class CreateSortedArrayThroughInstructionsBenchmarks
{
    // LC problem number, used as the deterministic Random seed.
    private const int RandomSeed = 1649;

    private const int MaxGeneratedValueExclusive = 1_000;

    private int[] _instructions = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _instructions = SeededDraws.Values(Length, 1, MaxGeneratedValueExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScan() =>
        CreateSortedArrayThroughInstructionsSolution.CreateSortedArrayByPairwiseScan(_instructions);

    [Benchmark]
    public int FenwickTreeSweep() =>
        CreateSortedArrayThroughInstructionsSolution.CreateSortedArrayByFenwickTreeSweep(_instructions);
}
