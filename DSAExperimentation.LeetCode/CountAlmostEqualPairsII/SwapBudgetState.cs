namespace DSAExperimentation.LeetCode.CountAlmostEqualPairsII;

// Backtrack.Search's mutable TState for LC 3267: a fixed-width digit buffer plus how
// many of the swap budget's operations remain, so Candidates alone decides what is
// legal next - stop offering swaps once the budget is spent. Answers one problem and
// nothing else, so it lives beside the solution rather than in Algorithms/ or
// Domain/ (ARCHITECTURE.md 17.3) - the same shape GenerateParentheses' own
// ParenthesesState uses.
internal sealed class SwapBudgetState(char[] digits, char[] target, int budget)
{
    public bool MatchesTarget => digits.AsSpan().SequenceEqual(target);

    private int SwapsUsed { get; set; }

    // A swap between two equal digits is never useful (it's a no-op on the string),
    // so it is pruned here rather than left for Choose/Unchoose to waste a level of
    // recursion on.
    public IEnumerable<(int I, int J)> Candidates()
    {
        if (SwapsUsed >= budget)
        {
            yield break;
        }

        for (var i = 0; i < digits.Length; i++)
        {
            for (var j = i + 1; j < digits.Length; j++)
            {
                if (digits[i] != digits[j])
                {
                    yield return (i, j);
                }
            }
        }
    }

    public void Choose((int I, int J) swap)
    {
        (digits[swap.I], digits[swap.J]) = (digits[swap.J], digits[swap.I]);
        SwapsUsed++;
    }

    public void Unchoose((int I, int J) swap)
    {
        (digits[swap.I], digits[swap.J]) = (digits[swap.J], digits[swap.I]);
        SwapsUsed--;
    }
}
