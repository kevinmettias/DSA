using System.Collections;
using System.Globalization;
using System.Text;

namespace DSAExperimentation.Benchmarks.Tests;

// Benchmark arms for one problem are competing strategies for the same question, so two arms
// that answer differently are timing two different problems. Comparing their answers needs one
// rendering that treats an array and a list of equal values as equal: Assert.Equal on a jagged
// array falls back to reference equality for the inner arrays and would pass every time.
internal static class AnswerText
{
    // The two renderings below write these between elements and for a null answer, so they are
    // named once here rather than repeated as bare literals at each call site.
    private const string ElementSeparator = ",";
    private const string NullText = "null";

    public static string Of(object? answer)
    {
        var text = new StringBuilder();
        Append(text, answer);

        return text.ToString();
    }

    // LeetCode leaves the order of the returned collection unspecified for many problems while
    // pinning the order inside each element - merged accounts, grouped anagrams, partitioned
    // lists. Rendering such an answer as a set of rendered elements keeps each element's own
    // order checked without pinning an outer order the problem never promised.
    public static string OfUnorderedSet(object? answer)
    {
        var text = new StringBuilder();
        AppendSet(text, answer);

        return text.ToString();
    }

    private static void Append(StringBuilder text, object? value)
    {
        if (value is null)
        {
            text.Append(NullText);
        }
        else if (value is string item)
        {
            text.Append('"').Append(item).Append('"');
        }
        else if (value is IEnumerable sequence)
        {
            AppendSequence(text, sequence);
        }
        else
        {
            text.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
        }
    }

    private static void AppendSequence(StringBuilder text, IEnumerable sequence)
    {
        var separator = string.Empty;
        text.Append('[');

        foreach (var item in sequence)
        {
            text.Append(separator);
            Append(text, item);
            separator = ElementSeparator;
        }

        text.Append(']');
    }

    private static void AppendSet(StringBuilder text, object? value)
    {
        if (value is not IEnumerable sequence)
        {
            Append(text, value);

            return;
        }

        var elements = new List<string>();

        foreach (var item in sequence)
        {
            elements.Add(Of(item));
        }

        elements.Sort(StringComparer.Ordinal);
        text.Append('{').Append(string.Join(ElementSeparator, elements)).Append('}');
    }
}
