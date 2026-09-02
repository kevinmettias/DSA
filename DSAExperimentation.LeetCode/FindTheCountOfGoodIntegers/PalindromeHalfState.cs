namespace DSAExperimentation.LeetCode.FindTheCountOfGoodIntegers;

// Backtrack.Search's mutable TState for LC 3272: the half of an n-digit palindrome
// being built one digit at a time, 0-9 except a leading zero at position 0. Answers
// one problem and nothing else, so it lives beside the solution rather than in
// Algorithms/ or Domain/ (ARCHITECTURE.md 17.3) - the same shape GenerateParentheses'
// own ParenthesesState uses.
internal sealed class PalindromeHalfState(int halfLength)
{
    public List<int> Digits { get; } = new(halfLength);

    public bool IsComplete => Digits.Count == halfLength;

    public IEnumerable<int> Candidates()
    {
        var lowestDigit = Digits.Count == 0 ? 1 : 0;

        for (var digit = lowestDigit; digit <= 9; digit++)
        {
            yield return digit;
        }
    }

    public void Choose(int digit) => Digits.Add(digit);

    public void Unchoose(int digit) => Digits.RemoveAt(Digits.Count - 1);
}
