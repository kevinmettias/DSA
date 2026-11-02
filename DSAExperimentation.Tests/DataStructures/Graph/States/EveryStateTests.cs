using DSAExperimentation.DataStructures.Graph.States;

namespace DSAExperimentation.Tests.DataStructures.Graph.States;

public sealed partial class EveryStateTests
{
    public static TheoryData<string> States => new() { "0000", string.Empty, "9999" };

    [Theory]
    [MemberData(nameof(States))]
    public void IsKept_EveryState(string state) => Assert.True(new EveryState<string>().IsKept(state));
}
