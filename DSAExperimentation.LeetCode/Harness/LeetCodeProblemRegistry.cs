using System.Reflection;

namespace DSAExperimentation.LeetCode.Harness;

// Every registered problem, discovered once per process by scanning this assembly
// for ILeetCodeProblemRegistration. Both harnesses enumerate this: the test
// harness over CaseArms, the benchmark harness over WorkloadArms.
//
// Discovery runs a registration's Describe() eagerly, at scan time, precisely so
// that a malformed registration (no cases, no stated equality, a duplicate slug)
// fails loudly the first time anything touches the registry rather than
// contributing zero silent theory rows.
internal static class LeetCodeProblemRegistry
{
    private static readonly Lazy<IReadOnlyDictionary<string, LeetCodeProblem>> BySlug = new(Discover);

    public static IReadOnlyList<LeetCodeProblem> All => BySlug.Value.Values.ToList();

    public static LeetCodeProblem Get(string titleSlug)
        => BySlug.Value.TryGetValue(titleSlug, out var problem)
            ? problem
            : UnknownProblem(titleSlug);

    // A slug that is not registered is a caller bug, so it fails loudly instead of
    // answering null.
    private static LeetCodeProblem UnknownProblem(string titleSlug)
        => throw new ArgumentException($"No registered LeetCode problem with slug '{titleSlug}'.", nameof(titleSlug));

    // One row per (problem, strategy, case) - the unit the single test harness
    // asserts, so a failure names all three.
    public static IEnumerable<LeetCodeArm> CaseArms()
        => All.SelectMany(
            problem => problem.StrategyNames.SelectMany(
                strategy => problem.CaseNames.Select(
                    example => new LeetCodeArm(problem.TitleSlug, strategy, example))));

    // The units the single benchmark harness measures. Each problem decides its
    // own pairs rather than the registry taking a cross product - see
    // LeetCodeProblem.WorkloadArms. Empty for a problem that registered no
    // workloads, which is the normal state for one whose strategies are not worth
    // comparing.
    public static IEnumerable<LeetCodeArm> WorkloadArms()
        => All.SelectMany(problem => problem.WorkloadArms);

    private static IReadOnlyDictionary<string, LeetCodeProblem> Discover()
    {
        var problems = new Dictionary<string, LeetCodeProblem>(StringComparer.Ordinal);

        foreach (var problem in DescribeAll())
        {
            if (!problems.TryAdd(problem.TitleSlug, problem))
            {
                throw new InvalidOperationException(
                    $"Two registrations both claim the slug '{problem.TitleSlug}'.");
            }
        }

        return problems;
    }

    private static IEnumerable<LeetCodeProblem> DescribeAll()
        => typeof(LeetCodeProblemRegistry).Assembly
            .GetTypes()
            .Where(IsRegistration)
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .Select(type => (ILeetCodeProblemRegistration)Activator.CreateInstance(type)!)
            .Select(registration => registration.Describe());

    private static bool IsRegistration(Type type)
        => type is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false }
            && type.IsAssignableTo(typeof(ILeetCodeProblemRegistration))
            && type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes) is not null;
}
