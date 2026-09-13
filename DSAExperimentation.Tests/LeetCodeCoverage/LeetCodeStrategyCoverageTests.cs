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
public sealed class LeetCodeStrategyCoverageTests
{
    private const string StrategyInfix = "By";
    private const string SolutionSuffix = "Solution";

    public static TheoryData<string> RegisteredSolutions
    {
        get
        {
            var data = new TheoryData<string>();

            foreach (var (solution, _) in Pairs())
            {
                data.Add(solution.FullName!);
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(RegisteredSolutions))]
    public void Registration_NamesEveryStrategyItsSolutionExposes(string solutionTypeName)
    {
        var (solution, registration) = Pairs().Single(pair => pair.Solution.FullName == solutionTypeName);
        var problem = ((ILeetCodeProblemRegistration)Activator.CreateInstance(registration)!).Describe();
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
            .ToDictionary(type => type.Namespace!, type => type, StringComparer.Ordinal);

        return LeetCodeTypes()
            .Where(IsSolutionClass)
            .Where(type => registrations.ContainsKey(type.Namespace!))
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .Select(type => (type, registrations[type.Namespace!]));
    }

    private static IEnumerable<Type> LeetCodeTypes()
        => typeof(LeetCodeProblem).Assembly.GetTypes().Where(type => type.Namespace is not null);

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
