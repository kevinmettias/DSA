using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Letter Combinations of a Phone Number (LC 17): direct nested expansion vs. this
// repo's generic Backtrack.Search enumeration engine.
[MemoryDiagnoser]
public class LetterCombinationsOfAPhoneNumberBenchmarks
{
    private string _digits = null!;

    [Params(3, 5)]
    public int DigitCount;

    [GlobalSetup]
    public void Setup() => _digits = new string('7', DigitCount);

    [Benchmark(Baseline = true)]
    public int IterativeExpansion()
    {
        var results = new List<string> { string.Empty };

        foreach (var digit in _digits)
        {
            var next = new List<string>();
            foreach (var prefix in results)
            {
                foreach (var letter in LettersFor(digit))
                {
                    next.Add(prefix + letter);
                }
            }

            results = next;
        }

        return results.Count;
    }

    [Benchmark]
    public int Backtracking()
    {
        var count = 0;
        var state = new CombinationState();

        Backtrack.Search<CombinationState, char>(
            state,
            isSolution: s => s.Index == _digits.Length,
            candidates: s => s.Index == _digits.Length ? [] : LettersFor(_digits[s.Index]),
            choose: (s, letter) =>
            {
                s.Chosen.Add(letter);
                s.Index++;
            },
            unchoose: (s, _) =>
            {
                s.Index--;
                s.Chosen.RemoveAt(s.Chosen.Count - 1);
            },
            onSolution: _ => count++);

        return count;
    }

    private static string LettersFor(char digit)
        => digit switch
        {
            '2' => "abc",
            '3' => "def",
            '4' => "ghi",
            '5' => "jkl",
            '6' => "mno",
            '7' => "pqrs",
            '8' => "tuv",
            '9' => "wxyz",
            _ => string.Empty,
        };

    private sealed class CombinationState
    {
        public List<char> Chosen { get; } = [];

        public int Index { get; set; }
    }
}

