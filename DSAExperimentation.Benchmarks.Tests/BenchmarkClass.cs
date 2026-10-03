using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.Tests;

// One benchmark class, driven the way BenchmarkDotNet drives it but at its cheapest point: every
// [Params] member at its smallest value, every [GlobalSetup] and [IterationSetup] that targets the
// arm, then the arm itself on a harness nothing else has touched. A fresh harness per arm matters
// because an arm may rewrite the workload in place, and the next arm must not inherit its result.
//
// An arm that takes its size as an argument - [Arguments] or an [ArgumentsSource], so that an
// exponential baseline can stop at sizes a polynomial arm runs far past - is called with the
// smallest size every arm of its class is measured at. Arms are only comparable on one workload,
// so a class whose arms share no size is refused rather than compared at two different ones.
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

    // The size every arm is compared at, worked out once and only when an arm first runs.
    private readonly Lazy<object?> _sharedArgument;

    private BenchmarkClass(Type type)
    {
        Type = type;
        Name = NameSuffixes
            .Where(suffix => type.Name.EndsWith(suffix, StringComparison.Ordinal))
            .Select(suffix => type.Name[..^suffix.Length])
            .FirstOrDefault(type.Name);
        Arms = [.. type.GetMethods().Where(IsArm).OrderBy(arm => arm.MetadataToken)];
        Baseline = Arms.FirstOrDefault(arm => arm.GetCustomAttribute<BenchmarkAttribute>() is { Baseline: true }) ?? Arms[0];
        _sharedArgument = new Lazy<object?>(SmallestSharedArgument);
    }

    public static IEnumerable<BenchmarkClass> All => ByName.Value.Values.OrderBy(benchmark => benchmark.Name, StringComparer.Ordinal);

    public Type Type { get; }

    public string Name { get; }

    public IReadOnlyList<MethodInfo> Arms { get; }

    // The arm BenchmarkDotNet reports the others relative to, or the first declared when none is marked.
    public MethodInfo Baseline { get; }

    public static BenchmarkClass Named(string name) => ByName.Value[name];

    // A class outside the benchmark assembly, for testing this type's own rules on samples.
    public static BenchmarkClass Of(Type type) => new(type);

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

    public object? Run(object harness, MethodInfo arm)
    {
        var arguments = arm.GetParameters().Length == 0 ? null : new[] { _sharedArgument.Value };
        var answer = arm.Invoke(harness, BindingFlags.DoNotWrapExceptions, binder: null, parameters: arguments, culture: null);

        return IsAnsweredByHarness(arm) ? harness : answer;
    }

    // The values one arm is measured at, from [Arguments] or an [ArgumentsSource] member - a
    // property, field or method, static or on a harness. Only single-argument arms are supported:
    // the argument is a size, and a second one would need a rule for which pairs to compare.
    public IReadOnlyList<object?> ArgumentsOf(MethodInfo arm)
    {
        if (arm.GetParameters().Length > 1)
        {
            throw new NotSupportedException($"{Type.Name}.{arm.Name} takes more than one argument.");
        }

        var listed = arm.GetCustomAttributes<ArgumentsAttribute>().Select(arguments => arguments.Values.Single());
        var sourced = arm.GetCustomAttribute<ArgumentsSourceAttribute>() is { } source
            ? ValuesFrom(source.Name)
            : [];

        return [.. listed.Concat(sourced)];
    }

    private IEnumerable<object?> ValuesFrom(string memberName)
    {
        const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        var member = Type.GetMember(memberName, Any).Single();
        var owner = member is MethodInfo { IsStatic: true } or PropertyInfo { GetMethod.IsStatic: true } or FieldInfo { IsStatic: true }
            ? null
            : Activator.CreateInstance(Type);

        var values = member switch
        {
            MethodInfo method => method.Invoke(owner, null),
            PropertyInfo property => property.GetValue(owner),
            FieldInfo field => field.GetValue(owner),
            _ => null,
        };

        return values is System.Collections.IEnumerable sequence
            ? sequence.Cast<object?>()
            : throw new NotSupportedException($"{Type.Name}.{memberName} does not yield a sequence of arguments.");
    }

    private object? SmallestSharedArgument()
    {
        var sized = Arms.Where(arm => arm.GetParameters().Length > 0).ToList();

        if (sized.Count == 0)
        {
            return null;
        }

        if (sized.Count < Arms.Count)
        {
            throw new InvalidOperationException($"{Type.Name}: some arms take a size and some do not, so no one workload runs them all.");
        }

        var shared = sized
            .Select(arm => ArgumentsOf(arm).ToHashSet())
            .Aggregate((left, right) => [.. left.Intersect(right)]);

        return shared.Count > 0
            ? shared.Min()
            : throw new InvalidOperationException($"{Type.Name}: its arms share no size, so no workload compares them.");
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
