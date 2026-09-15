using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ReplaceNonCoprimeNumbersInArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReplaceNonCoprimeNumbersInArraySolution's, the same
// methods ReplaceNonCoprimeNumbersInArrayTests proves correct. RepeatedFullRescan
// sweeps the whole remaining list from the front for a mergeable adjacent pair and
// restarts after every single merge; StackCascadingMerge keeps only the merged
// prefix on this repo's own Stack<long>, where cascading is just "keep peeking and
// popping the top" and no rescan is ever needed. Both take LeetCode's own int[]
// shape, which is already what [GlobalSetup] prepares, so nothing but the merge
// work is charged to the measured call.
[MemoryDiagnoser]
public class ReplaceNonCoprimeNumbersInArrayBenchmarks
{
    private const int RandomSeed = 2197;

    // Small value range so adjacent numbers frequently share a factor, forcing
    // real merge (and merge-cascade) work on every run.
    private const int MinValueInclusive = 2;
    private const int MaxValueExclusive = 10;

    private int[] _values = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(MinValueInclusive, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] RepeatedFullRescan() =>
        ReplaceNonCoprimeNumbersInArraySolution.ReplaceByRepeatedRescan(_values);

    [Benchmark]
    public int[] StackCascadingMerge() =>
        ReplaceNonCoprimeNumbersInArraySolution.ReplaceByStackCascade(_values);
}
