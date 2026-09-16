using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheKthCharacterInStringGameII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheKthCharacterInStringGameIISolution's,
// the same methods FindTheKthCharacterInStringGameIITests proves correct.
// operations alternates 0/1 so every round still doubles word's length,
// letting k sit exactly on the final character (2^OperationCount) while
// staying small enough for the brute-force arm to remain tractable - #3307's
// own published bound (k up to 10^16) is exactly what BackwardTrace exists to
// reach without ever materializing word, which is the payoff this comparison
// is measuring.
[MemoryDiagnoser]
public class FindTheKthCharacterInStringGameIIBenchmarks
{
    private long _targetPosition;

    private int[] _operations = [];
    [Params(8, 16)]
    public int OperationCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _operations = new int[OperationCount];

        for (var i = 0; i < OperationCount; i++)
        {
            _operations[i] = i % 2;
        }

        _targetPosition = 1L << OperationCount;
    }

    [Benchmark(Baseline = true)]
    public char BruteForceSimulation() =>
        FindTheKthCharacterInStringGameIISolution.KthCharacterByBruteForceSimulation(_targetPosition, _operations);

    [Benchmark]
    public char BackwardTrace() =>
        FindTheKthCharacterInStringGameIISolution.KthCharacterByBackwardTrace(_targetPosition, _operations);
}
