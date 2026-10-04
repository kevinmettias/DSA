using System.Reflection;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.LeetCode.Tests;

// suppressions.json waives check-test-coverage over DSAExperimentation.LeetCode/**
// on the strength of the claim that every strategy and every helper a solution class
// exposes is named by a test, and the two theories here are what make that claim
// checked rather than prose. Until the first existed, KthSmallestElementInABSTSolution had
// no caller at all while its test asserted a private copy of both walks, and the
// waiver hid it.
//
// - Section 17.3 says a solution class holds EVERY strategy for its problem, each
//   named <Operation>By<Strategy>, and section 17.7 asks for one test method per
//   strategy, named for it - so a solution cannot grow an arm that nothing asserts,
//   the "the naive baseline was never tested" gap the five-tier reorg set out to
//   close. The first theory requires that of every strategy-named static method and
//   every strategy-named nested type; a design problem's strategies are whole
//   classes, while its interfaces and value wrappers carry no behavior to name.
// - Section 17.4 makes a strategy's input preparation - the graph, the distance
//   matrix, the trie, the prefix sums - a public helper so a benchmark can charge it
//   to [GlobalSetup]. That helper is behavior too, and a test that only feeds it to
//   a strategy checks the final answer, not what was built. The second theory
//   requires a test named for every non-private static method the first leaves out,
//   so between them every such method is named exactly once.
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

    // A member is named by a test method whose name is the member's name or starts
    // with it as the first underscore-separated word - the Member_Scenario_Expectation
    // shape check-test-coverage also reads - in the test namespace that mirrors the
    // solution's own problem folder.
    [Theory]
    [MemberData(nameof(SolutionClasses))]
    public void EveryStrategy_IsNamedByATestMethodInItsProblemFolder(string solutionTypeName)
    {
        var solution = SolutionNamed(solutionTypeName);
        var strategies = StrategyMembersOf(solution);
        var unnamed = UnnamedAmong(strategies, solution);

        Assert.NotEmpty(strategies);
        Assert.True(
            unnamed.Count == 0,
            $"{solution.Name}: no test method in {TestNamespaceFor(solution)} is named for [{Listed(unnamed)}]. "
            + "Section 17.7 asks for one test method per strategy, starting with the strategy's own name.");
    }

    // A solution class need not have a helper at all, so unlike the theory above this
    // one asserts nothing about how many it found.
    [Theory]
    [MemberData(nameof(SolutionClasses))]
    public void EveryInputPreparationHelper_IsNamedByATestMethodInItsProblemFolder(string solutionTypeName)
    {
        var solution = SolutionNamed(solutionTypeName);
        var unnamed = UnnamedAmong(HelperMethodsOf(solution), solution);

        Assert.True(
            unnamed.Count == 0,
            $"{solution.Name}: no test method in {TestNamespaceFor(solution)} is named for [{Listed(unnamed)}]. "
            + "Section 17.4's input-preparation helpers are public so a benchmark can hoist them, "
            + "and each needs a test named for it that asserts what it builds.");
    }

    // Both theories above have one row per solution class, so a discovery that found
    // none would report them green while checking nothing.
    [Fact]
    public void SolutionClasses_AfterDiscovery_AreFound() => Assert.NotEmpty(SolutionClasses);

    private static bool IsNamedBy(string strategy, string testMethod)
        => testMethod == strategy
            || (testMethod.StartsWith(strategy, StringComparison.Ordinal)
                && testMethod[strategy.Length] == TestNameSeparator);

    private static Type SolutionNamed(string solutionTypeName)
        => LeetCodeTypes().Single(type => FullNameOf(type) == solutionTypeName);

    private static List<string> UnnamedAmong(List<string> members, Type solution)
    {
        var testMethods = TestMethodNamesFor(solution);

        return members.Where(member => !testMethods.Any(test => IsNamedBy(member, test))).ToList();
    }

    // The non-private members a strategy name can sit on: a static method for a
    // function-shaped problem, a nested type for a design problem whose strategies are
    // whole classes.
    private static List<string> StrategyMembersOf(Type solution)
    {
        var nestedTypes = solution
            .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
            .Where(type => type.IsNestedPublic || type.IsNestedAssembly)
            .Select(type => type.Name)
            .Where(IsAuthored);

        return Sorted(NonPrivateStaticMethodNamesOf(solution).Concat(nestedTypes).Where(IsStrategyNamed));
    }

    // Every non-private static method the strategy rule above does not reach. Nested
    // types are left out on purpose: one that is not strategy-named is an interface or
    // a value wrapper, with no behavior of its own for a test to name.
    private static List<string> HelperMethodsOf(Type solution)
        => Sorted(NonPrivateStaticMethodNamesOf(solution).Where(name => !IsStrategyNamed(name)));

    // Property accessors and operators are special names, not methods an author wrote
    // to be called by name.
    private static IEnumerable<string> NonPrivateStaticMethodNamesOf(Type solution)
        => solution
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => (method.IsPublic || method.IsAssembly) && !method.IsSpecialName)
            .Select(method => method.Name)
            .Where(IsAuthored);

    // Compiler-generated members carry '<' in their names and are not the author's - a
    // local function, for one, is emitted as an internal static method of its class.
    private static bool IsAuthored(string memberName) => !memberName.Contains('<');

    private static bool IsStrategyNamed(string memberName) => StrategyNameOf(memberName).Length > 0;

    private static List<string> Sorted(IEnumerable<string> names)
        => names.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToList();

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

    private static string Listed(IEnumerable<string> names)
        => string.Join(", ", names.Order(StringComparer.Ordinal));

    private static IEnumerable<Type> LeetCodeTypes()
        => typeof(LeetCodeWireFormat).Assembly.GetTypes().Where(type => type.Namespace is not null);

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

    // A static class: abstract and sealed at the same time, which no other shape is.
    private static bool IsSolutionClass(Type type)
        => type is { IsClass: true, IsAbstract: true, IsSealed: true, IsNested: false }
            && type.Name.EndsWith(SolutionSuffix, StringComparison.Ordinal);

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
