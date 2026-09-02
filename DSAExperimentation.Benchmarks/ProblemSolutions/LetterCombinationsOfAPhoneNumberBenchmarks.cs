using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LetterCombinationsOfAPhoneNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Letter Combinations of a Phone Number (LC 17): harness only. Both arms are
// LetterCombinationsOfAPhoneNumberSolution's, the same methods
// LetterCombinationsOfAPhoneNumberTests proves correct - direct nested expansion
// vs. this repo's generic Backtrack.Search enumeration engine.
//
// Both arms now return the built combination list rather than a bare count - the
// backtracking arm previously only incremented a counter in onSolution, weaker
// than the answer LetterCombinationsByBacktracking actually produces; promoted
// here to match, the same deliberate change ARCHITECTURE.md §17.8 records for
// WordLadderII.
[MemoryDiagnoser]
public class LetterCombinationsOfAPhoneNumberBenchmarks
{
    private string _digits = null!;

    [Params(3, 5)]
    public int DigitCount;

    [GlobalSetup]
    public void Setup() => _digits = new string('7', DigitCount);

    [Benchmark(Baseline = true)]
    public List<string> IterativeExpansion() =>
        LetterCombinationsOfAPhoneNumberSolution.LetterCombinationsByIterativeExpansion(_digits);

    [Benchmark]
    public List<string> Backtracking() =>
        LetterCombinationsOfAPhoneNumberSolution.LetterCombinationsByBacktracking(_digits);
}
