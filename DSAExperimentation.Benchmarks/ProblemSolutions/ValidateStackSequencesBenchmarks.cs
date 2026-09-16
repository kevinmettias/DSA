using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ValidateStackSequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidateStackSequencesSolution's, the same methods
// ValidateStackSequencesTests proves correct - exhaustive push/pop-timing
// backtracking (O(2^n) worst case, undoing a branch on failure) against the single
// O(n) greedy sweep through this repo's own Stack<int>. The pushed/popped pair is a
// genuinely valid one, built once in [GlobalSetup], so the backtracking arm has to
// search rather than fail fast.
[MemoryDiagnoser]
public class ValidateStackSequencesBenchmarks
{
    // LC problem number, reused as the deterministic interleaving seed.
    private const int RandomSeed = 946;

    private int[] _pushed = [];

    private int[] _popped = [];
    [Params(10, 16)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _pushed = StackSequenceWorkloads.BuildPushed(Length);
        _popped = StackSequenceWorkloads.BuildValidPopOrder(_pushed, RandomSeed);
    }

    [Benchmark(Baseline = true)]
    public bool IsValidByBacktrackingSearch() =>
        ValidateStackSequencesSolution.IsValidByBacktrackingSearch(_pushed, _popped);

    [Benchmark]
    public bool IsValidByGreedyStackSweep() =>
        ValidateStackSequencesSolution.IsValidByGreedyStackSweep(_pushed, _popped);
}
