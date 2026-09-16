using DSAExperimentation.LeetCode.FaultyKeyboard;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FaultyKeyboard;

// Harness only: the algorithms live in FaultyKeyboardSolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed class FaultyKeyboardTests
{
    public static TheoryData<FaultyKeyboardCase> Examples =>
        new()
        {
            { new FaultyKeyboardCase(Keystrokes: "string", ScreenContents: "rtsng") },
            { new FaultyKeyboardCase(Keystrokes: "poiinter", ScreenContents: "ponter") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FinalStringByReversal_LeetCodeExamples_ReturnsFinalScreenContents(FaultyKeyboardCase example)
        => Assert.Equal(
            example.ScreenContents,
            FaultyKeyboardSolution.FinalStringByReversal(example.Keystrokes));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FinalStringByDeque_LeetCodeExamples_ReturnsFinalScreenContents(FaultyKeyboardCase example)
        => Assert.Equal(
            example.ScreenContents,
            FaultyKeyboardSolution.FinalStringByDeque(example.Keystrokes));

    // One LeetCode example: the keys pressed and what the screen actually shows after the
    // faulty keyboard has had them. The two values are named fields rather than two
    // adjacent `string` parameters, so a row is written `new FaultyKeyboardCase(...)` by
    // name and a keystrokes/screen swap has to be typed out instead of falling out of a
    // position the compiler would have accepted either way. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct FaultyKeyboardCase(string Keystrokes, string ScreenContents);
}
