using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Distinct Echo Substrings (LC 1316): the textbook double loop over every
// (start, halfLength) pair, extracting both halves as real substrings and
// comparing them with == (paying O(halfLength) on every single pair, match or
// not) - vs. this repo's own RollingHash as an O(1) equality screen per pair,
// only paying for a real SequenceEqual once the screen passes, the same
// screen-then-verify shape RollingHashSearch.FindAll/
// LongestChunkedPalindromeDecompositionBenchmarks already use. Both strategies
// dedupe found echoes into a set keyed by the substring's text - the baseline uses
// a plain BCL HashSet<string>, the primitive-composed version this repo's own
// Set<string> (HashMap-backed), the same DistinctEchoSubstringsTests composition.
// _text is random lowercase letters, so most candidate pairs fail fast - exactly
// the shape where RollingHash's O(1) screen avoids the baseline's per-pair
// substring allocation instead of merely relocating the same cost.
[MemoryDiagnoser]
public class DistinctEchoSubstringsBenchmarks
{
    [Params(80, 400)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1316);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(26));
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int NaiveSubstringComparison()
    {
        var echoes = new HashSet<string>();

        for (var halfLength = 1; halfLength * 2 <= _text.Length; halfLength++)
        {
            for (var start = 0; start + (2 * halfLength) <= _text.Length; start++)
            {
                var first = _text.Substring(start, halfLength);
                var second = _text.Substring(start + halfLength, halfLength);

                if (first == second)
                {
                    echoes.Add(_text.Substring(start, halfLength * 2));
                }
            }
        }

        return echoes.Count;
    }

    [Benchmark]
    public int RollingHashScreenedEchoCount()
    {
        var hash = new RollingHash(_text);
        var echoes = new Set<string>();

        for (var halfLength = 1; halfLength * 2 <= _text.Length; halfLength++)
        {
            for (var start = 0; start + (2 * halfLength) <= _text.Length; start++)
            {
                if (hash.Hash(start, halfLength) == hash.Hash(start + halfLength, halfLength)
                    && _text.AsSpan(start, halfLength).SequenceEqual(_text.AsSpan(start + halfLength, halfLength)))
                {
                    echoes.TryAdd(_text.Substring(start, halfLength * 2));
                }
            }
        }

        return echoes.Count;
    }
}
