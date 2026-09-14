namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 17.7, which until now was only prose: a
// harness holds assertions and workload sizing, and nothing else. The types that
// encode a problem's STRUCTURE - the topology a graph engine walks, the algebra a
// fold evaluates, the virtual sequence a binary search bisects - are tier 1 to 3,
// and a copy of one sitting in a test folder or in Benchmarks/Fixtures is exactly
// the duplication section 17.1 set out to remove: the same witness written twice
// because a test and a benchmark cannot see each other's copy. FunctionalGraph-
// Topology currently exists three times for that reason.
//
// Hooks are deliberately NOT on the list below. A recording IInOrderHooks that
// collects a traversal so a test can assert its order is an assertion device,
// which is the first of the two things 17.7 allows; a topology is not.
//
// The per-problem rule is scoped to problems that have already reached tier 4, so
// it tightens on its own as the migration proceeds: converting a problem brings
// its harnesses under the rule in the same commit, and nothing here needs editing
// to keep pace. Only the SHARED benchmark fixture folder needs a ledger, because
// it is flat and its stragglers sit in no folder that could key the rule.
public sealed class Tier5WitnessTests
{
    private const string BenchmarksProject = "DSAExperimentation.Benchmarks";
    private const string TestsProject = "DSAExperimentation.Tests";
    private const string SolutionTier = "DSAExperimentation.LeetCode";
    private const string BenchmarkSuffix = "Benchmarks.cs";
    private const string FixturesFolder = "Fixtures";

    // The contracts a witness implements. Every one of them is a parametrization
    // point of a tier 1 or tier 2 engine, so implementing one IS writing domain
    // code, wherever the file happens to sit.
    private static readonly string[] StructuralWitnesses =
    [
        "IGraphTopology",
        "ITreeTopology",
        "IDagTopology",
        "IEdgeTopology",
        "IEdges",
        "IChildren",
        "IChildOrder",
        "IFoldAlgebra",
        "IReduceAlgebra",
        "IFoldEvaluationStrategy",
        "IReduceOrderStrategy",
        "IPathHeuristic",
        "IHeapOrder",
        "ICombineOperation",
        "IRangeUpdateOperation",
        "IGroupOperation",
        "IScaledGroupOperation",
        "IRandomAccessSequence",
        "IIndexedSequence",
    ];

    // Witnesses stranded in the shared benchmark fixture folder, each with the
    // unmigrated problem it belongs to. Every one of these moves down a tier when
    // that problem is converted - delete the line then, which is what makes the
    // staleness check below a burn-down list rather than a permanent allowance.
    private static readonly Dictionary<string, string> SharedFixtureStragglers = new()
    {
        ["CircularArrayTopology.cs"] = "Shortest Distance To Target String In A Circular Array",
        ["FunctionalGraphTopology.cs"] =
            "Count Visited Nodes In A Directed Graph, which also keeps its own copy under the test "
            + "project - two copies of one witness, down from three now that Longest Cycle In A Graph "
            + "declares its own beside its solution",
        ["JumpGridChildren.cs"] = "Minimum Number Of Visited Cells In A Grid",
        ["JumpGridTopology.cs"] = "Minimum Number Of Visited Cells In A Grid",
        ["ReversalChildren.cs"] = "Minimum Reverse Operations",
        ["ReversalTopology.cs"] = "Minimum Reverse Operations",
    };

    [Fact]
    public void EveryMigratedProblem_LeavesNoStructuralWitnessInItsHarnesses()
    {
        var root = RepositoryFiles.Root();
        var offences = new List<string>();

        foreach (var (file, relative, problem) in HarnessFiles(root))
        {
            // A problem still waiting its turn keeps the old shape by design. It
            // comes under this rule the moment tier 4 gains its folder, which is
            // also the moment the witness has somewhere to go.
            if (Directory.Exists(Path.Combine(root, SolutionTier, problem)))
            {
                offences.AddRange(StrandedWitnessesIn(file, relative, problem));
            }
        }

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    [Fact]
    public void SharedBenchmarkFixtures_HoldOnlyWorkloadGeneratorsAndListedStragglers()
    {
        var root = RepositoryFiles.Root();
        var fixtures = Path.Combine(root, BenchmarksProject, FixturesFolder);
        var offences = new List<string>();

        foreach (var file in RepositoryFiles.SourceFilesIn(fixtures))
        {
            if (!SharedFixtureStragglers.ContainsKey(Path.GetFileName(file)))
            {
                offences.AddRange(UnlistedWitnessesIn(file, RepositoryFiles.PathFromRoot(root, file)));
            }
        }

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    [Fact]
    public void SharedFixtureStragglers_AreAllStillPresent()
    {
        // A straggler that has moved down a tier is good news, but leaving its
        // line here would silently re-admit a file of that name later.
        var fixtures = Path.Combine(RepositoryFiles.Root(), BenchmarksProject, FixturesFolder);

        foreach (var (fileName, owner) in SharedFixtureStragglers)
        {
            Assert.True(
                File.Exists(Path.Combine(fixtures, fileName)),
                $"{fileName} is listed as a witness still stranded by {owner}, but it is gone - delete the line.");
        }
    }

    private static IEnumerable<string> StrandedWitnessesIn(string file, string relative, string problem)
        => WitnessesIn(file)
            .Select(witness =>
                $"{relative}: declares a {witness}, which encodes {problem}'s structure and belongs beside "
                + "the engine that consumes it, not in a tier 5 harness.");

    private static IEnumerable<string> UnlistedWitnessesIn(string file, string relative)
        => WitnessesIn(file)
            .Select(witness =>
                $"{relative}: declares a {witness}. Benchmarks/Fixtures holds workload generators - a seed "
                + "and a size - not the structure they build.");

    // Every harness file that belongs to one identifiable problem: the test folder
    // named after it, and the single benchmark file named after it.
    private static IEnumerable<(string File, string Relative, string Problem)> HarnessFiles(string root)
    {
        var coverage = Path.Combine(root, TestsProject, "LeetCodeCoverage");

        foreach (var file in RepositoryFiles.SourceFilesIn(coverage))
        {
            var segments = RepositoryFiles.PathFromRoot(coverage, file).Split('/');

            if (segments.Length > 1)
            {
                yield return (file, RepositoryFiles.PathFromRoot(root, file), segments[0]);
            }
        }

        foreach (var file in RepositoryFiles.SourceFilesIn(Path.Combine(root, BenchmarksProject, "ProblemSolutions")))
        {
            var name = Path.GetFileName(file);

            if (name.EndsWith(BenchmarkSuffix, StringComparison.Ordinal))
            {
                yield return (file, RepositoryFiles.PathFromRoot(root, file), name[..^BenchmarkSuffix.Length]);
            }
        }
    }

    private static IEnumerable<string> WitnessesIn(string file)
        => File.ReadLines(file)
            .Select(BaseListOf)
            .SelectMany(baseList => StructuralWitnesses.Where(witness => NamesType(baseList, witness)))
            .Distinct();

    // The base list of a type declared on this line, or empty when the line
    // declares no type. Generic constraints are cut away first: a method whose
    // `where` clause accepts a witness does not write one.
    private static string BaseListOf(string line)
    {
        var trimmed = line.TrimStart();
        var colon = trimmed.IndexOf(':');

        if (trimmed.StartsWith("//", StringComparison.Ordinal) || colon < 0 || !DeclaresType(trimmed[..colon]))
        {
            return string.Empty;
        }

        var baseList = trimmed[(colon + 1)..];
        var constraint = baseList.IndexOf(" where ", StringComparison.Ordinal);

        return constraint < 0 ? baseList : baseList[..constraint];
    }

    private static bool DeclaresType(string head)
        => head.Contains("class ", StringComparison.Ordinal)
            || head.Contains("struct ", StringComparison.Ordinal)
            || head.Contains("record ", StringComparison.Ordinal);

    // Whole-identifier match, so ListChildren is not read as IChildren.
    private static bool NamesType(string baseList, string witness)
    {
        for (var index = baseList.IndexOf(witness, StringComparison.Ordinal);
            index >= 0;
            index = baseList.IndexOf(witness, index + 1, StringComparison.Ordinal))
        {
            var end = index + witness.Length;

            if (!IsIdentifierPart(index == 0 ? ' ' : baseList[index - 1])
                && !IsIdentifierPart(end >= baseList.Length ? ' ' : baseList[end]))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsIdentifierPart(char character)
        => char.IsLetterOrDigit(character) || character is '_' or '.';
}
