using System.Net;
using System.Text.RegularExpressions;

namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode;

// What the catalog reads out of a question's HTML statement, because LeetCode's API has no field
// for it: each example's stated input and output, and the constraint lines. The stated input is
// read only to confirm the example it belongs to (StatedExample).
//
// LeetCode writes examples in two layouts. Older questions use a <pre> block ("<strong>Output:</strong>
// [0,1]\n<strong>Explanation:</strong>..."), newer ones a <div class="example-block"> with each
// value in a <span class="example-io">. InputLabel and OutputLabel match the label in either. An
// input runs to the next label or the block's end, and an output's value runs
// to whatever closes it - the next label, a line that opens an explanation without one, </pre>,
// </p> or </div> - then loses its tags. A value can span lines, as a list of lists often does, so
// a bare line break does not end it.
internal static partial class StatementText
{
    private const string Superscript = "^$1";
    private const string InputGroup = "input";
    private const string OutputGroup = "output";
    private const string ItemsGroup = "items";
    private const string ItemGroup = "item";
    private const string SingleSpace = " ";
    private const char NoBreakSpace = '\u00a0';
    private const string ZeroWidthSpace = "\u200b";

    // Each example's input as the statement writes it, such as "nums = [2,7,11,15], target = 9".
    public static IReadOnlyList<string> Inputs(string contentHtml)
        => InputLabel().Matches(contentHtml)
            .Select(match => PlainText(match.Groups[InputGroup].Value))
            .ToList();

    [GeneratedRegex(@"Input:?(?:\s|&nbsp;)*</(?:strong|b)>\s*(?<input>.*?)\s*(?=\n\s*<(?:strong|b)>|</pre>|</p>|</div>)", RegexOptions.Singleline)]
    private static partial Regex InputLabel();

    public static IReadOnlyList<string> Outputs(string contentHtml)
        => OutputLabel().Matches(contentHtml)
            .Select(match => PlainText(match.Groups[OutputGroup].Value))
            .ToList();

    [GeneratedRegex(@"Output:?(?:\s|&nbsp;)*</(?:strong|b)>\s*(?<output>.*?)\s*(?=\n\s*(?:<(?:strong|b)>|Explanation)|</pre>|</p>|</div>)", RegexOptions.Singleline)]
    private static partial Regex OutputLabel();

    public static IReadOnlyList<string> Constraints(string contentHtml)
    {
        var block = ConstraintsBlock().Match(contentHtml);

        if (!block.Success)
        {
            return [];
        }

        return ListItem().Matches(block.Groups[ItemsGroup].Value)
            .Select(item => ConstraintLine(item.Groups[ItemGroup].Value))
            .Where(line => line.Length > 0)
            .ToList();
    }

    [GeneratedRegex(@"Constraints:\s*</(?:strong|b)>.*?<ul>(?<items>.*?)</ul>", RegexOptions.Singleline)]
    private static partial Regex ConstraintsBlock();

    [GeneratedRegex(@"<li>(?<item>.*?)</li>", RegexOptions.Singleline)]
    private static partial Regex ListItem();

    // A power is written as a superscript, which plain text would flatten into the base's digits.
    private static string ConstraintLine(string itemHtml)
    {
        var withCarets = Superscripts().Replace(itemHtml, Superscript);
        return PlainText(withCarets);
    }

    [GeneratedRegex(@"<sup>(.*?)</sup>", RegexOptions.Singleline)]
    private static partial Regex Superscripts();

    // Tags dropped, entities decoded, zero-width spaces removed, every run of whitespace (no-break
    // spaces included) one space.
    private static string PlainText(string html)
    {
        var untagged = Tag().Replace(html, string.Empty);
        var decoded = WebUtility.HtmlDecode(untagged).Replace(NoBreakSpace, ' ').Replace(ZeroWidthSpace, string.Empty);
        return Whitespace().Replace(decoded, SingleSpace).Trim();
    }

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex Tag();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();
}
