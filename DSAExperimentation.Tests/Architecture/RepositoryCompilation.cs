using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DSAExperimentation.Tests.Architecture;

// The framework, solution and benchmark projects compiled from their source with Roslyn, each
// against the one below it, so an architecture test can ask what a name binds to rather than how it
// is spelled. Each compilation is handed what MSBuild hands the compiler: the shared framework, the
// SDK's implicit usings and the project's own <Using> items, its InternalsVisibleTo grants, and
// nullable on. The benchmark project is compiled only where problems are measured, ProblemSolutions
// and Fixtures: the rest drives BenchmarkDotNet's runner, which no benchmark arm enters, and binds to
// more of BenchmarkDotNet than its attributes. It binds to the built LeetCode catalog rather than its
// source, as it does to BenchmarkDotNet: the suite benchmarks read their cases from it only in
// [GlobalSetup], and no rule here judges the catalog. Compiled once per test run.
internal static class RepositoryCompilation
{
    public const string FrameworkProject = "DSAExperimentation";
    public const string SolutionProject = "DSAExperimentation.LeetCode";
    public const string BenchmarkProject = "DSAExperimentation.Benchmarks";

    // The implicit usings Microsoft.NET.Sdk adds to a project that enables them, as every project
    // here does through Directory.Build.props.
    private static readonly string[] ImplicitUsings =
        ["System", "System.Collections.Generic", "System.IO", "System.Linq", "System.Net.Http", "System.Threading", "System.Threading.Tasks"];

    private static readonly string[] MeasuredBenchmarkFolders = ["ProblemSolutions", "Fixtures"];

    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.CSharp14);

    private static readonly Lazy<CompiledProjects> Compiled = new(Compile);

    public static CSharpCompilation Framework => Compiled.Value.Framework;

    public static CSharpCompilation Solutions => Compiled.Value.Solutions;

    public static CSharpCompilation Benchmarks => Compiled.Value.Benchmarks;

    private static CompiledProjects Compile()
    {
        var root = RepositoryFiles.Root();
        var shared = SharedFrameworkReferences();
        var framework = CompileProject(root, FrameworkProject, [FrameworkProject], shared);
        var solutions = CompileProject(root, SolutionProject, [SolutionProject], [.. shared, framework.ToMetadataReference()]);
        var benchmarks = CompileProject(
            root,
            BenchmarkProject,
            [.. MeasuredBenchmarkFolders.Select(folder => Path.Combine(BenchmarkProject, folder))],
            [
                .. shared,
                framework.ToMetadataReference(),
                solutions.ToMetadataReference(),
                MetadataReference.CreateFromFile(typeof(BenchmarkDotNet.Attributes.BenchmarkAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DSAExperimentation.LeetCode.Catalog.LeetCodeCatalog).Assembly.Location),
            ]);

        return new CompiledProjects(framework, solutions, benchmarks);
    }

    // The runtime's own assemblies: those the host trusts from the folder System.Object loads from.
    private static List<MetadataReference> SharedFrameworkReferences()
    {
        var runtime = Path.GetDirectoryName(typeof(object).Assembly.Location)
            ?? throw new InvalidOperationException("System.Object loaded from no folder, so the shared framework cannot be found.");
        var trusted = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string
            ?? throw new InvalidOperationException("The host lists no trusted platform assemblies, so the shared framework cannot be found.");

        return
        [
            .. trusted
                .Split(Path.PathSeparator)
                .Where(path => string.Equals(Path.GetDirectoryName(path), runtime, StringComparison.OrdinalIgnoreCase))
                .Select(path => MetadataReference.CreateFromFile(path)),
        ];
    }

    private static CSharpCompilation CompileProject(
        string root, string project, string[] sourceFolders, IEnumerable<MetadataReference> references)
    {
        var projectFile = XDocument.Load(Path.Combine(root, project, $"{project}.csproj"));
        var sources = sourceFolders
            .SelectMany(folder => RepositoryFiles.SourceFilesIn(Path.Combine(root, folder)))
            .Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), ParseOptions, file.Path));
        var generated = CSharpSyntaxTree.ParseText(GeneratedSourceOf(projectFile), ParseOptions, $"{project}.Generated.cs");
        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable);

        return CSharpCompilation.Create(project, [.. sources, generated], references, options);
    }

    // What MSBuild writes into obj/ before compiling: a global using for each implicit using and
    // each <Using> item, and an InternalsVisibleTo attribute for each <InternalsVisibleTo> item.
    private static string GeneratedSourceOf(XDocument projectFile)
    {
        var usings = ImplicitUsings.Concat(ItemsOf(projectFile, "Using")).Select(name => $"global using {name};");
        var grants = ItemsOf(projectFile, "InternalsVisibleTo")
            .Select(assembly => $"[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(\"{assembly}\")]");

        return string.Join(Environment.NewLine, usings.Concat(grants));
    }

    // The items a project file includes by name. An item that updates or removes one instead has no
    // Include; none of the project files here writes one.
    private static IEnumerable<string> ItemsOf(XDocument projectFile, string itemName)
        => projectFile.Descendants(itemName).Select(item => (string?)item.Attribute("Include")).OfType<string>();

    private sealed record CompiledProjects(CSharpCompilation Framework, CSharpCompilation Solutions, CSharpCompilation Benchmarks);
}
