using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// The namespaces a source file declares, which decide the names its types can be reached by. A
// file that declares none has put its types in the global namespace, where every file can name
// them, and reads as declaring that.
internal static class NamespaceDeclarations
{
    // What a file that declares no namespace declares instead.
    public const string Global = "the global namespace";

    // A namespace declaration, file-scoped or block, read from the start of its line. A block
    // namespace nested in another reads as its own name alone, so it lands outside any container
    // and fails rather than passing unread.
    private static readonly Regex Declaration = new(@"^\s*namespace\s+(?<name>[\w.]+)", RegexOptions.Multiline);

    // Each namespace the code declares that is neither the container nor inside it, the global
    // namespace included when the code declares none. A sibling that merely shares the container's
    // prefix is outside it. The code is a file's text with its comments and strings blanked, as
    // RepositoryFiles.CodeOf hands it out.
    public static IEnumerable<string> OutsideOf(string container, string code)
        => NamesIn(code).Where(name => !IsWithin(name, container));

    private static List<string> NamesIn(string code)
    {
        var declared = Declaration.Matches(code).Select(match => match.Groups["name"].Value).ToList();

        if (declared.Count == 0)
        {
            return [Global];
        }

        return declared;
    }

    private static bool IsWithin(string name, string container)
        => name == container || name.StartsWith($"{container}.", StringComparison.Ordinal);
}
