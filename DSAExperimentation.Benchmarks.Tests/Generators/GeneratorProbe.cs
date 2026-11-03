using DSAExperimentation.Benchmarks.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DSAExperimentation.Benchmarks.Tests.Generators;

// Small compilations for the generator's tests: a probe source compiled against the running platform's
// assemblies, so a test states the solution or the type it is about as C# rather than building symbols by
// hand. The probe declares its own LeetCodeQuestionAttribute and BinaryTreeNode under the real names,
// which is all the generator matches them by.
internal static class GeneratorProbe
{
    public const string Conventions = """
        namespace DSAExperimentation.LeetCode.Solutions.Conventions
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

    private static readonly Lazy<MetadataReference[]> PlatformReferences = new(() =>
        [.. ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(path => MetadataReference.CreateFromFile(path))]);

    public static GeneratorDriverRunResult Generate(string source)
    {
        var driver = CSharpGeneratorDriver.Create(new CatalogBenchmarkGenerator()).RunGenerators(Compile(source));

        return driver.GetRunResult();
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
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
}
