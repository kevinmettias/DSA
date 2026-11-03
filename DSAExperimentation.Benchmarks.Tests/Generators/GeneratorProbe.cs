using DSAExperimentation.Benchmarks.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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
}
