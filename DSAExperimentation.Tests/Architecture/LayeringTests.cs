using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 17.2's tier order, which is otherwise only a
// convention the file tree implies: a tier may depend downward and never upward.
//
// This is the guard a project-per-tier split would have bought from the compiler.
// It is deliberately a test instead, for two reasons the split could not answer:
// every type here is `internal` and several designs depend on single-assembly
// encapsulation (see IVisitGuard's own doc comment), and one upward edge is
// genuinely intended - IntervalSet composes BinarySearch rather than duplicating
// its bisection loop. A ProjectReference cannot say "this one edge, for this
// reason"; the allow-list below can.
public sealed partial class LayeringTests
{
    // Lowest tier first. A file in tier i may reference tiers 0..i only.
    private static readonly string[] Tiers = ["DataStructures", "Algorithms", "Domain", "LeetCode"];

    // Upward edges that are deliberate, each with the reason it is allowed. An entry names the
    // file and the one higher-tier namespace it may reach, so it sanctions that edge and no
    // other: a second upward reference from the same file still fails.
    private static readonly Dictionary<AllowedInversion, string> AllowedInversions = new()
    {
        [new("DataStructures/IntervalSet/IntervalSet.cs", "DSAExperimentation.Algorithms.Searching")] =
            "IntervalSet keeps its intervals sorted and locates candidates with BinarySearch.LowerBound "
            + "through a private IRandomAccessSequence view, rather than carrying a second bisection loop. "
            + "Documented in the type's own doc comment.",
    };

    // A name in a tier namespace, wherever code writes one: in a using directive of any kind, or
    // inline as a qualified name. The root qualifier is optional because inside any
    // DSAExperimentation namespace `Algorithms.Searching.BinarySearch` resolves exactly as the
    // fully qualified name does, with no using at all.
    private static readonly Regex Reference = new(
        $@"(?<![\w.])(?<qualifier>{RootNamespace}\.)?(?<tier>{string.Join('|', Tiers)})\b(?<rest>(?:\.\w+)*)");

    // The two spellings of a global using: a directive in a source file, and a <Using> item in a
    // build file.
    private static readonly Regex GlobalUsingDirective = new(@"\bglobal\s+using\b[^;]*;");
    private static readonly Regex UsingItem = new(@"<Using\b[^>]*\bInclude\s*=\s*""(?<name>[^""]*)""");
    private static readonly Regex XmlComment = new("<!--.*?-->", RegexOptions.Singleline);

    // The one project laid out in tier folders. DSAExperimentation.LeetCode is the top
    // tier in a project of its own, so nothing it could reference sits above it - a row
    // for it here could never fail, and the boundary it does have is the compiler's.
    private const string FrameworkProject = "DSAExperimentation";

    private const string RootNamespace = "DSAExperimentation";

    [Fact]
    public void EveryFile_ReferencesItsOwnTierOrALowerOne()
    {
        var offences = Inversions();
        var report = string.Join(Environment.NewLine, offences);

        Assert.True(offences.Count == 0, report);
    }

    // A global using puts its namespace in scope for every file in the project, the lowest
    // tier's included, so it is judged as though each of them had written it and no allow-list
    // entry can sanction it: it may name the lowest tier and nothing above.
    [Fact]
    public void GlobalUsings_NameNoTierAboveTheLowest()
    {
        var offences = GlobalUsingLeaks().ToList();
        var report = string.Join(Environment.NewLine, offences);

        Assert.True(offences.Count == 0, report);
    }

    // A sweep that found no tiered file would report no inversions while checking nothing.
    [Fact]
    public void TierSweep_OverTheFrameworkProject_FindsFilesInEveryLowerTier()
    {
        var root = RepositoryFiles.Root();
        var tiersFound = RepositoryFiles.SourceFilesIn(Path.Combine(root, FrameworkProject))
            .Select(file => TierOf(RepositoryFiles.PathFromRoot(root, file)))
            .OfType<string>()
            .ToHashSet();

        Assert.Equal(Tiers[..^1], Tiers.Where(tiersFound.Contains));
    }

    // The scanner's reach, stated as cases: a sweep that reports nothing proves only that
    // nothing it can see is wrong.
    public static TheoryData<string> SpellingsOfAnAlgorithmsReference =>
        new()
        {
            "using DSAExperimentation.Algorithms.Searching;",
            "using static DSAExperimentation.Algorithms.Searching.BinarySearch;",
            "using Search = DSAExperimentation.Algorithms.Searching.BinarySearch;",
            "global using DSAExperimentation.Algorithms.Searching;",
            "var at = DSAExperimentation.Algorithms.Searching.BinarySearch.LowerBound(keys, key);",
            "var at = global::DSAExperimentation.Algorithms.Searching.BinarySearch.LowerBound(keys, key);",
            "var at = Algorithms.Searching.BinarySearch.LowerBound(keys, key);",
        };

    public static TheoryData<string> LookalikesOfATierName =>
        new()
        {
            "// composes DSAExperimentation.Algorithms.Searching.BinarySearch",
            "var note = \"see Algorithms.Searching.BinarySearch\";",
            "var name = request.Domain.Name;",
            "return Domain;",
        };

    public static TheoryData<string, string> GlobalUsingsByFile =>
        new()
        {
            { "Usings.cs", "global using DSAExperimentation.Algorithms.Searching;" },
            { "Usings.cs", "global using static DSAExperimentation.Algorithms.Searching.BinarySearch;" },
            { "DSAExperimentation.csproj", "<ItemGroup><Using Include=\"DSAExperimentation.Algorithms.Searching\" /></ItemGroup>" },
            { "Directory.Build.props", "<Using Alias=\"Search\" Include=\"DSAExperimentation.Algorithms.Searching.BinarySearch\" />" },
        };

    public static TheoryData<string, string> ImportsThatAreNotGlobalByFile =>
        new()
        {
            { "IntervalSet.cs", "using DSAExperimentation.Algorithms.Searching;" },
            { "Directory.Build.props", "<!-- <Using Include=\"DSAExperimentation.Algorithms.Searching\" /> -->" },
        };

    public static TheoryData<string, bool> TargetsToWhetherSearchingCoversThem =>
        new()
        {
            { "DSAExperimentation.Algorithms.Searching", true },
            { "DSAExperimentation.Algorithms.Searching.BinarySearch.LowerBound", true },
            { "DSAExperimentation.Algorithms.SearchingTrees", false },
            { "DSAExperimentation.Algorithms.Sorting.MergeSort", false },
        };

    [Theory]
    [MemberData(nameof(SpellingsOfAnAlgorithmsReference))]
    public void ReferencesIn_FindsAHigherTierHoweverItIsNamed(string source)
        => Assert.Contains(ReferencesIn(RepositoryFiles.CodeOf(source)), reference => reference.Tier == "Algorithms");

    [Theory]
    [MemberData(nameof(LookalikesOfATierName))]
    public void ReferencesIn_IgnoresWhatOnlyLooksLikeATierName(string source)
        => Assert.Empty(ReferencesIn(RepositoryFiles.CodeOf(source)));

    [Theory]
    [MemberData(nameof(GlobalUsingsByFile))]
    public void GlobalImportsIn_FindsAGlobalUsingInASourceOrBuildFile(string fileName, string text)
        => Assert.Contains(GlobalImportsIn(fileName, text), reference => reference.Tier == "Algorithms");

    [Theory]
    [MemberData(nameof(ImportsThatAreNotGlobalByFile))]
    public void GlobalImportsIn_IgnoresWhatIsNotAGlobalUsing(string fileName, string text)
        => Assert.Empty(GlobalImportsIn(fileName, text));

    // An entry covers its namespace and everything inside it, and stops at the namespace's own
    // boundary: a sibling that merely shares its prefix is a different edge.
    [Theory]
    [MemberData(nameof(TargetsToWhetherSearchingCoversThem))]
    public void AllowedInversion_CoversItsNamespaceAndNothingBesideIt(string target, bool covered)
    {
        var edge = new AllowedInversion("DataStructures/IntervalSet/IntervalSet.cs", "DSAExperimentation.Algorithms.Searching");

        Assert.Equal(covered, edge.Covers(new TierReference("Algorithms", target)));
    }

    private static List<string> Inversions()
    {
        var root = RepositoryFiles.Root();
        var offences = new List<string>();

        foreach (var file in RepositoryFiles.SourceFilesIn(Path.Combine(root, FrameworkProject)))
        {
            var relative = RepositoryFiles.PathFromRoot(root, file);
            var tier = TierOf(relative);

            if (tier is not null)
            {
                offences.AddRange(InversionsIn(new TieredFile(file, relative, tier)));
            }
        }

        return offences;
    }

    // Every global using the framework project sees that names a tier above the lowest: a
    // directive in any of its files - tiered or not, since one at the project root reaches every
    // tier just the same - or a <Using> item in a build file MSBuild may import into it.
    private static IEnumerable<string> GlobalUsingLeaks()
    {
        var root = RepositoryFiles.Root();
        var files = RepositoryFiles.SourceFilesIn(Path.Combine(root, FrameworkProject))
            .Select(file => (string)file)
            .Concat(BuildFilesOf(root));

        return files.SelectMany(file => GlobalImportsIn(file, File.ReadAllText(file))
            .Where(reference => reference.Tier != Tiers[0])
            .Select(reference =>
                $"{Path.GetRelativePath(root, file).Replace(Path.DirectorySeparatorChar, '/')}: a global using of "
                + $"{reference.Target} puts {reference.Tier} in scope for every tier below it."));
    }

    // The framework project's own project file, which must exist, and whichever Directory.Build
    // files sit in its folder or at the repository root, where MSBuild looks for them.
    private static IEnumerable<string> BuildFilesOf(string root)
    {
        var project = Path.Combine(root, FrameworkProject);
        string[] directoryBuildFiles = ["Directory.Build.props", "Directory.Build.targets"];

        return directoryBuildFiles
            .SelectMany(name => new[] { Path.Combine(project, name), Path.Combine(root, name) })
            .Where(File.Exists)
            .Prepend(Path.Combine(project, $"{FrameworkProject}.csproj"));
    }

    [Fact]
    public void AllowedInversions_AreAllStillPresent()
    {
        // A stale entry would silently widen the rule; delete it once the edge is gone. The edge,
        // not the file: a file that survives but no longer names the namespace is just as stale.
        var root = RepositoryFiles.Root();

        foreach (var edge in AllowedInversions.Keys)
        {
            var full = Path.Combine(root, FrameworkProject, edge.PathInProject.Replace('/', Path.DirectorySeparatorChar));

            Assert.True(File.Exists(full), $"{edge.PathInProject} is allow-listed as a deliberate tier inversion but no longer exists.");

            var references = ReferencesIn(RepositoryFiles.CodeOf(File.ReadAllText(full)));

            Assert.True(
                references.Any(edge.Covers),
                $"{edge.PathInProject} is allow-listed as reaching up into {edge.Namespace}, but its code no longer names it.");
        }
    }

    [Fact]
    public void LeetCodeTier_LivesInItsOwnProject()
    {
        // The one boundary that IS compiler-enforced: nothing in the framework
        // project may depend on a LeetCode solution, because it cannot see one.
        var root = RepositoryFiles.Root();
        var frameworkLeetCode = Path.Combine(root, "DSAExperimentation", "LeetCode");

        Assert.False(
            Directory.Exists(frameworkLeetCode),
            "LeetCode solutions belong in DSAExperimentation.LeetCode, not the framework project.");
    }

    // The tier folder a framework file's path names directly, or null when it names none.
    private static string? TierOf(string relative)
    {
        var segments = relative[(FrameworkProject.Length + 1)..].Split('/');

        if (segments.Length <= 1 || !Tiers.Contains(segments[0]))
        {
            return null;
        }

        return segments[0];
    }

    private static IEnumerable<string> InversionsIn(TieredFile file)
    {
        var rank = Array.IndexOf(Tiers, file.Tier);
        var pathInProject = TrimProject(file.RelativePath);
        var code = RepositoryFiles.CodeOf(File.ReadAllText(file.FullPath));

        foreach (var reference in ReferencesIn(code))
        {
            var sanctioned = AllowedInversions.Keys
                .Any(edge => edge.PathInProject == pathInProject && edge.Covers(reference));

            if (Array.IndexOf(Tiers, reference.Tier) > rank && !sanctioned)
            {
                yield return $"{file.RelativePath}: {file.Tier} references {reference.Target}, in {reference.Tier}, a higher tier.";
            }
        }
    }

    // Every tier namespace the code names, each written out in full. An unqualified match counts
    // only once something is reached through it: a bare `Domain` is far likelier a property than
    // a namespace.
    private static IEnumerable<TierReference> ReferencesIn(string code)
        => Reference.Matches(code)
            .Where(match => match.Groups["qualifier"].Success || match.Groups["rest"].Length > 0)
            .Select(match => new TierReference(
                match.Groups["tier"].Value,
                $"{RootNamespace}.{match.Groups["tier"].Value}{match.Groups["rest"].Value}"))
            .Distinct();

    // The tier namespaces a file imports for the whole project, read as C# from a source file and
    // as MSBuild from anything else.
    private static IEnumerable<TierReference> GlobalImportsIn(string fileName, string text)
    {
        if (fileName.EndsWith(".cs", StringComparison.Ordinal))
        {
            return GlobalUsingDirective.Matches(RepositoryFiles.CodeOf(text))
                .SelectMany(directive => ReferencesIn(directive.Value));
        }

        return UsingItem.Matches(XmlComment.Replace(text, string.Empty))
            .SelectMany(item => ReferencesIn(item.Groups["name"].Value));
    }

    private static string TrimProject(string relative)
    {
        var slash = relative.IndexOf('/');

        if (slash < 0)
        {
            return relative;
        }

        return relative[(slash + 1)..];
    }

    // One source file under a tier project, named three ways: the absolute path to read, the
    // root-relative path a finding states it by, and the tier its location puts it in. Three
    // adjacent strings would let a call site transpose them and still compile.
    public readonly record struct TieredFile(string FullPath, string RelativePath, string Tier);

    // A name a file reaches for, written out from the root namespace, and the tier that name
    // sits in.
    public readonly record struct TierReference(string Tier, string Target);

    // One deliberate upward edge: a file, as a path inside the framework project, and the
    // namespace it may reach into. A reference falls inside that namespace when it names the
    // namespace itself or anything declared within it.
    public readonly record struct AllowedInversion(string PathInProject, string Namespace)
    {
        public bool Covers(TierReference reference)
            => reference.Target == Namespace
                || reference.Target.StartsWith($"{Namespace}.", StringComparison.Ordinal);
    }
}
