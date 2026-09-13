using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfStepsToReduceANumberInBinaryRepresentationToOne;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// NumberOfStepsToReduceANumberInBinaryRepresentationToOneSolution's, the same methods
// NumberOfStepsToReduceANumberInBinaryRepresentationToOneTests proves correct - the
// brute-force simulation that performs every "+1" step as a full LSB-first binary
// addition, rebuilding the whole remaining string, against the O(n) single-pass
// carry-propagation scan. An alternating "10" bit pattern maximizes how many separate
// full-length additions the baseline has to make (each digit flips the parity again
// instead of one long carry chain absorbing several steps into a single call), so the
// O(n) work per addition really does recur roughly n/2 times.
[MemoryDiagnoser]
public class NumberOfStepsToReduceANumberInBinaryRepresentationToOneBenchmarks
{
    private const string AlternatingBitUnit = "10";
    private const int AlternatingBitUnitLength = 2;

    [Params(100, 1_000)]
    public int Length;

    private string _binary = null!;

    [GlobalSetup]
    public void Setup()
    {
        var repeatedUnits = Enumerable.Repeat(AlternatingBitUnit, Length / AlternatingBitUnitLength);
        _binary = string.Concat(repeatedUnits);
    }

    [Benchmark(Baseline = true)]
    public int StackAddSimulation() =>
        NumberOfStepsToReduceANumberInBinaryRepresentationToOneSolution
            .NumStepsByStackAddSimulation(_binary);

    [Benchmark]
    public int CarryPropagationScan() =>
        NumberOfStepsToReduceANumberInBinaryRepresentationToOneSolution
            .NumStepsByCarryPropagationScan(_binary);
}
