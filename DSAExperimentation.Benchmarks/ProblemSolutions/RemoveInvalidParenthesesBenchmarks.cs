using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.RemoveInvalidParentheses;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveInvalidParenthesesSolution's, the same
// methods RemoveInvalidParenthesesTests proves correct. [Length] stays small
// since the brute-force arm is genuinely O(2^n * n); the generated input always
// carries exactly 2 unmatched leading '(' characters, so both strategies do
// real removal work.
[MemoryDiagnoser]
public class RemoveInvalidParenthesesBenchmarks
{
    // Splits Length in half to build the generated input's opener/closer counts.

    private string _input = "";

    [Params(14, 20)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _input = new string('(', (Length / AlgorithmConstants.HalvingFactor) + 1) + new string(')', (Length / AlgorithmConstants.HalvingFactor) - 1);

    [Benchmark(Baseline = true)]
    public List<string> BruteForceAllSubsets() => RemoveInvalidParenthesesSolution.RemoveByBruteForceAllSubsets(_input);

    [Benchmark]
    public List<string> QueueBfsMinimalRemoval() => RemoveInvalidParenthesesSolution.RemoveByQueueBfs(_input);
}
