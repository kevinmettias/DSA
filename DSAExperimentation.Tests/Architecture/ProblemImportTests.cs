using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 17.3's rule inside the LeetCode tier: a problem folder reaches
// into another only along a named edge, a Delegation (two problems ask the same question under
// different constraints, and one owns the arms both run) or a Subroutine (one problem's answer runs
// another's algorithm as a step). Each edge is an entry below saying which it is and why. An owner
// imports no problem, so delegation never chains and never runs both ways, and the tier's root and
// Conventions/ import no problem at all.
//
// The scan reads the solution project's compilation (RepositoryCompilation): every name in a file is
// bound, and a symbol in another problem's namespace is a reference to that problem, however the file
// spelled it - a using of any kind, a partially qualified `PermutationsII.PermutationsIISolution`
// that needs no using, or a bare call a static using brought in.
public sealed partial class ProblemImportTests
{
    private const string SolutionProject = "DSAExperimentation.LeetCode";
    private const string RootImporter = "(root)";
    private const string EdgeArrow = " -> ";
    private const string SolutionSuffix = "Solution.cs";
    private const string TierNamespacePrefix = "DSAExperimentation.LeetCode.";

    // Where checks.json declares check-duplicate-constant's problem units, and the folders they share.
    private const string ChecksFile = "checks.json";
    private const string ChecksProperty = "checks";
    private const string DuplicateConstantCheck = "check-duplicate-constant";
    private const string OptionsProperty = "options";
    private const string IndependentUnitsOption = "independent_units";
    private const string SharedField = "shared";
    private const string ProjectFolderPrefix = $"{SolutionProject}/";

    // Folders at the tier's root that hold what LeetCode fixes for every problem, not a problem: its
    // notation and answer rules. Arithmetic modulo the 1e9+7 it reports answers in is Domain/Modular.
    private static readonly string[] TierRootFolders = ["Conventions"];

    private static readonly string[] EdgeKinds = ["Delegation: ", "Subroutine: "];

    // Every reference one problem makes to another, read once: binding the project's names is the
    // costly part of the scan.
    private static readonly Lazy<List<ProblemReference>> References = new(ProblemReferences);

    // Keyed "Importer -> Imported" by folder.
    private static readonly Dictionary<string, string> AllowedProblemImports = new()
    {
        ["BeautifulTowersII -> BeautifulTowersI"] =
            "Delegation: Part II is Part I at a larger bound, and Part I's arms already answer a long.",
        ["CheckIfDigitsAreEqualInStringAfterOperationsII -> CheckIfDigitsAreEqualInStringAfterOperationsI"] =
            "Delegation: Part II's baseline is Part I's direct reduction, run at Part II's bound.",
        ["CountPrefixAndSuffixPairsI -> CountPrefixAndSuffixPairsII"] =
            "Delegation: Part I's baseline is Part II's, narrowed to int; the rolling-hash arm stays Part I's.",
        ["CountSubarraysWithEvenOddRatioI -> CountSubarraysWithEvenOddRatioII"] =
            "Delegation: both of Part I's arms are Part II's, narrowed to int.",
        ["CountSubarraysWithMajorityElementI -> CountSubarraysWithMajorityElementII"] =
            "Delegation: both of Part I's arms are Part II's, narrowed to int.",
        ["DistributeCandiesAmongChildrenI -> DistributeCandiesAmongChildrenII"] =
            "Delegation: both of Part I's arms are Part II's, narrowed to int.",
        ["FindBeautifulIndicesInTheGivenArrayI -> FindBeautifulIndicesInTheGivenArrayII"] =
            "Delegation: Part I's baseline and the pairing step both matchers finish with are Part II's.",
        ["FindTheCountOfMonotonicPairsI -> FindTheCountOfMonotonicPairsII"] =
            "Delegation: Part I is Part II with nums[i] <= 50, and Part II owns both arms.",
        ["FindTheNumberOfWaysToPlacePeopleI -> FindTheNumberOfWaysToPlacePeopleII"] =
            "Delegation: both of Part I's arms and the sweep order they read are Part II's.",
        ["MaximumAreaRectangleWithPointConstraintsII -> MaximumAreaRectangleWithPointConstraintsI"] =
            "Delegation: Part II's baseline is Part I's quadruple scan, run at Part II's bound.",
        ["MaximumStrongPairXORI -> MaximumStrongPairXORII"] =
            "Delegation: both of Part I's arms are Part II's.",
        ["MaximumSumOfMNonOverlappingSubarraysII -> MaximumSumOfMNonOverlappingSubarraysI"] =
            "Delegation: Part II's baseline is Part I's DP, run at Part II's bound.",
        ["MinimumNumberOfValidStringsToFormTargetI -> MinimumNumberOfValidStringsToFormTargetII"] =
            "Delegation: both of Part I's arms are Part II's.",
        ["MinimumPairRemovalToSortArrayI -> MinimumPairRemovalToSortArrayII"] =
            "Delegation: Part I's baseline is Part II's, whose long sums answer Part I's bound identically.",
        ["MinimumTimeToRevertWordToInitialStateI -> MinimumTimeToRevertWordToInitialStateII"] =
            "Delegation: both of Part I's arms are Part II's.",
        ["NumberOfSubarraysThatMatchAPatternI -> NumberOfSubarraysThatMatchAPatternII"] =
            "Delegation: Part I's baseline and the sign encoding it searches are Part II's; the matcher stays Part I's.",
        ["Permutations -> PermutationsII"] =
            "Delegation: Permutations is Permutations II on distinct values.",
        ["PopulatingNextRightPointersInEachNode -> PopulatingNextRightPointersInEachNodeII"] =
            "Delegation: a perfect tree is one case of Part II's arbitrary binary tree.",
        ["XORAfterRangeMultiplicationQueriesII -> XORAfterRangeMultiplicationQueriesI"] =
            "Delegation: Part II's baseline is Part I's strided walk, run at Part II's bound.",
        ["BestTimeToBuyAndSellStockIII -> BestTimeToBuyAndSellStockIV"] =
            "Delegation: Part III is Part IV with the transaction budget fixed at 2.",
        ["HouseRobberII -> HouseRobber"] =
            "Subroutine: the circle's answer is House Robber's over its two runs that leave out an end house.",
        ["MaximalRectangle -> LargestRectangleInHistogram"] =
            "Subroutine: the largest rectangle of ones runs the histogram's largest rectangle on every row.",
        ["LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees -> ConstructQuadTree"] =
            "Subroutine: each arm builds its quad trees with Construct Quad Tree's matching arm.",
    };


    [Fact]
    public void EveryProblemFolder_ReferencesAnotherOnlyAlongAnAllowedEdge()
    {
        var offences = References.Value
            .Where(reference => !AllowedProblemImports.ContainsKey(reference.Edge))
            .Select(reference => $"{reference.File}: \"{reference.Edge}\" is not an allowed edge. "
                + "A delegation or a subroutine gets an entry saying which it is (ARCHITECTURE.md 17.3); "
                + "anything else two folders share belongs below the problems.")
            .Distinct()
            .ToList();

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    // An entry whose edge has gone would otherwise sit in the list sanctioning nothing.
    [Fact]
    public void AllowedProblemImports_AreAllStillPresent()
    {
        var present = References.Value.Select(reference => reference.Edge).ToHashSet();

        Assert.DoesNotContain(AllowedProblemImports.Keys, edge => !present.Contains(edge));
    }

    [Fact]
    public void AllowedProblemImports_OwnersImportNoProblem()
    {
        var importers = AllowedProblemImports.Keys.Select(edge => EndsOf(edge).Importer).ToHashSet();

        Assert.DoesNotContain(AllowedProblemImports.Keys.Select(edge => EndsOf(edge).Imported), importers.Contains);
    }

    [Fact]
    public void AllowedProblemImports_EachSaysWhichKindOfEdgeItIs() =>
        Assert.All(
            AllowedProblemImports.Values,
            reason => Assert.Contains(EdgeKinds, kind => reason.StartsWith(kind, StringComparison.Ordinal)));

    // A new shared folder at the tier's root must be declared, not silently scanned as a problem.
    [Fact]
    public void SolutionProjectFolders_AreProblemsOrDeclaredRootFolders()
    {
        var project = Path.Combine(RepositoryFiles.Root(), SolutionProject);

        Assert.DoesNotContain(ProblemFolders(), folder => !File.Exists(Path.Combine(project, folder, folder + SolutionSuffix)));
    }

    // checks.json makes every folder under the project a problem unit for check-duplicate-constant unless it
    // lists the folder as shared. A root folder missing there would be read as one more problem, and the
    // copies the problems take of its constants would be set aside as coincidences between units.
    [Fact]
    public void TierRootFolders_AreSharedByTheDuplicateConstantUnits() =>
        Assert.Equal(TierRootFolders.Order(StringComparer.Ordinal), DuplicateConstantSharedFolders().Order(StringComparer.Ordinal));

    [Fact]
    public void ProblemReferencesIn_SelfAliasConventionsAndTheTierItself_FindsNone() =>
        Assert.Empty(ProbeReferences(
            """
            using DSAExperimentation.LeetCode.Conventions;
            using Self = DSAExperimentation.LeetCode.Alpha.AlphaSolution;

            namespace DSAExperimentation.LeetCode.Alpha;

            internal static class AlphaSolution
            {
                public static int Answer() => Self.Other() + LeetCodeAnswer.None + RootRule.Value;

                public static int Other() => 0;
            }
            """));

    [Fact]
    public void ProblemReferencesIn_SiblingNamespaceWithNoUsing_FindsTheSibling() =>
        Assert.Equal(
            ["AlphaII"],
            ProbeReferences(
                """
                namespace DSAExperimentation.LeetCode.Alpha;

                internal static class AlphaSolution
                {
                    public static int Answer(int[] nums) => AlphaII.AlphaIISolution.Answer(nums);
                }
                """));

    [Fact]
    public void ProblemReferencesIn_SiblingNamedInACommentOrString_FindsNone() =>
        Assert.Empty(ProbeReferences(
            """
            namespace DSAExperimentation.LeetCode.Alpha;

            // AlphaII.AlphaIISolution does the work
            internal static class AlphaSolution
            {
                public static string Answer() => "AlphaII.AlphaIISolution";
            }
            """));

    // A static using brings the sibling's members in with no type name at the call.
    [Fact]
    public void ProblemReferencesIn_CallAStaticUsingBroughtIn_FindsTheSibling() =>
        Assert.Equal(
            ["AlphaII"],
            ProbeReferences(
                """
                using static DSAExperimentation.LeetCode.AlphaII.AlphaIISolution;

                namespace DSAExperimentation.LeetCode.Alpha;

                internal static class AlphaSolution
                {
                    public static int Run(int[] nums) => Answer(nums);
                }
                """));

    [Fact]
    public void ProblemReferencesIn_AliasedUsing_FindsTheSibling() =>
        Assert.Equal(
            ["AlphaII"],
            ProbeReferences(
                """
                using Owner = DSAExperimentation.LeetCode.AlphaII.AlphaIISolution;

                namespace DSAExperimentation.LeetCode.Alpha;

                internal static class AlphaSolution
                {
                    public static int Run(int[] nums) => Owner.Answer(nums);
                }
                """));

    // The problem folders the names in one file bind to, other than its own.
    private static IEnumerable<string> ProblemReferencesIn(string importer, SyntaxTree tree, SemanticModel model, ICollection<string> problems)
        => tree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>()
            .Select(name => model.GetSymbolInfo(name).Symbol)
            .OfType<ISymbol>()
            .Select(ProblemFolderOf)
            .OfType<string>()
            .Where(folder => problems.Contains(folder) && folder != importer)
            .Distinct();

    // The problem folder a symbol is declared in: the namespace segment after the tier's own.
    private static string? ProblemFolderOf(ISymbol symbol)
    {
        var name = (symbol as INamespaceSymbol ?? symbol.ContainingNamespace)?.ToDisplayString() ?? string.Empty;

        if (!name.StartsWith(TierNamespacePrefix, StringComparison.Ordinal))
        {
            return null;
        }

        return name[TierNamespacePrefix.Length..].Split('.')[0];
    }

    private static List<ProblemReference> ProblemReferences()
    {
        var project = Path.Combine(RepositoryFiles.Root(), SolutionProject);
        var problems = ProblemFolders().ToHashSet();
        var compilation = RepositoryCompilation.Solutions;
        var references = new List<ProblemReference>();

        foreach (var tree in compilation.SyntaxTrees.Where(tree => tree.FilePath.StartsWith(project, StringComparison.Ordinal)))
        {
            var relative = RepositoryFiles.PathFromRoot(project, new RepositoryFiles.SourceFile(tree.FilePath));
            var importer = ImporterOf(relative);

            references.AddRange(
                ProblemReferencesIn(importer, tree, compilation.GetSemanticModel(tree), problems)
                    .Select(imported => new ProblemReference(relative, importer, imported)));
        }

        return references;
    }

    // The problems Alpha's probe file names, the file compiled beside its Neighbours.
    private static List<string> ProbeReferences(string alphaSource)
    {
        var alpha = CSharpSyntaxTree.ParseText(alphaSource);
        var probe = ProbeCompilation(alpha);

        return [.. ProblemReferencesIn("Alpha", alpha, probe.GetSemanticModel(alpha), ["Alpha", "AlphaII"])];
    }

    // A probe that does not compile would bind nothing, so it fails here instead.
    private static CSharpCompilation ProbeCompilation(SyntaxTree alpha)
    {
        var probe = CSharpCompilation.Create(
            "Probe",
            [alpha, CSharpSyntaxTree.ParseText(Neighbours)],
            RepositoryCompilation.Framework.References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Assert.Empty(probe.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));

        return probe;
    }

    // What a probe's Alpha folder can name: a sibling AlphaII, a Conventions folder and a tier-root
    // rule.
    private const string Neighbours = """
            namespace DSAExperimentation.LeetCode.AlphaII
            {
                internal static class AlphaIISolution
                {
                    public static int Answer(int[] nums) => nums.Length;
                }
            }

            namespace DSAExperimentation.LeetCode.Conventions
            {
                internal static class LeetCodeAnswer
                {
                    public const int None = -1;
                }
            }

            namespace DSAExperimentation.LeetCode
            {
                internal static class RootRule
                {
                    public const int Value = 0;
                }
            }
            """;

    private static IEnumerable<string> ProblemFolders() =>
        Directory.EnumerateDirectories(Path.Combine(RepositoryFiles.Root(), SolutionProject))
            .Select(Path.GetFileName)
            .OfType<string>()
            .Where(folder => folder is not ("bin" or "obj") && !TierRootFolders.Contains(folder));

    // The folders of this project that checks.json lists as shared among check-duplicate-constant's units.
    private static List<string> DuplicateConstantSharedFolders()
    {
        var path = Path.Combine(RepositoryFiles.Root(), ChecksFile);
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var rows = document.RootElement
            .GetProperty(ChecksProperty)
            .GetProperty(DuplicateConstantCheck)
            .GetProperty(OptionsProperty)
            .GetProperty(IndependentUnitsOption)
            .EnumerateArray();

        return [.. rows
            .Where(row => row.TryGetProperty(SharedField, out _))
            .SelectMany(row => row.GetProperty(SharedField).EnumerateArray())
            .Select(glob => glob.GetString() ?? string.Empty)
            .Where(glob => glob.StartsWith(ProjectFolderPrefix, StringComparison.Ordinal))
            .Select(glob => glob[ProjectFolderPrefix.Length..])];
    }

    // A file directly under the project belongs to the tier's root; anything else to its folder.
    private static string ImporterOf(string relativePath)
    {
        var slash = relativePath.IndexOf('/');

        if (slash < 0)
        {
            return RootImporter;
        }

        return relativePath[..slash];
    }

    private static (string Importer, string Imported) EndsOf(string edge)
    {
        var arrow = edge.IndexOf(EdgeArrow, StringComparison.Ordinal);

        return (edge[..arrow], edge[(arrow + EdgeArrow.Length)..]);
    }

    private readonly record struct ProblemReference(string File, string Importer, string Imported)
    {
        public string Edge => $"{Importer}{EdgeArrow}{Imported}";
    }
}
