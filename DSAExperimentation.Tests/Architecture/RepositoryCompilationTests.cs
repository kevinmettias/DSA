using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Tests.Architecture;

// A compilation with errors still answers, but a name it could not bind answers with an error
// symbol, so a test reading it would pass over the very reference it was asked about. Each project
// compiles cleanly here, or the tests that read it are reading less than the source says.
public sealed partial class RepositoryCompilationTests
{
    [Fact]
    public void Framework_CompilesWithoutErrors() => AssertNoErrors(RepositoryCompilation.Framework);

    [Fact]
    public void Solutions_CompileWithoutErrors() => AssertNoErrors(RepositoryCompilation.Solutions);

    [Fact]
    public void Benchmarks_CompileWithoutErrors() => AssertNoErrors(RepositoryCompilation.Benchmarks);

    // A compilation of no files has no errors either, so each must hold a type its folders declare.
    public static TheoryData<string> TypesEachCompilationDeclares =>
        new()
        {
            "DSAExperimentation.DataStructures.AlgorithmConstants",
            "DSAExperimentation.LeetCode.Solutions.TwoSum.TwoSumSolution",
            "DSAExperimentation.Benchmarks.ProblemSolutions.TwoSumBenchmarks",
            "DSAExperimentation.Benchmarks.Fixtures.PolygonWorkloads",
        };

    [Theory]
    [MemberData(nameof(TypesEachCompilationDeclares))]
    public void Compilations_DeclareTheirProjectsTypes(string metadataName)
    {
        Compilation[] compilations = [RepositoryCompilation.Framework, RepositoryCompilation.Solutions, RepositoryCompilation.Benchmarks];
        var declaring = compilations.Where(compilation => compilation.Assembly.GetTypeByMetadataName(metadataName) is not null);

        Assert.Single(declaring);
    }

    private static void AssertNoErrors(Compilation compilation)
    {
        var errors = compilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .Select(diagnostic => diagnostic.ToString())
            .Take(20)
            .ToList();

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }
}
