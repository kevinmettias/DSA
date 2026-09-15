using System.Text;
using System.Text.RegularExpressions;

namespace DSAExperimentation.LeetCode.CamelcaseMatching;

// LeetCode 1023. Camelcase Matching: a query matches the pattern when the pattern is
// a subsequence of it AND every uppercase letter the query contributes is one the
// pattern asked for - lowercase letters are the only characters a query may insert.
//
// That rule is exactly "[a-z]* spliced between every pattern character, anchored over
// the whole query", so the two strategies are the regex engine's backtracking state
// machine against a direct two-pointer scan. No repo data structure is involved
// either way - the same "nothing to compose" shape ValidPalindrome has.
internal static class CamelcaseMatchingSolution
{
    // Any run of lowercase letters, spliced between and around the pattern's characters.
    private const string LowercaseRun = "[a-z]*";

    // \A..\z, not ^..$: the pattern has to cover the WHOLE query. An unanchored
    // search would report "FooBarTest" as matching "FB" by ignoring the trailing
    // "Test", which is not LC 1023's answer.
    private const string StartOfInput = @"\A";
    private const string EndOfInput = @"\z";

    // The textbook shortcut: hand the whole rule to the BCL regex engine. Deliberately
    // written without this repo's primitives - it is the arm
    // CamelMatchByTwoPointerScan has to justify itself against.
    public static bool[] CamelMatchByRegex(string[] queries, string pattern) =>
        CamelMatchByRegex(queries, BuildMatcher(pattern));

    // Prepared-input overload: the matcher is compiled once by the caller, so a
    // benchmark charges regex construction to its setup rather than to the scan.
    public static bool[] CamelMatchByRegex(string[] queries, Regex matcher)
    {
        var results = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            results[i] = matcher.IsMatch(queries[i]);
        }

        return results;
    }

    // Splices a lowercase run before, between and after every pattern character and
    // anchors the result over the whole query.
    public static Regex BuildMatcher(string pattern)
    {
        var expression = new StringBuilder(StartOfInput);

        foreach (var c in pattern)
        {
            expression.Append(LowercaseRun).Append(Regex.Escape(c.ToString()));
        }

        return new Regex(expression.Append(LowercaseRun).Append(EndOfInput).ToString());
    }

    // One index walk per query: consume the next pattern character whenever it lines
    // up, skip lowercase letters that do not, and reject on any uppercase letter the
    // pattern did not ask for. Linear in the query with no engine overhead and no
    // allocation - the same per-item-scan shape NumberOfMatchingSubsequences uses.
    public static bool[] CamelMatchByTwoPointerScan(string[] queries, string pattern)
    {
        var results = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            results[i] = IsCamelMatch(new CamelQuery(queries[i]), new CamelPattern(pattern));
        }

        return results;
    }

    private static bool IsCamelMatch(CamelQuery query, CamelPattern pattern)
    {
        var p = 0;

        foreach (var c in query.Text)
        {
            if (p < pattern.Text.Length && pattern.Text[p] == c)
            {
                p++;
                continue;
            }

            if (char.IsUpper(c))
            {
                return false;
            }
        }

        return p == pattern.Text.Length;
    }
}
