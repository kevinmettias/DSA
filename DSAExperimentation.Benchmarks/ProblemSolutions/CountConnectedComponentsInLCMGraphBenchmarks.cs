using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountConnectedComponentsInLCMGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountConnectedComponentsInLCMGraphSolution's, the
// same methods CountConnectedComponentsInLCMGraphTests proves correct. nums is a
// deterministic set of distinct values in [1, Threshold] so both arms actually
// have work to connect, rather than the pairwise scan degenerating to n isolated
// components.
[MemoryDiagnoser]
public class CountConnectedComponentsInLCMGraphBenchmarks
{
    private const int Threshold = 2_000;
    private const int Seed = 3378;

    private int[] _nums = [];

    [Params(50, 400)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var values = new HashSet<int>();

        while (values.Count < Length)
        {
            var candidate = random.Next(1, Threshold + 1);
            values.Add(candidate);
        }

        _nums = values.ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseLcmScan() =>
        CountConnectedComponentsInLCMGraphSolution.CountComponentsByPairwiseLcmScan(_nums, Threshold);

    [Benchmark]
    public int MultipleUnion() =>
        CountConnectedComponentsInLCMGraphSolution.CountComponentsByMultipleUnion(_nums, Threshold);
}
