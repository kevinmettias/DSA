using DSAExperimentation.LeetCode.ValidParentheses;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidParenthesesSolution's, the same methods
// ValidParenthesesTests proves correct. The workload is always a properly nested string -
// the case LeetCode's own examples call valid, and the one where the stack scan actually
// has to hold a frontier - so the ratio isolates the cost of the scan itself rather than
// of an early exit on a malformed input.
public class ValidParenthesesBenchmarks
{
    private const int RandomSeed = 20; // LC problem number

    private string _brackets = string.Empty;

    [Params(64, 512)]
    public int PairCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var openers = new Stack<char>();
        var text = new List<char>(PairCount * 2);
        var pairs = new[] { "()", "[]", "{}" };

        while (text.Count < PairCount * 2)
        {
            var remaining = (PairCount * 2) - text.Count;
            var canOpen = openers.Count <= remaining - 2;
            var canClose = openers.Count >= 1;

            if (canOpen && (!canClose || random.Next(2) == 0))
            {
                var pair = pairs[random.Next(pairs.Length)];
                openers.Push(pair[1]);
                text.Add(pair[0]);
            }
            else
            {
                text.Add(openers.Pop());
            }
        }

        _brackets = new string(text.ToArray());
    }

    [Benchmark(Baseline = true)]
    public bool RepeatedPairRemoval() =>
        ValidParenthesesSolution.IsValidByRepeatedPairRemoval(_brackets);

    [Benchmark]
    public bool BracketStack() =>
        ValidParenthesesSolution.IsValidByBracketStack(_brackets);
}
