using DSAExperimentation.LeetCode.Solutions.NumberOfStudentsUnableToEatLunch;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfStudentsUnableToEatLunch;

public sealed partial class NumberOfStudentsUnableToEatLunchPromiseTests
{
    public static TheoryData<int[], int[], bool> Inputs =>
        new()
        {
            { [1, 1, 0, 0], [0, 1, 0, 1], true },
            { [1, 1], [0, 1, 0], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenThereIsASandwichForEveryStudent(int[] students, int[] sandwiches, bool expected)
        => Assert.Equal(expected, NumberOfStudentsUnableToEatLunchPromise.IsKeptBy(students, sandwiches));
}
