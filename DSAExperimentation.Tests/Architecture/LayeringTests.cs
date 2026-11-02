using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 17.2's tier order, which is otherwise only a
// convention the file tree implies: a tier reads only the tiers Readable names, and the
// three peer tiers that read one another form no cycle of folders between them.
//
// This is the guard a project-per-tier split would have bought from the compiler.
// It is deliberately a test instead, for two reasons the split could not answer:
// several designs depend on single-assembly encapsulation - the walk engines and
// visit guards stay `internal` behind public entry points (see IVisitGuard's own
// doc comment) - and one upward edge is
// genuinely intended - IntervalSet composes BinarySearch rather than duplicating
// its bisection loop. A ProjectReference cannot say "this one edge, for this
// reason"; the allow-list below can.
public sealed partial class LayeringTests
{
    // Lowest tier first, LeetCode last: the tier folders a framework file may sit in, and the tier
    // names a reference can name. Mechanisms has no number of its own (section 17.6b), so the tier
    // numbers code cites keep their meaning.
    private static readonly string[] Tiers = ["DataStructures", "Algorithms", "Domain", "Mechanisms", "LeetCode"];

    // The tiers each framework tier may read besides its own. DataStructures, Algorithms and
    // Domain are peers (section 17.2), save that DataStructures still reads no algorithm beyond the
    // edges AllowedInversions sanctions, and PeerTiers_FormNoCycleOfFoldersAcrossTiers keeps peers
    // from reading one another in a loop. Mechanisms reads all three, and none of them reads it.
    // No tier reads LeetCode.
    private static readonly Dictionary<string, string[]> Readable = new()
    {
        ["DataStructures"] = ["Domain"],
        ["Algorithms"] = ["DataStructures", "Domain"],
        ["Domain"] = ["DataStructures", "Algorithms"],
        ["Mechanisms"] = ["DataStructures", "Algorithms", "Domain"],
    };

    // Reads of a tier Readable withholds that are deliberate, each with the reason it is allowed.
    // An entry names the file and the one namespace it may reach, so it sanctions that edge and no
    // other: a second such reference from the same file still fails.
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
    public void EveryFile_ReferencesOnlyTiersItsOwnMayRead()
    {
        var offences = Inversions();
        var report = string.Join(Environment.NewLine, offences);

        Assert.True(offences.Count == 0, report);
    }

    // DataStructures, Algorithms and Domain may read one another, which leaves a loop between them
    // to rule out: a folder that reads itself back through another tier has split one subject
    // across two tiers, and neither half is the peer the other may read.
    [Fact]
    public void PeerTiers_FormNoCycleOfFoldersAcrossTiers()
    {
        var cycles = FolderCycles.AcrossTiers(FolderReads()).ToList();
        var report = string.Join(Environment.NewLine, cycles);

        Assert.True(cycles.Count == 0, report);
    }

    // A global using puts its namespace in scope for every file in the project, so a file could
    // read that tier with nothing in its own code to show it: a read neither the tier rule nor the
    // cycle check could see. It may name no tier, and no allow-list entry can sanction one.
    [Fact]
    public void GlobalUsings_NameNoTier()
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

    // The framework project holds the tiers below LeetCode and nothing else. A file outside their
    // folders - a loose file at the project root, a folder no tier names - would be judged by no
    // rule above, so it fails here instead.
    [Fact]
    public void EveryFrameworkFile_SitsInATierFolder()
    {
        var root = RepositoryFiles.Root();
        var untiered = RepositoryFiles.SourceFilesIn(Path.Combine(root, FrameworkProject))
            .Select(file => RepositoryFiles.PathFromRoot(root, file))
            .Where(relative => !Tiers[..^1].Contains(TierOf(relative)))
            .ToList();

        Assert.True(untiered.Count == 0, string.Join(Environment.NewLine, untiered));
    }

    // The scans above read a tier off the namespace a name is written in, so they hold only while
    // every file declares its types inside its own tier. A type an Algorithms file declared in a
    // DataStructures namespace, or in the root one, would reach DataStructures files under a name
    // no scan reads as Algorithms.
    [Fact]
    public void DeclaredNamespaces_StayInsideTheTierOfTheirFile()
    {
        var offences = TieredFiles().SelectMany(NamespaceEscapesIn).ToList();
        var report = string.Join(Environment.NewLine, offences);

        Assert.True(offences.Count == 0, report);
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
            "var name = request.Algorithms.Name;",
            "return Algorithms;",
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

    // Who may read whom, stated as cases: the peers each way and Mechanisms over them, then the
    // reads the table withholds.
    public static TheoryData<string, string> PermittedReadsByTier =>
        new()
        {
            { "DataStructures", "Domain" },
            { "Algorithms", "Domain" },
            { "Domain", "DataStructures" },
            { "Domain", "Algorithms" },
            { "Mechanisms", "Domain" },
        };

    public static TheoryData<string, string> WithheldReadsByTier =>
        new()
        {
            { "DataStructures", "Algorithms" },
            { "Domain", "Mechanisms" },
            { "Algorithms", "Mechanisms" },
            { "Mechanisms", "LeetCode" },
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

    [Theory]
    [MemberData(nameof(PermittedReadsByTier))]
    public void CanRead_LetsPeersAndMechanismsReadTheTier(string readerTier, string readTier)
        => Assert.True(CanRead(readerTier, readTier));

    [Theory]
    [MemberData(nameof(WithheldReadsByTier))]
    public void CanRead_WithholdsEveryReadTheTableLeavesOut(string readerTier, string readTier)
        => Assert.False(CanRead(readerTier, readTier));

    private static List<string> Inversions() => TieredFiles().SelectMany(InversionsIn).ToList();

    // Every framework file under a tier folder, with the tier its location puts it in. A file
    // outside them is EveryFrameworkFile_SitsInATierFolder's to report.
    private static IEnumerable<TieredFile> TieredFiles()
    {
        var root = RepositoryFiles.Root();

        foreach (var file in RepositoryFiles.SourceFilesIn(Path.Combine(root, FrameworkProject)))
        {
            var relative = RepositoryFiles.PathFromRoot(root, file);
            var tier = TierOf(relative);

            if (tier is not null)
            {
                yield return new TieredFile(file, relative, tier);
            }
        }
    }

    // Every global using the framework project sees that names a tier: a directive in any of its
    // files - tiered or not, since one at the project root reaches every tier just the same - or a
    // <Using> item in a build file MSBuild may import into it.
    private static IEnumerable<string> GlobalUsingLeaks()
    {
        var root = RepositoryFiles.Root();
        var files = RepositoryFiles.SourceFilesIn(Path.Combine(root, FrameworkProject))
            .Select(file => (string)file)
            .Concat(BuildFilesOf(root));

        return files.SelectMany(file => GlobalImportsIn(file, File.ReadAllText(file))
            .Select(reference =>
                $"{Path.GetRelativePath(root, file).Replace(Path.DirectorySeparatorChar, '/')}: a global using of "
                + $"{reference.Target} puts {reference.Tier} in scope for every file, with no code of theirs to show it."));
    }

    // The framework project's own project file, which must exist, and whichever of the files
    // MSBuild imports into every project beneath them sit in its folder or at the repository
    // root: the Directory.Build pair, and Directory.Packages.props, which NuGet's props import
    // whether or not central package management is on.
    private static IEnumerable<string> BuildFilesOf(string root)
    {
        var project = Path.Combine(root, FrameworkProject);
        string[] importedIntoEveryProject = ["Directory.Build.props", "Directory.Build.targets", "Directory.Packages.props"];

        return importedIntoEveryProject
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
        var pathInProject = TrimProject(file.RelativePath);
        var code = RepositoryFiles.CodeOf(File.ReadAllText(file.FullPath));

        foreach (var reference in ReferencesIn(code))
        {
            var sanctioned = AllowedInversions.Keys
                .Any(edge => edge.PathInProject == pathInProject && edge.Covers(reference));

            if (!CanRead(file.Tier, reference.Tier) && !sanctioned)
            {
                yield return $"{file.RelativePath}: {file.Tier} references {reference.Target}, in {reference.Tier}, which {file.Tier} may not read.";
            }
        }
    }

    private static bool CanRead(string readerTier, string readTier)
        => readerTier == readTier || Readable[readerTier].Contains(readTier);

    // Each folder's reads of another, off the same references the tier rule reads, each name turned
    // into a path inside the project.
    private static List<FolderGraph.FolderRead> FolderReads()
    {
        var files = TieredFiles().ToList();
        var folders = files.Select(FolderOf).ToHashSet();

        return files
            .SelectMany(file => FolderGraph.ReadsFrom(
                FolderOf(file),
                ReferencesIn(RepositoryFiles.CodeOf(File.ReadAllText(file.FullPath)))
                    .Select(reference => reference.Target[(RootNamespace.Length + 1)..].Replace('.', '/')),
                folders))
            .ToList();
    }

    private static string FolderOf(TieredFile file)
    {
        var pathInProject = TrimProject(file.RelativePath);

        return pathInProject[..pathInProject.LastIndexOf('/')];
    }

    // Every tier namespace the code names, each written out in full. An unqualified match counts
    // only once something is reached through it: a bare `Algorithms` is far likelier a property or
    // a local than a namespace.
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

    // The namespaces one file declares outside its tier, each stated as a finding.
    private static IEnumerable<string> NamespaceEscapesIn(TieredFile file)
    {
        var code = RepositoryFiles.CodeOf(File.ReadAllText(file.FullPath));
        var tierNamespace = $"{RootNamespace}.{file.Tier}";

        return NamespaceDeclarations.OutsideOf(tierNamespace, code)
            .Select(name => $"{file.RelativePath}: declares {name}, outside {tierNamespace}.");
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
