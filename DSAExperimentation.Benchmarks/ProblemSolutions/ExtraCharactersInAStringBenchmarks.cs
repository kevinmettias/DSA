using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ExtraCharactersInAString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ExtraCharactersInAStringSolution's, the same methods
// ExtraCharactersInAStringTests proves correct. [GlobalSetup] builds a repeating run of
// characters that never form a dictionary word, so neither arm gets an early exact-match
// shortcut - both are forced through their full per-start scan strategy, which is
// exactly the difference being measured: a full-length substring sweep against a trie
// walk that stops at the first dead prefix.
[MemoryDiagnoser]
public class ExtraCharactersInAStringBenchmarks
{
    private static readonly string[] Dictionary = ["ab", "cd", "ef", "gh", "ij"];

    [Params(300, 1_500)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('k' + i % 5);
        }

        _s = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int HashSetFullScan() =>
        ExtraCharactersInAStringSolution.MinExtraCharsByHashSetFullScan(_s, Dictionary);

    [Benchmark]
    public int TriePrunedScan() =>
        ExtraCharactersInAStringSolution.MinExtraCharsByTriePrunedScan(_s, Dictionary);
}
