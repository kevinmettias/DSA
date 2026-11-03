using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 1's promise that its section numbers are stable. Nearly a thousand
// files cite a section by number - "ARCHITECTURE 17.9", "§17.3", "ARCHITECTURE.md section 17.2" - and
// a citation is only as good as the heading it names, so every one in the code, the documents and the
// waiver ledger must name a number some heading of ARCHITECTURE.md carries. A numbered step inside a
// section is not a section of its own: "§5 step 5" cites section 5 and passes, and a dotted number
// for the same step fails.
//
// The scan reads text, not code, because citations live in comments, prose and waiver reasons. A
// section sign or the word "section" before a number is a citation; so is ARCHITECTURE or
// ARCHITECTURE.md before a number, a "#" or "Sec.", even when a comment wraps between the name and
// the number. A citation followed by "/", ",", "and" or "or" goes on to cite the next number too, if
// that number carries its own mark or a dot ("§6/§11.3", "ARCHITECTURE.md #17.3/#17.6"); the dot is
// what keeps a count after a comma from being read as a section.
public sealed partial class SectionCitationTests
{
    private const string Document = "ARCHITECTURE.md";

    private const string Advice =
        $"These citations name a number no heading of {Document} carries. Cite the section that holds the rule, and a numbered step inside one as \"§5 step 5\":";

    private static readonly string[] ScannedProjects =
    [
        "DSAExperimentation",
        "DSAExperimentation.LeetCode.Solutions",
        "DSAExperimentation.Tests",
        "DSAExperimentation.LeetCode.Solutions.Tests",
        "DSAExperimentation.Benchmarks",
        "DSAExperimentation.Benchmarks.Tests",
    ];

    // Read one level deep: the root's documents and waiver ledger, and the agent runbooks and
    // workflows, whose prompts cite sections as well.
    private static readonly (string Directory, string Pattern)[] ScannedDocuments =
    [
        (".", "*.md"),
        (".", "suppressions.json"),
        (".claude/leetcode-coverage", "*.md"),
        (".claude/workflows", "*.js"),
    ];

    private static readonly Regex Whitespace = new(@"\s+");

    public static TheoryData<string, string[]> CitationSpellings =>
        new()
        {
            { "the rule in §17.3", ["17.3"] },
            { "(ARCHITECTURE 17.9)", ["17.9"] },
            { "ARCHITECTURE.md §17.4's hoisted overload", ["17.4"] },
            { "ARCHITECTURE.md section 17.2's tier order", ["17.2"] },
            { "ARCHITECTURE.md Sec.4.1", ["4.1"] },
            { "ARCHITECTURE.md #17.3/#17.6", ["17.3", "17.6"] },
            { "§6/§11.3", ["6", "11.3"] },
            { "sections 17.3 and 17.4", ["17.3", "17.4"] },
            { "the shape section 17.4 describes", ["17.4"] },
            { "(ARCHITECTURE\n// 17.9): its two arms", ["17.9"] },
            { "§17.6a's last paragraphs", ["17.6a"] },
            { "§5 step 5", ["5"] },
            { "§17.9, 25 files", ["17.9"] },
        };

    public static TheoryData<string> NumbersThatAreNotCitations =>
        new() { "LC 3161", "measured 2026-10-03", "a 2x3 board", "subsection 3", "ARCHITECTURE-HISTORY.md, 7 reorgs" };

    [Fact]
    public void EveryCitation_NamesASectionTheDocumentHas()
    {
        var root = RepositoryFiles.Root();
        var documentPath = Path.Combine(root, Document);
        var document = File.ReadAllText(documentPath);
        var sections = HeadingNumbers(document);
        var broken = ScannedFiles(root)
            .SelectMany(file => BrokenCitationsIn(root, file, sections))
            .ToList();
        var message = string.Join(Environment.NewLine, broken.Prepend(Advice));

        Assert.True(broken.Count == 0, message);
    }

    [Fact]
    public void HeadingNumbers_NumberedHeadings_AreTheSections()
    {
        string[] expected = ["17.6a", "5"];

        var sections = HeadingNumbers("# Title\n\n## 5. Checklist\n\n1. a list item\n\n### 17.6a Enforcement\n")
            .Order(StringComparer.Ordinal);

        Assert.Equal(expected, sections);
    }

    [Theory]
    [MemberData(nameof(CitationSpellings))]
    public void Citations_EachSpelling_ReadsEveryNumberItCites(string text, string[] numbers)
    {
        var cited = Citations(text).Select(citation => citation.Number);

        Assert.Equal(numbers, cited);
    }

    [Theory]
    [MemberData(nameof(NumbersThatAreNotCitations))]
    public void Citations_NumberThatIsNotACitation_ReadsNothing(string text)
    {
        var cited = Citations(text);

        Assert.Empty(cited);
    }

    [Fact]
    public void CitationsWithoutAHeading_AStepAndAMissingSection_ReportOnlyTheMissingSection()
    {
        string[] expected = ["17.3"];
        var sections = HeadingNumbers("## 5. Checklist\n");

        var missing = CitationsWithoutAHeading("§5 step 5, then §17.3", sections).Select(citation => citation.Number);

        Assert.Equal(expected, missing);
    }

    private static HashSet<string> HeadingNumbers(string markdown) =>
        SectionCitationGrammar.Heading.Matches(markdown).Select(match => match.Groups["number"].Value).ToHashSet();

    private static IEnumerable<RepositoryFiles.SourceFile> ScannedFiles(string root) =>
        ScannedProjects
            .SelectMany(project => RepositoryFiles.SourceFilesIn(Path.Combine(root, project)))
            .Concat(ScannedDocuments.SelectMany(source => DocumentsIn(root, source)));

    private static IEnumerable<RepositoryFiles.SourceFile> DocumentsIn(string root, (string Directory, string Pattern) source)
    {
        var directory = Path.Combine(root, source.Directory);

        if (!Directory.Exists(directory))
        {
            return [];
        }

        return Directory.EnumerateFiles(directory, source.Pattern, SearchOption.TopDirectoryOnly)
            .Select(file => new RepositoryFiles.SourceFile(file));
    }

    // "path:line: citation (no section N)" for every citation in the file whose number has no heading.
    private static IEnumerable<string> BrokenCitationsIn(string root, RepositoryFiles.SourceFile file, HashSet<string> sections)
    {
        var path = RepositoryFiles.PathFromRoot(root, file);
        var text = File.ReadAllText(file);

        return CitationsWithoutAHeading(text, sections)
            .Select(citation => $"{path}:{citation.Line}: {citation.Text} (no section {citation.Number})");
    }

    private static IEnumerable<SectionCitation> CitationsWithoutAHeading(string text, HashSet<string> sections) =>
        Citations(text).Where(citation => !sections.Contains(citation.Number));

    // One entry per number cited, so a list cites each of its numbers on the line it starts on.
    private static IEnumerable<SectionCitation> Citations(string text)
    {
        foreach (Match match in SectionCitationGrammar.Citation.Matches(text))
        {
            var line = LineOf(text, match.Index);
            var spelling = Whitespace.Replace(match.Value, " ");

            foreach (Capture number in match.Groups["number"].Captures)
            {
                yield return new SectionCitation(line, spelling, number.Value);
            }
        }
    }

    private static int LineOf(string text, int index) => text.AsSpan(0, index).Count('\n') + 1;

    private readonly record struct SectionCitation(int Line, string Text, string Number);
}
