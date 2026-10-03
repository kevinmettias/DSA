using System.Collections.Concurrent;
using System.Reflection;
using BenchmarkDotNet.Running;

namespace DSAExperimentation.Benchmarks;

// Categorises every benchmark without a [BenchmarkCategory] on any of them: an arm's categories
// are the folder its class sits in and every library namespace any arm of its class reaches, so
// `--anyCategories DataStructures.SegmentTree` runs every benchmark that exercises a segment tree,
// and `--anyCategories StrategySwaps` runs the strategy-swap suites. The reach is the whole
// class's, not the arm's own, because BenchmarkDotNet filters arm by arm: categorised alone, the
// segment-tree arm would run without the BCL baseline it exists to be compared with. A tag written by hand on
// 1,100 classes would drift from the code the moment a solution changed primitive; this reads the
// relationship from the IL, so it cannot.
//
// Reach is followed through the benchmark, the solution tier and the library itself, so an arm
// that calls a solution that runs Dijkstra is also categorised by the heap Dijkstra uses.
// Constructing an object of one of those assemblies' types reaches all of that type's methods,
// because a design problem's arm calls its strategy through an interface the IL cannot see past.
// Every namespace prefix is a category of its own - DataStructures, DataStructures.Graph,
// DataStructures.Graph.Grids - so a filter can be as broad or as narrow as the question.
//
// Categories BenchmarkDotNet's own discoverer finds, from [BenchmarkCategory], are kept.
internal sealed class LibraryCategoryDiscoverer : ICategoryDiscoverer
{
    private const string RepositoryPrefix = "DSAExperimentation.";
    private const char NamespaceSeparator = '.';

    // The library tiers a category may name. The solution tier and the harnesses are where the
    // reach starts, not what it is looking for.
    private static readonly string[] LibraryTiers = ["DataStructures", "Algorithms", "Domain"];

    private readonly ConcurrentDictionary<Type, SortedSet<string>> _classCategories = new();

    public static LibraryCategoryDiscoverer Instance { get; } = new();

    public string[] GetCategories(MethodInfo method)
    {
        var categories = new SortedSet<string>(DefaultCategoryDiscoverer.Instance.GetCategories(method), StringComparer.Ordinal);

        if (method.DeclaringType is { } benchmark)
        {
            categories.UnionWith(_classCategories.GetOrAdd(benchmark, CategoriesOfClass));
        }

        return [.. categories];
    }

    private static SortedSet<string> CategoriesOfClass(Type benchmark)
    {
        var categories = new SortedSet<string>(StringComparer.Ordinal);

        if (FolderOf(benchmark) is { } folder)
        {
            categories.Add(folder);
        }

        foreach (var arm in benchmark.GetMethods().Where(method => method.IsDefined(typeof(BenchmarkAttribute))))
        {
            foreach (var reachedNamespace in LibraryNamespacesReachedFrom(arm))
            {
                categories.UnionWith(PrefixesOf(reachedNamespace));
            }
        }

        return categories;
    }

    // DSAExperimentation.Benchmarks.ProblemSolutions -> ProblemSolutions; a class at the
    // project's root has no folder.
    private static string? FolderOf(Type benchmark)
    {
        var rootNamespace = typeof(LibraryCategoryDiscoverer).Namespace;
        var benchmarkNamespace = benchmark.Namespace;

        return benchmarkNamespace is not null && benchmarkNamespace.StartsWith(rootNamespace + NamespaceSeparator, StringComparison.Ordinal)
            ? benchmarkNamespace[(rootNamespace!.Length + 1)..]
            : null;
    }

    private static SortedSet<string> LibraryNamespacesReachedFrom(MethodInfo arm)
    {
        var reached = new SortedSet<string>(StringComparer.Ordinal);
        var visited = new HashSet<MethodBase>();
        var pending = new Queue<MethodBase>();
        pending.Enqueue(arm);

        while (pending.TryDequeue(out var method))
        {
            if (!visited.Add(method))
            {
                continue;
            }

            var references = MethodReferences.Of(method);

            foreach (var type in references.Types.Concat(references.Methods.Select(target => target.DeclaringType)).OfType<Type>())
            {
                NoteLibraryNamespaces(type, reached);
            }

            foreach (var target in references.Methods.Where(IsRepositoryCode))
            {
                pending.Enqueue(target);

                if (target is ConstructorInfo { DeclaringType: { } constructed })
                {
                    EnqueueMethodsOf(constructed, pending);
                }
            }
        }

        return reached;
    }

    private static void EnqueueMethodsOf(Type type, Queue<MethodBase> pending)
    {
        const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            | BindingFlags.Static | BindingFlags.DeclaredOnly;

        foreach (var method in type.GetMethods(Declared))
        {
            pending.Enqueue(method);
        }
    }

    // A type and every type argument it was built from, so ListEdges<WeightedAdjacencyNode<int>, int> notes the
    // edge list's namespace and a Dijkstra over a library topology notes the topology's.
    private static void NoteLibraryNamespaces(Type type, SortedSet<string> reached)
    {
        var element = type.HasElementType ? type.GetElementType() : type;

        if (element is null || element.IsGenericParameter)
        {
            return;
        }

        if (LibraryNamespaceOf(element) is { } libraryNamespace)
        {
            reached.Add(libraryNamespace);
        }

        if (element.IsConstructedGenericType)
        {
            foreach (var argument in element.GetGenericArguments())
            {
                NoteLibraryNamespaces(argument, reached);
            }
        }
    }

    // DSAExperimentation.DataStructures.Heap -> DataStructures.Heap; anything outside the
    // library tiers has none.
    private static string? LibraryNamespaceOf(Type type)
    {
        var typeNamespace = type.Namespace;

        if (typeNamespace is null || !typeNamespace.StartsWith(RepositoryPrefix, StringComparison.Ordinal))
        {
            return null;
        }

        var relative = typeNamespace[RepositoryPrefix.Length..];
        var tier = relative.Split(NamespaceSeparator)[0];

        return LibraryTiers.Contains(tier) ? relative : null;
    }

    // Repository code is followed; the BCL and BenchmarkDotNet are not, since nothing they call
    // can reach back into the library.
    private static bool IsRepositoryCode(MethodBase method)
        => method.DeclaringType?.Assembly.GetName().Name?.StartsWith(RepositoryPrefix.TrimEnd(NamespaceSeparator), StringComparison.Ordinal) == true;

    private static IEnumerable<string> PrefixesOf(string libraryNamespace)
    {
        for (var end = libraryNamespace.IndexOf(NamespaceSeparator); end > 0; end = libraryNamespace.IndexOf(NamespaceSeparator, end + 1))
        {
            yield return libraryNamespace[..end];
        }

        yield return libraryNamespace;
    }
}
