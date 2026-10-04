using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 17.3's rule inside the LeetCode tier: a problem folder reaches
// into another only along a named edge, a Delegation (two problems ask the same question under
// different constraints, and one owns the arms both run) or a Subroutine (one problem's answer runs
// another's algorithm as a step). Each edge is an entry below saying which it is and why. An owner
// imports no problem, so delegation never chains and never runs both ways, and the tier's root and
// Conventions/ import no problem at all.
//
// The scan reads qualified names as well as using directives: every problem's namespace sits inside
// DSAExperimentation.LeetCode, so a sibling's class resolves as `PermutationsII.PermutationsIISolution`
// with no using at all.
public sealed partial class ProblemImportTests
{
    private const string SolutionProject = "DSAExperimentation.LeetCode";
    private const string RootImporter = "(root)";
    private const string EdgeArrow = " -> ";
    private const string SolutionSuffix = "Solution.cs";

    // Folders at the tier's root that hold conventions every problem may use, not a problem.
    private static readonly string[] TierRootFolders = ["Conventions"];

    private static readonly string[] EdgeKinds = ["Delegation: ", "Subroutine: "];

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

    // A sibling namespace named by a using directive of any kind: plain, static or aliased.
    private static readonly Regex UsingDirective = new(
        @"^[ \t]*(?:global\s+)?using\s+(?:static\s+)?(?:\w+\s*=\s*)?DSAExperimentation\.LeetCode\.(?<name>\w+)",
        RegexOptions.Multiline);

    // Directive lines, blanked before the qualified-name scan so a using is not read twice and a
    // namespace declaration does not name its own folder.
    private static readonly Regex Directive = new(
        @"^[ \t]*(?:(?:global\s+)?using\s+[^;=]*(?:=[^;]*)?|namespace\s+[\w.]+)\s*[;{]",
        RegexOptions.Multiline);

    // The first segment of a qualified name, with or without the tier's own namespace in front. The
    // lookbehind keeps member access (`x.Name.Other`) from matching mid-chain.
    private static readonly Regex QualifiedName = new(
        @"(?<![\w.])(?:DSAExperimentation\s*\.\s*LeetCode\s*\.\s*)?(?<name>\w+)\s*\.\s*\w");

    [Fact]
    public void EveryProblemFolder_ReferencesAnotherOnlyAlongAnAllowedEdge()
    {
        var offences = ProblemReferences()
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
        var present = ProblemReferences().Select(reference => reference.Edge).ToHashSet();

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

    [Fact]
    public void ProblemReferencesIn_SelfAliasConventionsAndTheTierItself_FindsNone() =>
        Assert.Empty(ProblemReferencesIn(
            "Permutations",
            "using Self = DSAExperimentation.LeetCode.Permutations.PermutationsSolution;\n"
                + "using DSAExperimentation.LeetCode.Conventions;\nnamespace DSAExperimentation.LeetCode.Permutations;\n"
                + "var answer = LeetCodeAnswer.None;",
            ["Permutations", "PermutationsII"]));

    [Fact]
    public void ProblemReferencesIn_SiblingNamespaceWithNoUsing_FindsTheSibling() =>
        Assert.Equal(
            ["PermutationsII"],
            ProblemReferencesIn("Permutations", "var all = PermutationsII.PermutationsIISolution.Permute(nums);", ["Permutations", "PermutationsII"]));

    [Fact]
    public void ProblemReferencesIn_SiblingNamedInACommentOrString_FindsNone() =>
        Assert.Empty(ProblemReferencesIn(
            "Permutations",
            "// PermutationsII.PermutationsIISolution does the work\nvar text = \"PermutationsII.X\";",
            ["Permutations", "PermutationsII"]));

    [Fact]
    public void ProblemReferencesIn_StaticAndAliasedUsings_FindTheSibling() =>
        Assert.Equal(
            ["PermutationsII"],
            ProblemReferencesIn(
                "Permutations",
                "using static DSAExperimentation.LeetCode.PermutationsII.PermutationsIISolution;\n"
                    + "using Owner = DSAExperimentation.LeetCode.PermutationsII.PermutationsIISolution;",
                ["Permutations", "PermutationsII"]));

    // The problem folders a file names, other than its own: using directives first, then qualified
    // names in the code with directives blanked. Comments and strings never count.
    private static IEnumerable<string> ProblemReferencesIn(string importer, string source, ICollection<string> problems)
    {
        var code = RepositoryFiles.CodeOf(source);
        var named = UsingDirective.Matches(code).Select(match => match.Groups["name"].Value)
            .Concat(QualifiedName.Matches(Directive.Replace(code, string.Empty)).Select(match => match.Groups["name"].Value));

        return named.Where(name => problems.Contains(name) && name != importer).Distinct();
    }

    private static List<ProblemReference> ProblemReferences()
    {
        var project = Path.Combine(RepositoryFiles.Root(), SolutionProject);
        var problems = ProblemFolders().ToHashSet();
        var references = new List<ProblemReference>();

        foreach (var file in RepositoryFiles.SourceFilesIn(project))
        {
            var relative = RepositoryFiles.PathFromRoot(project, file);
            var importer = ImporterOf(relative);

            references.AddRange(
                ProblemReferencesIn(importer, File.ReadAllText(file), problems)
                    .Select(imported => new ProblemReference(relative, importer, imported)));
        }

        return references;
    }

    private static IEnumerable<string> ProblemFolders() =>
        Directory.EnumerateDirectories(Path.Combine(RepositoryFiles.Root(), SolutionProject))
            .Select(Path.GetFileName)
            .OfType<string>()
            .Where(folder => folder is not ("bin" or "obj") && !TierRootFolders.Contains(folder));

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
