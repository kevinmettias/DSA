using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.SpiralMatrixIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SpiralMatrixIIISolution's, the same methods
// SpiralMatrixIIITests proves correct - the "defensive visited tracking" walk
// that guards every step against re-adding a cell against the direct
// growing-stride walk that trusts the strictly increasing strides never to
// revisit one and skips tracking entirely. Both visit exactly rows*cols
// in-bounds cells; the gap under [MemoryDiagnoser] is the set's per-step
// hashing/allocation overhead, not algorithm class.
//
// The whole input is four ints, so there is nothing to hoist into [GlobalSetup]
// beyond choosing them from the measured size.
[MemoryDiagnoser]
public class SpiralMatrixIIIBenchmarks
{

    private int _rows;

    private int _cols;
    private int _rStart;
    private int _cStart;
    [Params(20, 200)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _rows = Size;
        _cols = Size;
        _rStart = Size / AlgorithmConstants.HalvingFactor;
        _cStart = Size / AlgorithmConstants.HalvingFactor;
    }

    [Benchmark(Baseline = true)]
    public int[][] DirectionVectorWithVisitedSet() =>
        SpiralMatrixIIISolution.SpiralWalkByVisitedSet(_rows, _cols, _rStart, _cStart);

    [Benchmark]
    public int[][] GrowingStepDirectionWalk() =>
        SpiralMatrixIIISolution.SpiralWalkByGrowingStride(_rows, _cols, _rStart, _cStart);
}
