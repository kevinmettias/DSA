using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.Tests;

// One benchmark class, driven the way BenchmarkDotNet drives it but at its cheapest point: every
// [Params] member at its smallest value, every [GlobalSetup] and [IterationSetup] that targets the
// arm, then the arm itself on a harness nothing else has touched. A fresh harness per arm matters
// because an arm may rewrite the workload in place, and the next arm must not inherit its result.
internal sealed class BenchmarkClass
{
    // A case is named by its class name less whichever of these suffixes it carries: xunit cuts a
    // theory argument at fifty characters, and the suffix is the part every name shares.
    private static readonly string[] NameSuffixes = ["Benchmarks", "Benchmark"];

    private static readonly Lazy<IReadOnlyDictionary<string, BenchmarkClass>> ByName = new(() =>
        typeof(BenchmarkConfig).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false, ContainsGenericParameters: false })
            .Where(type => type.GetMethods().Any(IsArm))
            .Select(type => new BenchmarkClass(type))
            .ToDictionary(benchmark => benchmark.Name));

    private BenchmarkClass(Type type)
    {
        Type = type;
        Name = NameSuffixes
            .Where(suffix => type.Name.EndsWith(suffix, StringComparison.Ordinal))
            .Select(suffix => type.Name[..^suffix.Length])
            .FirstOrDefault(type.Name);
        Arms = [.. type.GetMethods().Where(IsArm).OrderBy(arm => arm.MetadataToken)];
        Baseline = Arms.FirstOrDefault(arm => arm.GetCustomAttribute<BenchmarkAttribute>() is { Baseline: true }) ?? Arms[0];
    }

    public static IEnumerable<BenchmarkClass> All => ByName.Value.Values.OrderBy(benchmark => benchmark.Name, StringComparer.Ordinal);

    public Type Type { get; }

    public string Name { get; }

    public IReadOnlyList<MethodInfo> Arms { get; }

    // The arm BenchmarkDotNet reports the others relative to, or the first declared when none is marked.
    public MethodInfo Baseline { get; }

    public static BenchmarkClass Named(string name) => ByName.Value[name];

    public object Prepare(MethodInfo arm)
    {
        // BenchmarkDotNet itself requires a public parameterless constructor, and a class is never null.
        var harness = Activator.CreateInstance(Type)
            ?? throw new InvalidOperationException($"{Type.Name} could not be constructed.");

        foreach (var member in Type.GetMembers())
        {
            AssignSmallestParameter(harness, member);
        }

        RunSetups<GlobalSetupAttribute>(harness, arm);
        RunSetups<IterationSetupAttribute>(harness, arm);

        return harness;
    }

    public static object? Run(object harness, MethodInfo arm)
    {
        var answer = arm.Invoke(harness, BindingFlags.DoNotWrapExceptions, binder: null, parameters: null, culture: null);

        return IsAnsweredByHarness(arm) ? harness : answer;
    }

    // A void arm answers by what it leaves in the harness, so the harness itself is its answer.
    public static bool IsAnsweredByHarness(MethodInfo arm) => arm.ReturnType == typeof(void);

    private static bool IsArm(MethodInfo method) => method.IsDefined(typeof(BenchmarkAttribute));

    private static void AssignSmallestParameter(object harness, MemberInfo member)
    {
        if (member.IsDefined(typeof(ParamsSourceAttribute)) || member.IsDefined(typeof(ParamsAllValuesAttribute)))
        {
            throw new NotSupportedException($"{member.Name} draws its values from a source this check does not evaluate.");
        }

        var values = member.GetCustomAttribute<ParamsAttribute>()?.Values;

        if (values is null)
        {
            return;
        }

        var smallest = values.Min();

        switch (member)
        {
            case PropertyInfo property:
                property.SetValue(harness, Convert.ChangeType(smallest, property.PropertyType));
                break;
            case FieldInfo field:
                field.SetValue(harness, Convert.ChangeType(smallest, field.FieldType));
                break;
        }
    }

    private void RunSetups<TSetup>(object harness, MethodInfo arm)
        where TSetup : TargetedAttribute
    {
        foreach (var method in Type.GetMethods())
        {
            if (method.GetCustomAttribute<TSetup>() is { } setup && IsSetupFor(setup, arm))
            {
                method.Invoke(harness, BindingFlags.DoNotWrapExceptions, binder: null, parameters: null, culture: null);
            }
        }
    }

    // A setup that names no targets runs before every arm, as BenchmarkDotNet runs it.
    private static bool IsSetupFor(TargetedAttribute setup, MethodInfo arm) =>
        setup.Targets.Length == 0 || setup.Targets.Contains(arm.Name);
}
