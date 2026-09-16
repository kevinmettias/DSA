namespace DSAExperimentation.LeetCode.GenerateParentheses;

// Backtrack.Search's mutable TState for LC 22: tracks how many '(' and ')' have been
// placed so Candidates alone can decide what is legal next - never more opens than
// pairs, never more closes than opens already placed. This shape answers one problem
// and nothing else, so it lives beside the solution rather than in Algorithms/ or
// Domain/ (ARCHITECTURE.md 17.3).
internal sealed class ParenthesesState(int pairs)
{
    private int Opened { get; set; }

    private int Closed { get; set; }

    public int TargetLength => pairs * 2;

    public List<char> Buffer { get; } = [];

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

    public void Choose(char parenthesis)
    {
        Buffer.Add(parenthesis);

        if (parenthesis == '(')
        {
            Opened++;
        }
        else
        {
            Closed++;
        }
    }

    public void Unchoose(char parenthesis)
    {
        Buffer.RemoveAt(Buffer.Count - 1);

        if (parenthesis == '(')
        {
            Opened--;
        }
        else
        {
            Closed--;
        }
    }
}
