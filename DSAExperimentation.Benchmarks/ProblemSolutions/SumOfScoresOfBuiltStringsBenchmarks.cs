using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sum of Scores of Built Strings (LC 2223): the textbook worst-case O(n^2) approach
// (compare each suffix against the full string character by character) vs. this
// repo's own ZFunction.Compute over the reversed string, O(n). score(t_i) - the
// longest common prefix between s's length-i suffix and s itself - is exactly the
// Z-value at position i of reverse(s) (see SumOfScoresOfBuiltStringsTests for the
// derivation); summed for i=1..n-1, plus n for the whole string itself. A
// small alphabet is used for both benchmark inputs specifically to maximize
// self-overlap, the brute force's actual worst case (a high-entropy string lets it
// bail out of most comparisons after one character).
[MemoryDiagnoser]
public class SumOfScoresOfBuiltStringsBenchmarks
{
    private const int RandomSeed = 2223; // LC problem number
    private const int AlphabetSize = 2;

    [Params(500, 5_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _text = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public long BruteForceSuffixComparison()
    {
        var n = _text.Length;
        var total = (long)n;

        for (var i = 1; i < n; i++)
        {
            var start = n - i;
            var score = 0;

            while (score < i && _text[start + score] == _text[score])
            {
                score++;
            }

            total += score;
        }

        return total;
    }

    [Benchmark]
    public long ZFunctionOnReversed()
    {
        var reversed = new string(_text.Reverse().ToArray());
        var z = ZFunction.Compute(reversed);

        var total = (long)_text.Length;

        for (var i = 1; i < z.Length; i++)
        {
            total += z[i];
        }

        return total;
    }
}
