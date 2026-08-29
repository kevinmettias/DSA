using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GenerateParentheses;

// LeetCode 22. Generate Parentheses: Backtrack.Search owns the choose/explore/
// unchoose recursion while the state enforces the open/close counts.
public sealed partial class GenerateParenthesesTests
{
    [Fact]
    public void Generate_NEqualsThree_ReturnsFiveBalancedStrings()
    {
        var generated = Generate(3);

        Assert.Equal(5, generated.Count);
        Assert.Contains("((()))", generated);
        Assert.Contains("(()())", generated);
        Assert.Contains("(())()", generated);
        Assert.Contains("()(())", generated);
        Assert.Contains("()()()", generated);
    }

    private static List<string> Generate(int n)
    {
        var results = new List<string>();
        var state = new ParenthesesState(n);

        Backtrack.Search<ParenthesesState, char>(
            state,
            isSolution: s => s.Buffer.Count == s.TargetLength,
            candidates: s => s.Buffer.Count == s.TargetLength ? [] : s.Candidates(),
            choose: (s, c) => s.Choose(c),
            unchoose: (s, c) => s.Unchoose(c),
            onSolution: s => results.Add(new string(s.Buffer.ToArray())));

        return results;
    }

    private sealed class ParenthesesState(int pairs)
    {
        public int TargetLength => pairs * 2;

        public List<char> Buffer { get; } = [];

        private int Opened { get; set; }

        private int Closed { get; set; }

        public IEnumerable<char> Candidates()
        {
            if (Opened < pairs)
            {
                yield return '(';
            }

            if (Closed < Opened)
            {
                yield return ')';
            }
        }

        public void Choose(char c)
        {
            Buffer.Add(c);
            if (c == '(') Opened++; else Closed++;
        }

        public void Unchoose(char c)
        {
            Buffer.RemoveAt(Buffer.Count - 1);
            if (c == '(') Opened--; else Closed--;
        }
    }
}
