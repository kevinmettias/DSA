using System.Text.Json;

namespace DSAExperimentation.LeetCode.Catalog;

// LeetCode writes every value its judge passes as JSON: numbers, double-quoted strings and
// characters, true/false, and arrays of those nested to any depth, with null standing in for an
// absent tree node. So System.Text.Json reads all of it, and a tree or a list is read here as the
// array LeetCode wrote (int?[], int[]) for LeetCodeWireFormat to build the nodes from.
//
// A bare null is never a value LeetCode passes - an empty tree or list is "[]" - so reading one
// throws rather than handing a harness a null it would fail on somewhere further away.
/// <summary>Reads LeetCode's notation for an input or an answer into a typed value.</summary>
public static class LeetCodeNotation
{
    /// <summary>What LeetCode puts between one case's inputs when it writes them as a single text: a newline.</summary>
    public const char InputSeparator = '\n';

    /// <summary>Parses one value written in LeetCode's notation.</summary>
    /// <typeparam name="TValue">The type to read the value as, such as int[], string or IList&lt;IList&lt;int&gt;&gt;.</typeparam>
    /// <param name="text">The value as LeetCode writes it, such as "[2,7,11,15]".</param>
    /// <returns>The value as a <typeparamref name="TValue"/>.</returns>
    /// <exception cref="FormatException">The text is a bare null, which LeetCode never passes as a value.</exception>
    /// <exception cref="JsonException">The text is not a value of type <typeparamref name="TValue"/>.</exception>
    public static TValue Parse<TValue>(string text)
    {
        var value = JsonSerializer.Deserialize<TValue>(text);

        if (value is null)
        {
            throw new FormatException($"'{text}' is a bare null, which LeetCode never passes as a value.");
        }

        return value;
    }
}
