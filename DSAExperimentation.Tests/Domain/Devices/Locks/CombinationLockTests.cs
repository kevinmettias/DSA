using DSAExperimentation.Domain.Devices.Locks;

namespace DSAExperimentation.Tests.Domain.Devices.Locks;

// The general lock, read through two builds: the common four-digit lock, and a three-wheel lock of
// the letters a to e, which no LeetCode problem uses, so nothing here leans on decimal digits.
public sealed partial class CombinationLockTests
{
    public static TheoryData<int, string> FourDigitCombinations => new() { { 0, "0000" }, { 42, "0042" }, { 9999, "9999" } };

    public static TheoryData<int, string> LetterCombinations => new() { { 0, "aaa" }, { 7, "abc" }, { 124, "eee" } };

    [Fact]
    public void CombinationCount_IsSymbolsToThePowerOfWheels()
    {
        Assert.Equal(10_000, CombinationLock<FourDigitLock>.CombinationCount);
        Assert.Equal(125, CombinationLock<LetterLock>.CombinationCount);
    }

    [Fact]
    public void OneTurnAwayCount_IsTwoPerWheel() => Assert.Equal(6, CombinationLock<LetterLock>.OneTurnAwayCount);

    [Theory]
    [MemberData(nameof(FourDigitCombinations))]
    public void Combination_ReadsTheNumberWheelByWheel_FirstWheelFirst(int index, string expected)
        => Assert.Equal(expected, CombinationLock<FourDigitLock>.Combination(index));

    [Theory]
    [MemberData(nameof(LetterCombinations))]
    public void Combination_SpellsEachWheelFromItsFirstSymbol(int index, string expected)
        => Assert.Equal(expected, CombinationLock<LetterLock>.Combination(index));

    [Fact]
    public void Combination_NamesEveryCombinationOnce()
        => Assert.Equal(
            CombinationLock<LetterLock>.CombinationCount,
            Enumerable.Range(0, CombinationLock<LetterLock>.CombinationCount).Select(CombinationLock<LetterLock>.Combination).Distinct().Count());

    [Fact]
    public void ClickUp_StepsToTheNextSymbol() => Assert.Equal('c', CombinationLock<LetterLock>.ClickUp('b'));

    [Fact]
    public void ClickUp_WrapsTheLastSymbolToTheFirst() => Assert.Equal('a', CombinationLock<LetterLock>.ClickUp('e'));

    [Fact]
    public void ClickDown_StepsToThePreviousSymbol() => Assert.Equal('a', CombinationLock<LetterLock>.ClickDown('b'));

    [Fact]
    public void ClickDown_WrapsTheFirstSymbolToTheLast() => Assert.Equal('9', CombinationLock<FourDigitLock>.ClickDown('0'));

    // Each wheel up then down, first wheel first: the order LC 752's two arms both search in.
    [Fact]
    public void WriteOneTurnAway_TurnsEachWheelUpThenDown()
    {
        var into = new string[CombinationLock<FourDigitLock>.OneTurnAwayCount];

        var written = CombinationLock<FourDigitLock>.WriteOneTurnAway("0000", into);

        Assert.Equal(8, written);
        Assert.Equal(["1000", "9000", "0100", "0900", "0010", "0090", "0001", "0009"], into);
    }

    [Fact]
    public void WriteOneTurnAway_LeavesTheCombinationTurnedFromAsItWas()
    {
        var combination = CombinationLock<LetterLock>.Combination(7);
        var into = new string[CombinationLock<LetterLock>.OneTurnAwayCount];

        CombinationLock<LetterLock>.WriteOneTurnAway(combination, into);

        Assert.Equal("abc", combination);
        Assert.DoesNotContain(combination, into);
    }

    // Three wheels of the letters a to e.
    private readonly struct LetterLock : ILockSpec
    {
        public static int WheelCount => 3;

        public static int SymbolsPerWheel => 5;

        public static char FirstSymbol => 'a';
    }
}
