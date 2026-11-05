namespace DSAExperimentation.LeetCode.Solutions.BasicCalculatorIV;

// LC 770's promise about its input: the expression consists of lowercase English letters, digits,
// '+', '-', '*', '(', ')' and ' ', so it never divides. LeetCode's judge never poses an input that
// breaks it. Both strategies read a '/' as no operator at all. Generated cases do not always keep the
// promise, so a harness that runs them asks this first.
internal static class BasicCalculatorIVPromise
{
    // The characters beside letters and digits that an expression may hold.
    private const string Symbols = "+-*() ";

    public static bool IsKeptBy(string expression, string[] evalvars, int[] evalints)
        => expression.All(character => char.IsAsciiLetterLower(character) || char.IsAsciiDigit(character) || Symbols.Contains(character));
}
