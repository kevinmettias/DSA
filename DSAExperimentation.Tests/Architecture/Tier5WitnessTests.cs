namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 17.7, which until now was only prose: a
// harness holds assertions and workload sizing, and nothing else. The types that
// encode a problem's STRUCTURE - the topology a graph engine walks, the algebra a
// fold evaluates, the virtual sequence a binary search bisects - are tier 1 to 3,
// and a copy of one sitting in a test folder or in Benchmarks/Fixtures is exactly
// the duplication section 17.1 set out to remove: the same witness written twice
// because a test and a benchmark cannot see each other's copy. FunctionalGraph-
// Topology once existed three times for that reason; the ledger below is now empty
// because the last of those copies moved down a tier with its problem.
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

    // Stands in for the character just outside the list, so a candidate at either
    // end is compared against a non-identifier character rather than crashing.
    private const char OutsideList = ' ';

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
    private static readonly Dictionary<string, string> SharedFixtureStragglers = [];

    [Fact]
    public void EveryMigratedProblem_LeavesNoStructuralWitnessInItsHarnesses()
    {
        var offences = StrandedWitnessesInHarnesses();
        var report = string.Join(Environment.NewLine, offences);

        Assert.True(offences.Count == 0, report);
    }

    private static List<string> StrandedWitnessesInHarnesses()
    {
        var root = RepositoryFiles.Root();
        var offences = new List<string>();

        foreach (var (file, relative, problem) in HarnessFiles(root))
        {
            var solutionDirectory = Path.Combine(root, SolutionTier, problem);

            // A problem still waiting its turn keeps the old shape by design. It
            // comes under this rule the moment tier 4 gains its folder, which is
            // also the moment the witness has somewhere to go.
            if (Directory.Exists(solutionDirectory))
            {
                offences.AddRange(StrandedWitnessesIn((file, relative, problem)));
            }
        }

        return offences;
    }

    [Fact]
    public void SharedBenchmarkFixtures_HoldOnlyWorkloadGeneratorsAndListedStragglers()
    {
        var offences = UnlistedWitnessesInSharedFixtures();
        var report = string.Join(Environment.NewLine, offences);

        Assert.True(offences.Count == 0, report);
    }

    private static List<string> UnlistedWitnessesInSharedFixtures()
    {
        var root = RepositoryFiles.Root();
        var fixtures = Path.Combine(root, BenchmarksProject, FixturesFolder);
        var offences = new List<string>();

        foreach (var file in RepositoryFiles.SourceFilesIn(fixtures))
        {
            if (!SharedFixtureStragglers.ContainsKey(Path.GetFileName(file)))
            {
                var relative = RepositoryFiles.PathFromRoot(root, file);

                offences.AddRange(UnlistedWitnessesIn((file, relative)));
            }
        }

        return offences;
    }

    [Fact]
    public void SharedFixtureStragglers_AreAllStillPresent()
    {
        // A straggler that has moved down a tier is good news, but leaving its
        // line here would silently re-admit a file of that name later.
        var fixtures = Path.Combine(RepositoryFiles.Root(), BenchmarksProject, FixturesFolder);

        foreach (var (fileName, owner) in SharedFixtureStragglers)
        {
            var fixturePath = Path.Combine(fixtures, fileName);

            Assert.True(
                File.Exists(fixturePath),
                $"{fileName} is listed as a witness still stranded by {owner}, but it is gone - delete the line.");
        }
    }

    // The three positions travel as one value: they arrive together from HarnessFiles
    // and mean nothing apart, and spelled as three adjacent strings a transposed call
    // site would compile and blame the wrong file for the wrong problem.
    private static IEnumerable<string> StrandedWitnessesIn((string File, string Relative, string Problem) harness)
        => WitnessesIn(harness.File)
            .Select(witness =>
                $"{harness.Relative}: declares a {witness}, which encodes {harness.Problem}'s structure and belongs "
                + "beside the engine that consumes it, not in a tier 5 harness.");

    private static IEnumerable<string> UnlistedWitnessesIn((string File, string Relative) harness)
        => WitnessesIn(harness.File)
            .Select(witness =>
                $"{harness.Relative}: declares a {witness}. Benchmarks/Fixtures holds workload generators - a seed "
                + "and a size - not the structure they build.");

    // Every harness file that belongs to one identifiable problem: the test folder
    // named after it, and the single benchmark file named after it.
    private static IEnumerable<(string File, string Relative, string Problem)> HarnessFiles(string root)
    {
        var coverage = Path.Combine(root, TestsProject, "LeetCodeCoverage");
        var solutions = Path.Combine(root, BenchmarksProject, "ProblemSolutions");

        foreach (var file in RepositoryFiles.SourceFilesIn(coverage))
        {
            var segments = RepositoryFiles.PathFromRoot(coverage, file).Split('/');

            if (segments.Length > 1)
            {
                yield return (file, RepositoryFiles.PathFromRoot(root, file), segments[0]);
            }
        }

        foreach (var file in RepositoryFiles.SourceFilesIn(solutions))
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
            .SelectMany(baseList => StructuralWitnesses.Where(witness => NamesType((baseList, witness))))
            .Distinct();

    // The base list of a type declared on this line, or empty when the line
    // declares no type.
    private static string BaseListOf(string line)
    {
        var trimmed = line.TrimStart();
        var colon = trimmed.IndexOf(':');

        if (DeclaresNoType(trimmed, colon))
        {
            return string.Empty;
        }

        var baseList = trimmed[(colon + 1)..];
        var constraint = baseList.IndexOf(" where ", StringComparison.Ordinal);

        if (constraint < 0)
        {
            return baseList;
        }

        return WithoutConstraint(baseList, constraint);
    }

    // A comment, a line with no colon, and a line whose head names no type all have
    // the same answer: there is no base list here to read.
    private static bool DeclaresNoType(string trimmed, int colon)
        => trimmed.StartsWith("//", StringComparison.Ordinal)
            || colon < 0
            || !DeclaresType(trimmed[..colon]);

    // Generic constraints are cut away before any name inside them is read: a method
    // whose `where` clause accepts a witness does not write one.
    private static string WithoutConstraint(string baseList, int constraint)
        => baseList[..constraint];

    private static bool DeclaresType(string head)
        => head.Contains("class ", StringComparison.Ordinal)
            || head.Contains("struct ", StringComparison.Ordinal)
            || head.Contains("record ", StringComparison.Ordinal);

    // The pair travels as one value: the witness name is compared against the base
    // list it was found in, and as two adjacent strings a transposed call would
    // search for the base list inside the name and quietly match nothing.
    private static bool NamesType((string BaseList, string Witness) candidate)
    {
        for (var index = candidate.BaseList.IndexOf(candidate.Witness, StringComparison.Ordinal);
            index >= 0;
            index = candidate.BaseList.IndexOf(candidate.Witness, index + 1, StringComparison.Ordinal))
        {
            var end = index + candidate.Witness.Length;
            var before = CharacterBefore(candidate.BaseList, index);
            var after = CharacterAfter(candidate.BaseList, end);

            if (!IsIdentifierPart(before) && !IsIdentifierPart(after))
            {
                return true;
            }
        }

        return false;
    }

    private static char CharacterBefore(string baseList, int index)
    {
        if (index == 0)
        {
            return OutsideList;
        }

        return baseList[index - 1];
    }

    private static char CharacterAfter(string baseList, int end)
    {
        if (end >= baseList.Length)
        {
            return OutsideList;
        }

        return baseList[end];
    }

    private static bool IsIdentifierPart(char character)
        => char.IsLetterOrDigit(character) || character is '_' or '.';
}
