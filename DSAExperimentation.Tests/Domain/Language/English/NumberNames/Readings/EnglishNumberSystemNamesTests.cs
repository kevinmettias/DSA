using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Language.English.NumberNames.Readings;
using DSAExperimentation.Domain.Mathematics.NumberSystems;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Readings;

public sealed partial class EnglishNumberSystemNamesTests
{
    public static TheoryData<NumberSystemName, NumberSystem, string> Names => new()
    {
        { EnglishNumberSystemNames.Naturals, NumberSystem.Naturals, "natural numbers" },
        { EnglishNumberSystemNames.Integers, NumberSystem.Integers, "integers" },
        { EnglishNumberSystemNames.Rationals, NumberSystem.Rationals, "rational numbers" },
        { EnglishNumberSystemNames.Reals, NumberSystem.Reals, "real numbers" },
        { EnglishNumberSystemNames.Complex, NumberSystem.Complex, "complex numbers" },
        { EnglishNumberSystemNames.Quaternions, NumberSystem.Quaternions, "quaternions" },
        { EnglishNumberSystemNames.Octonions, NumberSystem.Octonions, "octonions" },
        { EnglishNumberSystemNames.Sedenions, NumberSystem.Sedenions, "sedenions" },
    };

    [Theory]
    [MemberData(nameof(Names))]
    public void Name_IsItsSystemAndItsName(NumberSystemName name, NumberSystem system, string spelled) =>
        Assert.Equal(new NumberSystemName(system, spelled), name);

    // Every system the hierarchy and the doublings list has a name, each once, in that order.
    [Fact]
    public void All_NamesTheHierarchyThenTheDoublings() =>
        Assert.Equal(
            NumberSystem.Hierarchy.Concat(NumberSystem.CayleyDickson).Distinct(),
            EnglishNumberSystemNames.All.Select(name => name.System));
}
