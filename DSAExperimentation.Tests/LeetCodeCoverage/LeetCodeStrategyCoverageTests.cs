using System.Reflection;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage;

// Section 17.3 says a solution class holds EVERY strategy for its problem, each
// named <Operation>By<Strategy>; section 17.4 lets a strategy carry a second,
// hoisted overload for a pre-built domain object. Nothing checked that the
// registration then names them all - so a solution could grow a third arm that no
// case asserts and no benchmark measures, which is the same "the naive baseline
// was never tested" gap the five-tier reorg set out to close, only relocated from
// the per-problem test file to the registration.
//
// The pairing is by namespace, which is what the file tree already asserts: one
// problem folder holds one <Problem>Solution and one <Problem>Registration.
//
// The registration pairing reached ten problems. The rule below reaches every
// solution class: section 17.7 asks for one test method per strategy, named for it,
// and suppressions.json waives check-test-coverage over DSAExperimentation.LeetCode/**
// on the strength of exactly that claim. Until this check existed the claim was
// prose - KthSmallestElementInABSTSolution had no caller at all while its test
// asserted a private copy of both walks, and the waiver hid it.
public sealed partial class LeetCodeStrategyCoverageTests
{
    private const string StrategyInfix = "By";
    private const string SolutionSuffix = "Solution";
    private const char TestNameSeparator = '_';

    public static TheoryData<string> SolutionClasses
    {
        get
        {
            var solutionTypeNames = new TheoryData<string>();

            foreach (var solution in LeetCodeTypes().Where(IsSolutionClass).OrderBy(FullNameOf, StringComparer.Ordinal))
            {
                solutionTypeNames.Add(FullNameOf(solution));
            }

            return solutionTypeNames;
        }
    }

    // A strategy is named by a test method whose name is the member's name or starts
    // with it as the first underscore-separated word - the Member_Scenario_Expectation
    // shape check-test-coverage also reads - in the test namespace that mirrors the
    // solution's own problem folder.
    [Theory]
    [MemberData(nameof(SolutionClasses))]
    public void EveryStrategy_IsNamedByATestMethodInItsProblemFolder(string solutionTypeName)
    {
        var solution = LeetCodeTypes().Single(type => FullNameOf(type) == solutionTypeName);
        var strategies = StrategyMembersOf(solution);
        var testMethods = TestMethodNamesFor(solution);
        var unnamed = strategies.Where(strategy => !testMethods.Any(test => Names(test, strategy))).ToList();

        Assert.NotEmpty(strategies);
        Assert.True(
            unnamed.Count == 0,
            $"{solution.Name}: no test method in {TestNamespaceFor(solution)} is named for [{Listed(unnamed)}]. "
            + "Section 17.7 asks for one test method per strategy, starting with the strategy's own name.");
    }

    // The theory above has one row per solution class, so a discovery that found none
    // would report green while checking nothing.
    [Fact]
    public void SolutionClasses_AfterDiscovery_AreFound() => Assert.NotEmpty(SolutionClasses);

    private static bool Names(string testMethod, string strategy)
        => testMethod == strategy
            || (testMethod.StartsWith(strategy, StringComparison.Ordinal)
                && testMethod[strategy.Length] == TestNameSeparator);

    // The non-private members a strategy name can sit on: a static method for a
    // function-shaped problem, a nested type for a design problem whose strategies are
    // whole classes. Compiler-generated members - lambdas, local functions - carry '<'
    // in their names and are not the author's.
    private static List<string> StrategyMembersOf(Type solution)
    {
        var methods = solution
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => (method.IsPublic || method.IsAssembly) && !method.IsSpecialName)
            .Select(method => method.Name);
        var nestedTypes = solution
            .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
            .Where(type => type.IsNestedPublic || type.IsNestedAssembly)
            .Select(type => type.Name);

        return methods
            .Concat(nestedTypes)
            .Where(name => !name.Contains('<') && StrategyNameOf(name).Length > 0)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToList();
    }

    private static List<string> TestMethodNamesFor(Type solution)
    {
        var testNamespace = TestNamespaceFor(solution);

        return typeof(LeetCodeStrategyCoverageTests).Assembly
            .GetTypes()
            .Where(type => type.Namespace == testNamespace)
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            .Where(method => method.IsDefined(typeof(FactAttribute), inherit: true))
            .Select(method => method.Name)
            .ToList();
    }

    // DSAExperimentation.LeetCode.<Problem> is tested in <this namespace>.<Problem>.
    private static string TestNamespaceFor(Type solution)
        => $"{typeof(LeetCodeStrategyCoverageTests).Namespace}.{NamespaceOf(solution).Split('.')[^1]}";

    public static TheoryData<string> RegisteredSolutions
    {
        get
        {
            var solutionTypeNames = new TheoryData<string>();

            foreach (var (solution, _) in Pairs())
            {
                solutionTypeNames.Add(FullNameOf(solution));
            }

            return solutionTypeNames;
        }
    }

    [Theory]
    [MemberData(nameof(RegisteredSolutions))]
    public void Registration_NamesEveryStrategyItsSolutionExposes(string solutionTypeName)
    {
        var (solution, registration) = Pairs().Single(pair => FullNameOf(pair.Solution) == solutionTypeName);
        var problem = RegistrationOf(registration).Describe();
        var exposed = StrategyNamesOf(solution);

        Assert.True(
            exposed.SetEquals(problem.StrategyNames),
            $"{problem.TitleSlug}: {solution.Name} exposes [{Listed(exposed)}] but the registration names "
            + $"[{Listed(problem.StrategyNames)}]. Every strategy is measured and asserted, or none of them is.");
    }

    // A pairing that found nothing would leave the theory above with no rows and
    // still report green, which looks exactly like every registration passing.
    [Fact]
    public void EveryRegistration_IsPairedWithASolutionClass()
    {
        var paired = Pairs().Select(pair => pair.Registration).ToHashSet();
        var unpaired = LeetCodeTypes().Where(IsRegistration).Where(type => !paired.Contains(type)).ToList();

        Assert.Empty(unpaired.Select(type => type.FullName));
        Assert.Equal(LeetCodeProblemRegistry.All.Count, paired.Count);
    }

    private static string Listed(IEnumerable<string> names)
        => string.Join(", ", names.Order(StringComparer.Ordinal));

    private static IEnumerable<(Type Solution, Type Registration)> Pairs()
    {
        var registrations = LeetCodeTypes()
            .Where(IsRegistration)
            .ToDictionary(NamespaceOf, type => type, StringComparer.Ordinal);

        return LeetCodeTypes()
            .Where(IsSolutionClass)
            .Where(type => registrations.ContainsKey(NamespaceOf(type)))
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .Select(type => (type, registrations[NamespaceOf(type)]));
    }

    private static IEnumerable<Type> LeetCodeTypes()
        => typeof(LeetCodeProblem).Assembly.GetTypes().Where(type => type.Namespace is not null);

    // The compiler cannot narrow a Type property through the lambda in LeetCodeTypes(),
    // so the two invariants it already established are stated here. Assembly.GetTypes()
    // hands back type definitions, and FullName is null only for a bare generic
    // parameter or a type built out of one, which a definition is not; the namespace
    // filter above has already dropped every type that carries no namespace.
    private static string FullNameOf(Type type) =>
        type.FullName ?? throw new InvalidOperationException(
            $"{type.Name} has no full name, and a type definition reached from Assembly.GetTypes() always does.");

    private static string NamespaceOf(Type type) =>
        type.Namespace ?? throw new InvalidOperationException(
            $"{type.FullName} has no namespace, and LeetCodeTypes() admits only types that do.");

    // IsRegistration admits a type only if it is non-abstract and assignable to
    // ILeetCodeProblemRegistration, and Activator constructs such a type or throws, so
    // the instance it returns is a registration. A null here would mean the filter
    // above started admitting something else.
    private static ILeetCodeProblemRegistration RegistrationOf(Type registration) =>
        Activator.CreateInstance(registration) as ILeetCodeProblemRegistration
        ?? throw new InvalidOperationException(
            $"{registration.FullName} was paired as a problem registration but is not constructible as one.");

    private static bool IsRegistration(Type type)
        => type is { IsAbstract: false, IsInterface: false } && type.IsAssignableTo(typeof(ILeetCodeProblemRegistration));

    // A static class: abstract and sealed at the same time, which no other shape is.
    private static bool IsSolutionClass(Type type)
        => type is { IsClass: true, IsAbstract: true, IsSealed: true, IsNested: false }
            && type.Name.EndsWith(SolutionSuffix, StringComparison.Ordinal);

    private static HashSet<string> StrategyNamesOf(Type solution)
        => solution
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .Select(method => StrategyNameOf(method.Name))
            .Where(name => name.Length > 0)
            .ToHashSet(StringComparer.Ordinal);

    // <Operation>By<Strategy>, split at the first `By` that starts a new word - so
    // 17.4's two hoisted overloads collapse onto the one strategy name they share,
    // and a helper with no `By` at all is not mistaken for an arm.
    private static string StrategyNameOf(string methodName)
    {
        for (var index = methodName.IndexOf(StrategyInfix, StringComparison.Ordinal);
            index > 0;
            index = methodName.IndexOf(StrategyInfix, index + 1, StringComparison.Ordinal))
        {
            var start = index + StrategyInfix.Length;

            if (start < methodName.Length && char.IsUpper(methodName[start]))
            {
                return methodName[start..];
            }
        }

        return string.Empty;
    }
}
