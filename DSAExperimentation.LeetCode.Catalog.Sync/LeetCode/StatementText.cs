using System.Net;
using System.Text.RegularExpressions;

namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode;

// The two facts the catalog reads out of a question's HTML statement, because LeetCode's API has
// no field for either: each example's stated output, and the constraint lines.
//
// LeetCode writes examples in two layouts. Older questions use a <pre> block ("<strong>Output:</strong>
// [0,1]\n<strong>Explanation:</strong>..."), newer ones a <div class="example-block"> with each
// value in a <span class="example-io">. OutputLabel matches the label in either, and the value runs
// to whatever closes it - the next label, </pre>, </p> or </div> - then loses its tags.
internal static partial class StatementText
{
    private const string Superscript = "^$1";
    private const string OutputGroup = "output";
    private const string ItemsGroup = "items";
    private const string ItemGroup = "item";
    private const string SingleSpace = " ";
    private const char NoBreakSpace = '\u00a0';

    public static IReadOnlyList<string> Outputs(string contentHtml)
        => OutputLabel().Matches(contentHtml)
            .Select(match => PlainText(match.Groups[OutputGroup].Value))
            .ToList();

    [GeneratedRegex(@"Output:?\s*</(?:strong|b)>\s*(?<output>.*?)\s*(?=\n\s*<(?:strong|b)>|</pre>|</p>|</div>)", RegexOptions.Singleline)]
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

    // Tags dropped, entities decoded, every run of whitespace (no-break spaces included) one space.
    private static string PlainText(string html)
    {
        var untagged = Tag().Replace(html, string.Empty);
        var decoded = WebUtility.HtmlDecode(untagged).Replace(NoBreakSpace, ' ');
        return Whitespace().Replace(decoded, SingleSpace).Trim();
    }

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex Tag();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();
}
