using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TrafficSignalColor;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is TrafficSignalColorSolution's, the same method
// TrafficSignalColorTests proves correct. [Params] sweeps one representative
// timer value from each of LC's own ranges (green, orange, red and invalid),
// mirroring SqrtXBenchmarks' precedent for a fixed-value sweep over an O(1)
// operation.
[MemoryDiagnoser]
public class TrafficSignalColorBenchmarks
{
    [Params(0, 30, 60, 1000)]
    public int Timer;

    [Benchmark(Baseline = true)]
    public string RangeCheck() => TrafficSignalColorSolution.ColorByRangeCheck(Timer);
}
