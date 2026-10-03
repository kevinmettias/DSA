using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 17.7, which until now was only prose: a
// harness holds assertions and workload sizing, and nothing else. The types that
// encode a problem's STRUCTURE - the topology a graph engine walks, the algebra a
// fold evaluates, the recurrence a memoizer replays, the virtual sequence a binary
// search bisects - are tier 1 to 4, and a copy of one sitting in a test folder or in
// Benchmarks/Fixtures is exactly the duplication section 17.1 set out to remove: the
// same witness written twice because a test and a benchmark cannot see each other's
// copy. FunctionalGraphTopology once existed three times for that reason.
//
// Hooks are a witness in a benchmark and not in a test. A recording IInOrderHooks
// that collects a traversal so a test can assert its order is an assertion device,
// which is the first of the two things 17.7 allows. A benchmark asserts nothing, so
// a hook there is the algorithm itself - KthSmallestElementInABSTBenchmarks once
// timed its own RankHooks while the solution's went untested.
//
// Every harness file is in scope: the whole catalogue has reached tier 4, so there
// is no unmigrated problem left for a harness to be waiting on.
public sealed partial class Tier5WitnessTests
{
    private const string BenchmarksProject = "DSAExperimentation.Benchmarks";
    private const string SolutionTestsProject = "DSAExperimentation.LeetCode.Tests";
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
        "IRecurrence",
    ];

    // The traversal engines' callbacks: an assertion device in a test, the algorithm
    // in a benchmark.
    private static readonly string[] TraversalHooks =
    [
        "IBreadthFirstHooks",
        "IDepthFirstHooks",
        "IInOrderHooks",
        "ILevelGroupedHooks",
        "ITopDownHooks",
    ];

    private static readonly string[] MeasurementWitnesses = [.. StructuralWitnesses, .. TraversalHooks];

    // `where T : class` followed by a second `where` clause is a constraint, not a type
    // named "where".
    private static readonly Regex TypeDeclaration = new(@"\b(?:class|struct|record|interface)\s+(?!where\b)[A-Za-z_]\w*");

    private static readonly Regex Constraint = new(@"\bwhere\b");

    [Fact]
    public void EveryHarness_DeclaresNoWitness()
    {
        var offences = StrandedWitnessesInHarnesses();
        var report = string.Join(Environment.NewLine, offences);

        Assert.True(offences.Count == 0, report);
    }

    private static List<string> StrandedWitnessesInHarnesses()
    {
        var root = RepositoryFiles.Root();
        var offences = new List<string>();

        foreach (var harness in HarnessFiles(root))
        {
            offences.AddRange(
                WitnessesIn(harness.File, harness.Contracts)
                    .Select(witness =>
                        $"{harness.Relative}: declares a {witness}, which encodes {harness.Subject}'s structure and "
                        + "belongs beside the engine that consumes it, not in a tier 5 harness."));
        }

        return offences;
    }

    [Fact]
    public void SharedBenchmarkFixtures_HoldOnlyWorkloadGenerators()
    {
        var root = RepositoryFiles.Root();
        var fixtures = Path.Combine(root, BenchmarksProject, FixturesFolder);
        var offences = new List<string>();

        foreach (var file in RepositoryFiles.SourceFilesIn(fixtures))
        {
            var relative = RepositoryFiles.PathFromRoot(root, file);

            offences.AddRange(
                WitnessesIn(file, MeasurementWitnesses)
                    .Select(witness =>
                        $"{relative}: declares a {witness}. Benchmarks/Fixtures holds workload generators - a seed "
                        + "and a size - not the structure they build."));
        }

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    // The scans above report "no offences" over whatever they find, so a scan that
    // found no files - a renamed folder, a moved project - would pass while checking
    // nothing.
    [Fact]
    public void HarnessScan_OverTheRepository_FindsBothTestAndBenchmarkHarnesses()
    {
        var harnesses = HarnessFiles(RepositoryFiles.Root()).ToList();

        Assert.Contains(harnesses, harness => harness.Contracts == StructuralWitnesses);
        Assert.Contains(harnesses, harness => harness.Contracts == MeasurementWitnesses);
    }

    // A base list that starts on the line after its type's name - the usual layout
    // for a long generic contract - is still a base list.
    [Fact]
    public void WitnessesIn_BaseListOnTheNextLine_FindsTheWitness()
    {
        var source = "private sealed class CanBuild(string target)\n    : IRecurrence<(int First, int Second), bool>\n{\n}";

        Assert.Equal(["IRecurrence"], WitnessesInSource(source, StructuralWitnesses));
    }

    [Fact]
    public void WitnessesIn_WitnessOnlyInAConstraintOrAComment_FindsNone()
    {
        var source = "// class Fake : IRecurrence<int, int>\n"
            + "internal static class Runner\n{\n"
            + "    public static int Run<TRule>() where TRule : IRecurrence<int, int> => 0;\n}";

        Assert.Empty(WitnessesInSource(source, StructuralWitnesses));
    }

    // Every harness file that belongs to one identifiable subject: each file under a
    // folder of the solution-tier test project (a problem's tests, or the catalog's), and
    // each benchmark class - named for a problem, or for a
    // library-level choice such as ShortestPathAlgorithm. Tests may declare hooks as
    // assertion devices; benchmarks may not.
    private static IEnumerable<HarnessFile> HarnessFiles(string root)
    {
        var coverage = Path.Combine(root, SolutionTestsProject);
        var solutions = Path.Combine(root, BenchmarksProject, "ProblemSolutions");

        foreach (var file in RepositoryFiles.SourceFilesIn(coverage))
        {
            var segments = RepositoryFiles.PathFromRoot(coverage, file).Split('/');

            if (segments.Length > 1)
            {
                yield return new HarnessFile(file, RepositoryFiles.PathFromRoot(root, file), segments[0], StructuralWitnesses);
            }
        }

        foreach (var file in RepositoryFiles.SourceFilesIn(solutions))
        {
            var name = Path.GetFileName(file);

            if (name.EndsWith(BenchmarkSuffix, StringComparison.Ordinal))
            {
                yield return new HarnessFile(
                    file, RepositoryFiles.PathFromRoot(root, file), name[..^BenchmarkSuffix.Length], MeasurementWitnesses);
            }
        }
    }

    private static IEnumerable<string> WitnessesIn(string file, string[] contracts)
        => WitnessesInSource(File.ReadAllText(file), contracts);

    // Read across lines rather than line by line: the base list is whatever follows
    // the declaration's first top-level colon, up to its body or its terminating
    // semicolon, wherever the line breaks fall.
    private static List<string> WitnessesInSource(string source, string[] contracts)
    {
        var code = RepositoryFiles.CodeOf(source);

        return TypeDeclaration.Matches(code)
            .Select(declaration => BaseListAfter(code, declaration.Index + declaration.Length))
            .SelectMany(baseList => contracts.Where(contract => IsNamedInBaseList((baseList, contract))))
            .Distinct()
            .ToList();
    }

    private static string BaseListAfter(string code, int headerStart)
    {
        var depth = 0;
        var colon = -1;
        var end = headerStart;

        for (; end < code.Length; end++)
        {
            var character = code[end];

            if (character is '(' or '<' or '[')
            {
                depth++;
            }
            else if (character is ')' or '>' or ']')
            {
                depth--;
            }
            else if (depth == 0 && character is '{' or ';')
            {
                break;
            }
            else if (depth == 0 && character == ':' && colon < 0)
            {
                colon = end;
            }
        }

        return colon < 0 ? string.Empty : WithoutConstraint(code[(colon + 1)..end]);
    }

    // Generic constraints are cut away before any name inside them is read: a type
    // whose `where` clause accepts a witness does not write one.
    private static string WithoutConstraint(string baseList)
    {
        var constraint = Constraint.Match(baseList);

        return constraint.Success ? baseList[..constraint.Index] : baseList;
    }

    // The pair travels as one value: the witness name is compared against the base
    // list it was found in, and as two adjacent strings a transposed call would
    // search for the base list inside the name and quietly match nothing.
    private static bool IsNamedInBaseList((string BaseList, string Witness) candidate)
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

    // One harness file: the path to read, the root-relative path a finding names it by,
    // the subject it measures or asserts, and the contracts it may not implement. Four
    // positions, two of them strings that a transposed call would swap silently.
    private readonly record struct HarnessFile(string File, string Relative, string Subject, string[] Contracts);
}
