using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumNumberOfDartsInsideOfACircularDartboard;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfDartsInsideOfACircularDartboardSolution's,
// the same methods MaximumNumberOfDartsInsideOfACircularDartboardSolutionTests proves correct.
// They run identical O(n^3) candidate-center geometry and differ only in the buffer the
// candidates are grown in - a plain BCL List<(double, double)> against this repo's own
// DynamicArray<(double, double)>. The dart cloud (MaximumNumberOfDartsWorkloads, every
// dart unique as LC 1453 promises) is generated once in [GlobalSetup].
public class MaximumNumberOfDartsInsideOfACircularDartboardBenchmarks
{
    private const int Radius = 50;
    private const int RandomSeed = 1453; // LC problem number
    private const int CoordinateRange = 100;

    private int[][] _darts = [];

    [Params(20, 60)]
    public int DartCount { get; set; }

    [GlobalSetup]
    public void Setup() => _darts = MaximumNumberOfDartsWorkloads.BuildDarts(DartCount, CoordinateRange, RandomSeed);

    [Benchmark(Baseline = true)]
    public int PairwiseCandidateCentersWithList() =>
        MaximumNumberOfDartsInsideOfACircularDartboardSolution.MaxDartsByListCandidates(_darts, Radius);

    [Benchmark]
    public int PairwiseCandidateCentersWithDynamicArray() =>
        MaximumNumberOfDartsInsideOfACircularDartboardSolution.MaxDartsByDynamicArrayCandidates(_darts, Radius);
}
