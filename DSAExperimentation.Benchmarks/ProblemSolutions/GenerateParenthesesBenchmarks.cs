using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class GenerateParenthesesBenchmarks
{
    [Params(5, 8)] public int Pairs;

    [Benchmark(Baseline = true)]
    public int RecursiveSpecialized()
    {
        var count = 0;
        void Search(int open, int close)
        {
            if (open == Pairs && close == Pairs) { count++; return; }
            if (open < Pairs) Search(open + 1, close);
            if (close < open) Search(open, close + 1);
        }
        Search(0, 0);
        return count;
    }

    [Benchmark]
    public int Backtracking()
    {
        var count = 0;
        var state = new ParenthesesState(Pairs);
        Backtrack.Search<ParenthesesState, char>(
            state,
            isSolution: s => s.Buffer.Count == s.TargetLength,
            candidates: s => s.Buffer.Count == s.TargetLength ? [] : s.Candidates(),
            choose: (s, c) => s.Choose(c),
            unchoose: (s, c) => s.Unchoose(c),
            onSolution: _ => count++);
        return count;
    }

    private sealed class ParenthesesState(int pairs)
    {
        public int TargetLength => pairs * 2;
        public List<char> Buffer { get; } = [];
        private int Opened { get; set; }
        private int Closed { get; set; }
        public IEnumerable<char> Candidates()
        {
            if (Opened < pairs) yield return '(';
            if (Closed < Opened) yield return ')';
        }
        public void Choose(char c) { Buffer.Add(c); if (c == '(') Opened++; else Closed++; }
        public void Unchoose(char c) { Buffer.RemoveAt(Buffer.Count - 1); if (c == '(') Opened--; else Closed--; }
    }
}
