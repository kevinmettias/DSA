using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 17.5: a benchmark's baseline is "what you would write without this
// repo", so nothing it reaches names a library type - not a container, not an engine, not even a
// constant such as ModularArithmetic.Modulo. The baseline is the arm a ProblemSolutions benchmark
// marks Baseline = true. Its trace starts at the solution method that arm calls - only the
// overloads its arguments fit - or the solution type it constructs, and follows every name that
// code spells, resolved as ProblemFolderSource describes, through its own problem folder and any
// problem it delegates to (section 17.3). The input the harness hands it is the one exception: a
// type named in the entry's own signature is the caller's choice, not part of the baseline's
// character, and its body is not traced.
//
// The trace reads the repository as text, like the other architecture tests, and resolves names
// without types, so it can still over-reach - most often through a shared method that calls a
// strategy interface, which links every implementation the folder constructs. A link of that kind
// is listed in AllowedReaches with the reason the baseline never runs the library code.
public sealed partial class BaselineIsolationTests
{
    private const string SolutionProject = "DSAExperimentation.LeetCode";
    private const string FrameworkProject = "DSAExperimentation";

    // Fewer baselines than this means the sweep stopped finding them, not that they are all clean.
    private const int ExpectedBaselinesAtLeast = 1000;

    private static readonly string BenchmarkFolder = Path.Combine("DSAExperimentation.Benchmarks", "ProblemSolutions");

    // The library a baseline may not name, by namespace: the framework's tiers, and the LeetCode
    // tier's Modular folder, whose modulus the owner ruled out of baselines by name.
    private static readonly string[] LibraryNamespaces =
    [
        "DSAExperimentation.DataStructures",
        "DSAExperimentation.Algorithms",
        "DSAExperimentation.Domain",
        "DSAExperimentation.LeetCode.Modular",
    ];

    // ...and by full name: the content-fixing models section 17.6 keeps beside the one problem each
    // serves, barred from a baseline exactly as a library type is, even inside their own folder.
    private static readonly string[] LibraryTypeNames =
    [
        "DSAExperimentation.LeetCode.OpenTheLock.LockGraph",
        "DSAExperimentation.LeetCode.OpenTheLock.LockNode",
        "DSAExperimentation.LeetCode.OpenTheLock.LockTopology",
        "DSAExperimentation.LeetCode.OpenTheLock.LockWheels",
        "DSAExperimentation.LeetCode.SlidingPuzzle.PuzzleGraph",
        "DSAExperimentation.LeetCode.SlidingPuzzle.PuzzleNode",
        "DSAExperimentation.LeetCode.SlidingPuzzle.PuzzleTopology",
    ];

    // The library types that stand for LeetCode's own I/O shapes - its TreeNode, ListNode and quad
    // tree Node, and the cache interface its LRU and LFU design problems are driven through. A
    // baseline that takes, returns or builds one answers in LeetCode's terms rather than composing
    // the library.
    private static readonly HashSet<string> ContractTypes =
        new(["BinaryTreeNode", "SinglyLinkedListNode", "QuadTreeNode", "ICache"], StringComparer.Ordinal);

    // Keyed by benchmark class: a baseline the trace finds reaching the library, and why that is
    // not a baseline written with the library.
    private static readonly Dictionary<string, string> AllowedReaches = new(StringComparer.Ordinal)
    {
        ["OpenTheLockBenchmarks"] =
            "Not yet BCL: the baseline still turns wheels through LockGraph.WheelTurnNeighbors. It "
            + "states its own wheel arithmetic once the lock model's move into this tier lands, and "
            + "this entry goes with it.",
        ["SlidingPuzzleBenchmarks"] =
            "Not yet BCL: the baseline still slides the blank through PuzzleGraph.BlankSlideNeighbors. "
            + "It states its own board arithmetic once the puzzle model's move into this tier lands, "
            + "and this entry goes with it.",
    };

    private static readonly Lazy<LibraryIndex> Library = new(LibraryIndex.Read);

    private static readonly Regex BaselineArm = new(@"\[Benchmark\(\s*Baseline\s*=\s*true\s*\)\]");
    private static readonly Regex ArmDeclaration = new(@"\bpublic\s+[\w<>\[\],.?() ]+?\s+\w+\s*\(");
    // A call into a solution class, up to its type arguments or argument list; `new X.Y(...)`
    // constructs one of its types instead.
    private static readonly Regex SolutionCall = new(@"(?<!new\s)\b(?<problem>\w+)Solution\s*\.\s*(?<member>\w+)\s*(?=[<(])");
    private static readonly Regex SolutionConstruction = new(@"\bnew\s+(?<problem>\w+)Solution\s*\.\s*(?<member>\w+)\s*[<(]");
    private static readonly Regex FieldDeclaration = new(
        """
        ^[ \t]*(?:(?:private|public|protected|internal|static|readonly|required)\s+)+
        (?<type>[\w<>\[\],.?() ]+?)\s+(?<name>\w+)\s*(?:;|=(?!>)|\{)
        """,
        RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
    private static readonly Regex CapitalizedName = new(@"(?<![\w.])(?<name>[A-Z]\w*)");
    private static readonly Regex QualifiedLibraryName = new(
        @"(?<![\w.])(?:DSAExperimentation\.)?(?:DataStructures|Algorithms|Domain|LeetCode\.Modular)\.[\w.]*\w");

    public static TheoryData<string, string[]> StrategyEntries =>
        new()
        {
            { "SolveByPlainWalk", [] },
            { "SolveByRepoStackWalk", ["RepoStack"] },
        };

    public static TheoryData<string?[], string[]> OverloadArguments =>
        new()
        {
            { ["int[]"], ["RepoStack"] },
            { ["RepoStack"], [] },
        };

    [Fact]
    public void EveryBaseline_ReachesNoLibraryType()
    {
        var offences = Offences()
            .Where(offence => !AllowedReaches.ContainsKey(offence.BenchmarkClass))
            .Select(offence => offence.Describe())
            .ToList();

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    // An entry whose link has gone would otherwise sit in the list excusing nothing.
    [Fact]
    public void AllowedReaches_AreAllStillReached()
    {
        var reached = Offences().Select(offence => offence.BenchmarkClass).ToHashSet();

        Assert.DoesNotContain(AllowedReaches.Keys, benchmark => !reached.Contains(benchmark));
    }

    // A sweep that found no baseline would report no offence while checking nothing.
    [Fact]
    public void BaselineSweep_FindsTheBenchmarkBaselines() =>
        Assert.True(BaselineEntries().Count >= ExpectedBaselinesAtLeast);

    [Fact]
    public void LibraryNamesReached_AliasForALibraryType_AreFound() =>
        Assert.Equal(
            ["RepoStack"],
            LibraryNamesReached(
                """
                using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

                namespace Probe;

                internal static class ProbeSolution
                {
                    public static int Solve()
                    {
                        var pending = new RepoStack();

                        return pending.Count;
                    }
                }
                """,
                "Solve"));

    [Fact]
    public void LibraryNamesReached_PartiallyQualifiedName_AreFound() =>
        Assert.Equal(
            ["DataStructures.Queue.Queue"],
            LibraryNamesReached(
                """
                namespace Probe;

                internal static class ProbeSolution
                {
                    public static int Solve() => new DataStructures.Queue.Queue<int>().Count;
                }
                """,
                "Solve"));

    [Fact]
    public void LibraryNamesReached_NameInACommentOrString_AreIgnored() =>
        Assert.Empty(LibraryNamesReached(
            """
            using DSAExperimentation.Domain.Modular;

            namespace Probe;

            internal static class ProbeSolution
            {
                // ModularArithmetic.Modulo
                public static string Solve() => "ModularArithmetic";
            }
            """,
            "Solve"));

    [Fact]
    public void LibraryNamesReached_TypeFromTheEntrySignature_AreAllowed() =>
        Assert.Empty(LibraryNamesReached(
            """
            using DSAExperimentation.DataStructures.Stack;

            namespace Probe;

            internal static class ProbeSolution
            {
                public static int Solve(Stack<int> prepared) => prepared.Count;
            }
            """,
            "Solve"));

    [Fact]
    public void LibraryNamesReached_FieldInitializerTheBaselineReads_AreFound() =>
        Assert.Equal(
            ["RepoStack"],
            LibraryNamesReached(
                """
                using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

                namespace Probe;

                internal static class ProbeSolution
                {
                    private static readonly RepoStack Shared = new();

                    public static int Solve() => Shared.Count;
                }
                """,
                "Solve"));

    // A design problem's factory returns the instance the benchmark drives: what it builds, not
    // an input, however the signature names it.
    [Fact]
    public void LibraryNamesReached_FolderTypeAFactoryReturns_AreFound() =>
        Assert.Equal(
            ["RepoStack"],
            LibraryNamesReached(
                """
                using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

                namespace Probe;

                internal static class ProbeSolution
                {
                    public static StackQueue CreateByTwoStacks() => new();

                    internal sealed class StackQueue
                    {
                        private readonly RepoStack _in = new();

                        public int Count => _in.Count;
                    }
                }
                """,
                "CreateByTwoStacks"));

    [Fact]
    public void LibraryNamesReached_FolderTypeNamedLikeALibraryType_AreNotTheLibrarys() =>
        Assert.Empty(LibraryNamesReached(
            """
            using DSAExperimentation.DataStructures.Stack;

            namespace Probe;

            internal static class ProbeSolution
            {
                public static int Solve() => new Stack().Depth;
            }

            internal sealed class Stack
            {
                public int Depth => 0;
            }
            """,
            "Solve"));

    [Fact]
    public void ReachedDeclarations_MethodCalledThroughAnother_AreReached()
    {
        var reached = ReachedNames(
            """
            namespace Probe;

            internal static class ProbeSolution
            {
                public static int Solve(int n) => Twice(n);

                private static int Twice(int n) => Add(n, n);

                private static int Add(int a, int b) => a + b;

                private static int Unused(int n) => n;
            }
            """,
            "Solve");

        Assert.Equal(["Add", "Solve", "Twice"], reached);
    }

    // Both strategy classes declare Walk; a shared driver calls it through the interface, so only
    // the class an entry constructs can be the one that runs.
    [Theory]
    [MemberData(nameof(StrategyEntries))]
    public void LibraryNamesReached_StrategyClassAnEntryConstructs_AreTheOnlyOnesFound(string entry, string[] expected) =>
        Assert.Equal(
            expected,
            LibraryNamesReached(
                """
                using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

                namespace Probe;

                internal static class ProbeSolution
                {
                    public static int SolveByPlainWalk() => Run(new PlainWalk());

                    public static int SolveByRepoStackWalk() => Run(new RepoStackWalk());

                    private static int Run(IWalk walk) => walk.Walk();

                    private interface IWalk
                    {
                        int Walk();
                    }

                    private sealed class PlainWalk : IWalk
                    {
                        public int Walk() => 1;
                    }

                    private sealed class RepoStackWalk : IWalk
                    {
                        public int Walk() => new RepoStack().Count;
                    }
                }
                """,
                entry));

    [Fact]
    public void LibraryNamesReached_TypeNamedOnlyAsTypeMember_AreOnlyThatMembers() =>
        Assert.Empty(LibraryNamesReached(
            """
            using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

            namespace Probe;

            internal static class ProbeSolution
            {
                public static int Solve() => Steps.Plain();
            }

            internal static class Steps
            {
                public static int Plain() => 1;

                public static int Composed() => new RepoStack().Count;
            }
            """,
            "Solve"));

    // The LeetCode-shaped overload builds the prepared input; the prepared one only reads it.
    [Theory]
    [MemberData(nameof(OverloadArguments))]
    public void LibraryNamesReached_OverloadTheArgumentsFit_AreItsAlone(string?[] argumentTypes, string[] expected) =>
        Assert.Equal(
            expected,
            LibraryNamesReached(
                """
                using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

                namespace Probe;

                internal static class ProbeSolution
                {
                    public static int Solve(int[] values) => Solve(new RepoStack(values));

                    public static int Solve(RepoStack prepared) => prepared.Count;
                }
                """,
                "Solve",
                argumentTypes));

    private static List<string> LibraryNamesReached(string source, string entry, IReadOnlyList<string?>? argumentTypes = null)
    {
        var folder = ProbeFolder(source);
        var probe = new BaselineEntry("ProbeBenchmarks.cs", "Probe", entry, argumentTypes);

        return LibraryNamesReachedFrom(probe, _ => folder);
    }

    private static List<string> ReachedNames(string source, string entry)
    {
        var folder = ProbeFolder(source);
        var probe = new BaselineEntry("ProbeBenchmarks.cs", "Probe", entry, null);
        var trace = Trace(probe, _ => folder);

        return [.. trace.Reached.Select(reached => reached.Declaration.Name).Distinct().Order(StringComparer.Ordinal)];
    }

    private static ProblemFolderSource ProbeFolder(string source)
    {
        var code = RepositoryFiles.CodeOf(source);

        return ProblemFolderSource.Of([("ProbeSolution.cs", code)]);
    }

    private static List<Offence> Offences()
    {
        var folders = new FolderCache();
        var offences = new List<Offence>();

        foreach (var entry in BaselineEntries())
        {
            var names = LibraryNamesReachedFrom(entry, folders.SourceOf);

            if (names.Count > 0)
            {
                offences.Add(new Offence(entry, names));
            }
        }

        return offences;
    }

    private static List<string> LibraryNamesReachedFrom(BaselineEntry entry, Func<string, ProblemFolderSource?> sourceOf)
    {
        var trace = Trace(entry, sourceOf);
        var names = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var reached in trace.Reached)
        {
            names.UnionWith(LibraryNamesIn(reached, trace.InputTypes));
        }

        return [.. names];
    }

    // Every declaration the entry reaches: its own, then everything a reached declaration links to
    // in its folder, and the entries of any problem it delegates to.
    private static BaselineTrace Trace(BaselineEntry entry, Func<string, ProblemFolderSource?> sourceOf)
    {
        var start = sourceOf(entry.Folder);

        if (start is null)
        {
            return new BaselineTrace([], new HashSet<string>());
        }

        var entries = start.EntriesNamed(entry.Member, entry.ArgumentTypes);
        var inputTypes = InputTypesOf(entries);
        var origins = entries.Select(declaration => new ReachedDeclaration(start, declaration));
        var reached = ReachedFrom(origins, inputTypes, sourceOf);

        return new BaselineTrace(reached, inputTypes);
    }

    // The input the harness hands the baseline: every type its entry's parameters name, and the
    // ContractTypes, LeetCode's own I/O shapes, wherever they appear. Any other type an entry only
    // returns is what it builds - a design problem's instance, or the library structure itself -
    // and is traced like the rest.
    private static HashSet<string> InputTypesOf(IEnumerable<SourceDeclaration> entries)
    {
        var inputTypes = new HashSet<string>(ContractTypes, StringComparer.Ordinal);

        foreach (var entry in entries)
        {
            var parameterList = OverloadFit.ParameterListOf(entry.Signature);
            inputTypes.UnionWith(CapitalizedNamesIn(parameterList));
        }

        return inputTypes;
    }

    private static IEnumerable<string> CapitalizedNamesIn(string code) =>
        CapitalizedName.Matches(code).Select(match => match.Groups["name"].Value);

    private static List<ReachedDeclaration> ReachedFrom(
        IEnumerable<ReachedDeclaration> origins, IReadOnlySet<string> inputTypes, Func<string, ProblemFolderSource?> sourceOf)
    {
        var reached = new List<ReachedDeclaration>();
        var seen = new HashSet<SourceDeclaration>();
        var pending = new Stack<ReachedDeclaration>(origins);

        while (pending.TryPop(out var next))
        {
            if (seen.Add(next.Declaration))
            {
                reached.Add(next);
                PushLinked(next, inputTypes, sourceOf, pending);
            }
        }

        return reached;
    }

    private static void PushLinked(
        ReachedDeclaration from,
        IReadOnlySet<string> inputTypes,
        Func<string, ProblemFolderSource?> sourceOf,
        Stack<ReachedDeclaration> pending)
    {
        foreach (var declaration in from.Folder.LinkedFrom(from.Declaration, inputTypes))
        {
            pending.Push(new ReachedDeclaration(from.Folder, declaration));
        }

        foreach (var delegated in DelegationsFrom(from, sourceOf))
        {
            pending.Push(delegated);
        }
    }

    // A call into another problem's Solution class, or a construction of one of its types: a
    // delegation edge (section 17.3) the baseline runs through into that folder.
    private static IEnumerable<ReachedDeclaration> DelegationsFrom(
        ReachedDeclaration from, Func<string, ProblemFolderSource?> sourceOf)
    {
        var text = from.Declaration.Text;
        var targets = SolutionCall.Matches(text).Concat(SolutionConstruction.Matches(text));

        foreach (var target in targets)
        {
            var other = sourceOf(target.Groups["problem"].Value);

            if (other is null || ReferenceEquals(other, from.Folder))
            {
                continue;
            }

            foreach (var declaration in other.EntriesNamed(target.Groups["member"].Value, null))
            {
                yield return new ReachedDeclaration(other, declaration);
            }
        }
    }

    // Library names a reached declaration spells: qualified ones outright, and simple names the
    // file's usings and aliases resolve to a library type - unless they are the entry's input.
    private static IEnumerable<string> LibraryNamesIn(ReachedDeclaration reached, IReadOnlySet<string> inputTypes)
    {
        var declaration = reached.Declaration;
        var code = reached.Folder.CodeOf(declaration);
        var scope = Library.Value.ScopeOf(code);
        var qualified = QualifiedLibraryName.Matches(declaration.Text).Select(match => match.Value);
        var simple = CapitalizedName.Matches(declaration.Text)
            .Select(match => match.Groups["name"].Value)
            .Where(name => !inputTypes.Contains(name) && IsLibraryName(name, reached.Folder, scope));

        return qualified.Concat(simple);
    }

    // A type the folder declares shadows any library type its usings import, as C# resolves it -
    // unless that folder type is itself barred by full name.
    private static bool IsLibraryName(string name, ProblemFolderSource folder, IReadOnlySet<string> scope)
    {
        if (folder.HasTypeNamed(name))
        {
            return Library.Value.IsBarredType($"{folder.Namespace}.{name}");
        }

        return scope.Contains(name);
    }

    // Every Baseline = true arm of every ProblemSolutions benchmark, with the solution member it reaches.
    private static List<BaselineEntry> BaselineEntries()
    {
        var directory = Path.Combine(RepositoryFiles.Root(), BenchmarkFolder);
        var entries = new List<BaselineEntry>();

        foreach (var file in RepositoryFiles.SourceFilesIn(directory))
        {
            var code = RepositoryFiles.CodeOf(File.ReadAllText(file));
            entries.AddRange(BaselineEntriesIn(Path.GetFileName(file.Path), code));
        }

        return entries;
    }

    private static IEnumerable<BaselineEntry> BaselineEntriesIn(string benchmark, string code)
    {
        var fieldTypes = FieldTypesOf(code);

        foreach (Match arm in BaselineArm.Matches(code))
        {
            var body = ArmBody(code, arm);

            foreach (var entry in EntriesInArm(benchmark, body, fieldTypes))
            {
                yield return entry;
            }
        }
    }

    // The benchmark class's fields and properties by name, with their simple type names - what an
    // arm's arguments are, as far as the text says.
    private static Dictionary<string, string> FieldTypesOf(string code)
    {
        var fieldTypes = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (Match field in FieldDeclaration.Matches(code))
        {
            fieldTypes[field.Groups["name"].Value] = OverloadFit.SimpleTypeName(field.Groups["type"].Value);
        }

        return fieldTypes;
    }

    private static string ArmBody(string code, Match arm)
    {
        var declaration = ArmDeclaration.Match(code, arm.Index + arm.Length);

        if (!declaration.Success)
        {
            return string.Empty;
        }

        var end = SourceSpans.BodyEnd(code, declaration.Index + declaration.Length - 1);

        return code[declaration.Index..end];
    }

    private static IEnumerable<BaselineEntry> EntriesInArm(
        string benchmark, string body, IReadOnlyDictionary<string, string> fieldTypes)
    {
        foreach (Match call in SolutionCall.Matches(body))
        {
            var open = SourceSpans.ArgumentListOpen(body, call.Index + call.Length);

            if (open < 0)
            {
                continue;
            }

            var close = SourceSpans.MatchingClose(body, open);
            var argumentTypes = OverloadFit.ArgumentTypesOf(body[(open + 1)..close], fieldTypes);

            yield return new BaselineEntry(benchmark, call.Groups["problem"].Value, call.Groups["member"].Value, argumentTypes);
        }

        foreach (Match construction in SolutionConstruction.Matches(body))
        {
            yield return new BaselineEntry(
                benchmark, construction.Groups["problem"].Value, construction.Groups["member"].Value, null);
        }
    }

    // One Baseline = true arm's way into a problem folder: the solution member it names and, for a
    // call, its arguments' types where the benchmark's text states them.
    private sealed record BaselineEntry(
        string Benchmark, string Folder, string Member, IReadOnlyList<string?>? ArgumentTypes);

    private readonly record struct ReachedDeclaration(ProblemFolderSource Folder, SourceDeclaration Declaration);

    private sealed record BaselineTrace(List<ReachedDeclaration> Reached, IReadOnlySet<string> InputTypes);

    private readonly record struct Offence(BaselineEntry Entry, List<string> Names)
    {
        public string BenchmarkClass => Path.GetFileNameWithoutExtension(Entry.Benchmark);

        public string Describe() =>
            $"{Entry.Benchmark}: baseline {Entry.Folder}Solution.{Entry.Member} reaches {string.Join(", ", Names)}. "
            + "A baseline is what you would write without this repo, so it states its own BCL version "
            + "(ARCHITECTURE.md 17.5); a name the trace links only through code the baseline never runs "
            + "goes in AllowedReaches with why.";
    }

    // Problem folders read once per sweep, by folder name.
    private sealed class FolderCache
    {
        private readonly Dictionary<string, ProblemFolderSource?> _sources = new(StringComparer.Ordinal);

        public ProblemFolderSource? SourceOf(string folder)
        {
            if (!_sources.TryGetValue(folder, out var source))
            {
                source = Read(folder);
                _sources[folder] = source;
            }

            return source;
        }

        private static ProblemFolderSource? Read(string folder)
        {
            var path = Path.Combine(RepositoryFiles.Root(), SolutionProject, folder);

            if (!Directory.Exists(path))
            {
                return null;
            }

            var files = RepositoryFiles.SourceFilesIn(path)
                .Select(file => (file.Path, RepositoryFiles.CodeOf(File.ReadAllText(file))));

            return ProblemFolderSource.Of(files);
        }
    }

    // The library's types by namespace, read once, and which simple names each file's usings and
    // aliases bring into scope from it.
    private sealed class LibraryIndex
    {
        private static readonly Regex NamespaceDeclaration = new(@"^namespace\s+(?<name>[\w.]+)", RegexOptions.Multiline);
        private static readonly Regex TypeDeclaration = new(
            @"\b(?:class|struct|record|interface|enum)\s+(?:struct\s+|class\s+)?(?<name>[A-Z]\w*)");
        private static readonly Regex UsingAlias = new(@"^\s*using\s+(?<alias>\w+)\s*=\s*(?<target>[\w.]+)", RegexOptions.Multiline);
        private static readonly Regex UsingNamespace = new(@"^\s*using\s+(?!static\b)(?<name>[\w.]+)\s*;", RegexOptions.Multiline);

        private readonly Dictionary<string, HashSet<string>> _typesByNamespace = new(StringComparer.Ordinal);
        private readonly HashSet<string> _barredTypes = new(LibraryTypeNames, StringComparer.Ordinal);
        private readonly Dictionary<string, HashSet<string>> _scopeByCode = new(StringComparer.Ordinal);

        public static LibraryIndex Read()
        {
            var index = new LibraryIndex();
            var root = RepositoryFiles.Root();
            var files = RepositoryFiles.SourceFilesIn(Path.Combine(root, FrameworkProject))
                .Concat(RepositoryFiles.SourceFilesIn(Path.Combine(root, SolutionProject)));

            foreach (var file in files)
            {
                var code = RepositoryFiles.CodeOf(File.ReadAllText(file));
                index.AddLibraryTypesIn(code);
            }

            index.AddBarredTypes();

            return index;
        }

        public bool IsBarredType(string fullName) => _barredTypes.Contains(fullName);

        // The simple names one file's usings and aliases resolve to a library type. Keyed by the
        // file's code rather than its name: the probe folders the parser tests build all name
        // their one file alike.
        public HashSet<string> ScopeOf(string code)
        {
            if (!_scopeByCode.TryGetValue(code, out var scope))
            {
                scope = ScopeIn(code);
                _scopeByCode[code] = scope;
            }

            return scope;
        }

        private static bool IsLibraryNamespace(string name) =>
            Array.Exists(LibraryNamespaces, library => name == library || name.StartsWith($"{library}.", StringComparison.Ordinal));

        private void AddLibraryTypesIn(string code)
        {
            var declared = NamespaceDeclaration.Match(code);

            if (!declared.Success || !IsLibraryNamespace(declared.Groups["name"].Value))
            {
                return;
            }

            var names = TypeDeclaration.Matches(code).Select(match => match.Groups["name"].Value);
            TypesIn(declared.Groups["name"].Value).UnionWith(names);
        }

        private void AddBarredTypes()
        {
            foreach (var fullName in LibraryTypeNames)
            {
                var lastDot = fullName.LastIndexOf('.');
                TypesIn(fullName[..lastDot]).Add(fullName[(lastDot + 1)..]);
            }
        }

        private HashSet<string> TypesIn(string namespaceName)
        {
            if (!_typesByNamespace.TryGetValue(namespaceName, out var names))
            {
                names = new HashSet<string>(StringComparer.Ordinal);
                _typesByNamespace[namespaceName] = names;
            }

            return names;
        }

        private HashSet<string> ScopeIn(string code)
        {
            var scope = new HashSet<string>(StringComparer.Ordinal);

            foreach (Match alias in UsingAlias.Matches(code))
            {
                if (IsLibraryAliasTarget(alias.Groups["target"].Value))
                {
                    scope.Add(alias.Groups["alias"].Value);
                }
            }

            foreach (Match imported in UsingNamespace.Matches(code))
            {
                scope.UnionWith(_typesByNamespace.GetValueOrDefault(imported.Groups["name"].Value) ?? []);
            }

            return scope;
        }

        private bool IsLibraryAliasTarget(string target) => IsLibraryNamespace(target) || _barredTypes.Contains(target);
    }
}
