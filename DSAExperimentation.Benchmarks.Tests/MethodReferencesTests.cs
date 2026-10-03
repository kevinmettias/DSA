using System.Reflection;

namespace DSAExperimentation.Benchmarks.Tests;

// MethodReferences reads IL by hand, and a mistake in its operand table would categorise
// benchmarks by the wrong code. The reader is strict - it throws on an undefined opcode or an
// instruction that overruns the body - so reading every method the categorisation can reach is
// the test that a misread operand fails, alongside bodies whose references are known.
public sealed partial class MethodReferencesTests
{
    [Fact]
    public void Of_MethodThatConstructsAndCalls_NamesTheConstructorAndTheCallee()
    {
        var references = MethodReferences.Of(MethodOf(nameof(Sample.ConstructsAndCalls)));

        Assert.Contains(references.Methods, method => method is ConstructorInfo { DeclaringType.Name: nameof(Sample) });
        Assert.Contains(references.Methods, method => method.Name == nameof(Sample.Callee));
    }

    // Every opcode shape the compiler emits occurs somewhere in these three assemblies - switch
    // jump tables of every length among them - so an operand read at the wrong size desynchronises
    // some body into an undefined opcode or an overrun, and the strict reader throws.
    [Fact]
    public void Of_EveryMethodTheCategorisationCanReach_ReadsEachBodyExactlyToItsEnd()
    {
        var unreadable = RepositoryMethods()
            .Select(method => (method, Error: Record.Exception(() => MethodReferences.Of(method))))
            .Where(read => read.Error is not null)
            .Select(read => read.Error!.Message)
            .ToList();

        Assert.True(unreadable.Count == 0, string.Join(Environment.NewLine, unreadable.Take(10)));
    }

    [Fact]
    public void Of_MethodWithNoBody_NamesNothing()
    {
        var references = MethodReferences.Of(typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose))!);

        Assert.Empty(references.Methods);
        Assert.Empty(references.Types);
    }

    private static IEnumerable<MethodBase> RepositoryMethods()
    {
        const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            | BindingFlags.Static | BindingFlags.DeclaredOnly;
        var benchmarks = typeof(BenchmarkConfig).Assembly;
        var assemblies = benchmarks.GetReferencedAssemblies()
            .Where(name => name.Name?.StartsWith(nameof(DSAExperimentation), StringComparison.Ordinal) == true)
            .Select(System.Reflection.Assembly.Load)
            .Prepend(benchmarks);

        return assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetMethods(Declared).Cast<MethodBase>().Concat(type.GetConstructors(Declared)));
    }

    private static MethodInfo MethodOf(string name)
        => typeof(Sample).GetMethod(name, BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException($"{nameof(Sample)} has no method named {name}.");

    public sealed class Sample
    {
        public static int ConstructsAndCalls() => Callee(new Sample());

        public static int Callee(Sample? sample) => sample is null ? 0 : 1;
    }
}
