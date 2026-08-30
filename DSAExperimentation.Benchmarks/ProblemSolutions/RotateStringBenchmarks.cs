using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Rotate String (LC 796): the naive O(n*m) substring scan for goal inside s+s vs.
// this repo's KMP-based PrefixFunctionSearch, which never re-scans from the start
// of goal on a mismatch. _s/_goal are both runs of 'a' with one differing trailing
// character, so nearly every scan position is a long near-miss - the worst case
// for the naive scan and exactly what KMP's failure function is built to skip.
[MemoryDiagnoser]
public class RotateStringBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;
    private string _goal = null!;

    [GlobalSetup]
    public void Setup()
    {
        _s = new string('a', Length - 1) + 'b';
        _goal = new string('a', Length - 1) + 'c';
    }

    [Benchmark(Baseline = true)]
    public bool NaiveSubstringScan() => Contains(_s + _s, _goal);

    [Benchmark]
    public bool KmpSearch() => PrefixFunctionSearch.FindAll(_s + _s, _goal).Count > 0;

    private static bool Contains(string text, string pattern)
    {
        for (var i = 0; i + pattern.Length <= text.Length; i++)
        {
            var matched = true;

            for (var j = 0; j < pattern.Length; j++)
            {
                if (text[i + j] != pattern[j])
                {
                    matched = false;
                    break;
                }
            }

            if (matched)
            {
                return true;
            }
        }

        return false;
    }
}
