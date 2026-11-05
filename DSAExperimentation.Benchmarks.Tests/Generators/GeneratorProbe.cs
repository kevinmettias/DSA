using DSAExperimentation.Benchmarks.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace DSAExperimentation.Benchmarks.Tests.Generators;

// Small compilations for the generator's tests: a probe source compiled against the running platform's
// assemblies, so a test states the solution or the type it is about as C# rather than building symbols by
// hand. The probe declares its own LeetCodeQuestionAttribute and BinaryTreeNode under the real names,
// which is all the generator matches them by. A probe compiles into one assembly, or into three laid out
// as the real projects are: the conventions, the solutions above them, and benchmarks above both.
internal static class GeneratorProbe
{
    public const string Conventions = """
        namespace DSAExperimentation.LeetCode
        {
            internal enum LeetCodeAnswerOrder { AsWritten, AnyOrder }

            [System.AttributeUsage(System.AttributeTargets.Class)]
            internal sealed class LeetCodeQuestionAttribute : System.Attribute
            {
                public LeetCodeQuestionAttribute(string titleSlug, LeetCodeAnswerOrder answerOrder) { }

                public bool RewritesInput { get; set; }

                public bool AnswersInPlace { get; set; }
            }
        }

        namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees
        {
            public sealed class BinaryTreeNode<T> { }
        }
        """;

    // The three-assembly layout's names, and what the conventions grant the two above them.
    private const string ConventionsAssembly = "ProbeConventions";
    private const string SolutionsAssembly = "ProbeSolutions";
    private const string BenchmarksAssembly = "ProbeBenchmarks";

    private const string ConventionsGrants = $$"""
        [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("{{SolutionsAssembly}}")]
        [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("{{BenchmarksAssembly}}")]
        """;

    // What the solutions grant the benchmarks above them, as the real solutions project does.
    private const string SolutionsGrants = $$"""
        [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("{{BenchmarksAssembly}}")]
        """;

    // This repository's own assemblies, which the test host trusts beside the platform's.
    private const string RepositoryAssemblyPrefix = "DSAExperimentation";

    private static readonly CSharpCompilationOptions Library = new(OutputKind.DynamicallyLinkedLibrary);

    private static readonly Lazy<MetadataReference[]> PlatformReferences = new(() =>
        [.. ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(path => MetadataReference.CreateFromFile(path))]);

    // The platform without this repository's assemblies. A probe split across assemblies declares the
    // attribute in one it references, where the real conventions would declare it a second time and leave
    // the generator no single type to match.
    private static readonly Lazy<MetadataReference[]> PlatformAlone = new(() =>
        [.. ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Where(path => !Path.GetFileName(path).StartsWith(RepositoryAssemblyPrefix, StringComparison.Ordinal))
            .Select(path => MetadataReference.CreateFromFile(path))]);

    public static GeneratorDriverRunResult Generate(string source)
    {
        var driver = CSharpGeneratorDriver.Create(new CatalogBenchmarkGenerator()).RunGenerators(Compile(source));

        return driver.GetRunResult();
    }

    // The generator over the probe with the catalog's file for one question beside it, as the benchmarks build
    // hands it each question's snapshot file.
    public static GeneratorDriverRunResult GenerateWithQuestion(string source, string slug, string questionJson)
    {
        AdditionalText[] files = [new QuestionFile($"C:/catalog/Snapshot/Questions/{slug}.json", questionJson)];
        var driver = CSharpGeneratorDriver.Create([new CatalogBenchmarkGenerator().AsSourceGenerator()], files).RunGenerators(Compile(source));

        return driver.GetRunResult();
    }

    // The generator over a benchmarks assembly that declares nothing, referencing the probe's solutions in
    // an assembly of their own and the conventions in another below them.
    public static GeneratorDriverRunResult GenerateFromReferences(string solutionSource)
    {
        var conventions = CSharpCompilation.Create(
            ConventionsAssembly, [CSharpSyntaxTree.ParseText(Conventions), CSharpSyntaxTree.ParseText(ConventionsGrants)], PlatformAlone.Value, Library);
        var solutions = CSharpCompilation.Create(
            SolutionsAssembly, [CSharpSyntaxTree.ParseText(solutionSource)], [.. PlatformAlone.Value, conventions.ToMetadataReference()], Library);
        var benchmarks = CSharpCompilation.Create(
            BenchmarksAssembly, [], [.. PlatformAlone.Value, conventions.ToMetadataReference(), solutions.ToMetadataReference()], Library);

        return CSharpGeneratorDriver.Create(new CatalogBenchmarkGenerator()).RunGenerators(benchmarks).GetRunResult();
    }

    // The same layout with the solutions and conventions emitted and referenced as assembly images, so the
    // generator reads the solutions from metadata as it reads the real project: there a static local
    // function is an internal method of its class, not a declaration inside a method body.
    public static GeneratorDriverRunResult GenerateFromEmittedReferences(string solutionSource)
    {
        var conventions = ImageOf(CSharpCompilation.Create(
            ConventionsAssembly, [CSharpSyntaxTree.ParseText(Conventions), CSharpSyntaxTree.ParseText(ConventionsGrants)], PlatformAlone.Value, Library));
        var solutions = ImageOf(CSharpCompilation.Create(
            SolutionsAssembly, [CSharpSyntaxTree.ParseText(solutionSource), CSharpSyntaxTree.ParseText(SolutionsGrants)], [.. PlatformAlone.Value, conventions], Library));
        var benchmarks = CSharpCompilation.Create(BenchmarksAssembly, [], [.. PlatformAlone.Value, conventions, solutions], Library);

        return CSharpGeneratorDriver.Create(new CatalogBenchmarkGenerator()).RunGenerators(benchmarks).GetRunResult();
    }

    // The compilation emitted, and referenced as the assembly it makes.
    private static MetadataReference ImageOf(CSharpCompilation compilation)
    {
        using var image = new MemoryStream();
        var emitted = compilation.Emit(image);

        if (!emitted.Success)
        {
            throw new InvalidOperationException(string.Join(Environment.NewLine, emitted.Diagnostics));
        }

        return MetadataReference.CreateFromImage(image.ToArray());
    }

    // The type of the probe's field named `name`, declared on a class named Holder.
    public static ITypeSymbol FieldType(string source, string name) =>
        Compile(source).GetTypeByMetadataName("Holder")?.GetMembers(name).OfType<IFieldSymbol>().Single().Type
            ?? throw new InvalidOperationException("The probe declares no class named Holder.");

    // The probe's static method named `name`, declared on a class named Holder.
    public static IMethodSymbol Method(string source, string name) =>
        Compile(source).GetTypeByMetadataName("Holder")?.GetMembers(name).OfType<IMethodSymbol>().Single()
            ?? throw new InvalidOperationException("The probe declares no class named Holder.");

    private static CSharpCompilation Compile(string source) =>
        CSharpCompilation.Create(
            "Probe",
            [CSharpSyntaxTree.ParseText(Conventions), CSharpSyntaxTree.ParseText(source)],
            PlatformReferences.Value,
            Library);

    // One question's snapshot file, held in memory.
    private sealed class QuestionFile(string path, string text) : AdditionalText
    {
        public override string Path { get; } = path;

        public override SourceText GetText(CancellationToken cancellationToken = default) => SourceText.From(text);
    }
}
