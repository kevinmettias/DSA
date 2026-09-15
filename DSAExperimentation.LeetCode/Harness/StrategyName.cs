namespace DSAExperimentation.LeetCode.Harness;

// Which of a problem's registered strategies a harness wants run - the arm's
// middle token, once the title/strategy/entry string both harnesses carry has been
// split back apart. Its own type rather than a bare string so
// RunCase(strategyName, caseName) cannot be handed the two the wrong way round.
internal readonly record struct StrategyName(string Text);
