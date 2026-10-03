using DSAExperimentation.LeetCode.LetterCombinationsOfAPhoneNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Letter Combinations of a Phone Number (LC 17): harness only. Both arms are
// LetterCombinationsOfAPhoneNumberSolution's, the same methods
// LetterCombinationsOfAPhoneNumberSolutionTests proves correct - direct nested expansion
// vs. this repo's generic Backtrack.Search enumeration engine.
//
// Both arms now return the built combination list rather than a bare count - the
// backtracking arm previously only incremented a counter in onSolution, weaker
// than the answer LetterCombinationsByBacktracking actually produces; promoted
// here to match, the same deliberate change ARCHITECTURE.md §17.8 records for
// WordLadderII.
//
// DigitCount stops at LC 17's 4-digit cap; each '7' holds four letters, so the
// answer still grows fourfold from one size to the next.
public class LetterCombinationsOfAPhoneNumberBenchmarks
{
    private string _digits = "";

    [Params(3, 4)]
    public int DigitCount { get; set; }

    [GlobalSetup]
    public void Setup() => _digits = new string('7', DigitCount);

    [Benchmark(Baseline = true)]
    public List<string> IterativeExpansion() =>
        LetterCombinationsOfAPhoneNumberSolution.LetterCombinationsByIterativeExpansion(_digits);

    [Benchmark]
    public List<string> Backtracking() =>
        LetterCombinationsOfAPhoneNumberSolution.LetterCombinationsByBacktracking(_digits);
}
