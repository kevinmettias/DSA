export const meta = {
  name: 'leetcode-coverage',
  description: 'Batch-processes .claude/leetcode-coverage/manifest.json: composes existing DSA primitives into a LeetCode/ solution class plus test and benchmark harnesses for the next pending problems, verifies, records progress',
  phases: [
    { title: 'Select', detail: 'read manifest, pick next batch of pending problems' },
    { title: 'Implement', detail: 'one agent per cluster: solution class + test harness + benchmark harness' },
    { title: 'Verify', detail: 'build src, then build + run the touched test/benchmark projects' },
    { title: 'Record', detail: 'update manifest with results from this batch' },
  ],
}

const MANIFEST_PATH = '.claude/leetcode-coverage/manifest.json'
const UPDATE_SCRATCH_PATH = '.claude/leetcode-coverage/_pending-update.json'
const DEFAULT_BATCH_SIZE = 12
const DEFAULT_CLUSTER_SIZE = 3
const MANIFEST_INDENT_SPACES = 2

const batchSize = (args && args.batchSize) || DEFAULT_BATCH_SIZE
const clusterSize = (args && args.clusterSize) || DEFAULT_CLUSTER_SIZE

const SELECT_SCHEMA = {
  type: 'object',
  properties: {
    batch: {
      type: 'array',
      items: {
        type: 'object',
        properties: {
          id: { type: 'integer' },
          title: { type: 'string' },
          difficulty: { type: 'string' },
        },
        required: ['id', 'title', 'difficulty'],
      },
    },
    totalPendingBeforeThisBatch: { type: 'integer' },
  },
  required: ['batch', 'totalPendingBeforeThisBatch'],
}

const IMPLEMENT_SCHEMA = {
  type: 'object',
  properties: {
    results: {
      type: 'array',
      items: {
        type: 'object',
        properties: {
          id: { type: 'integer' },
          status: { type: 'string', enum: ['done', 'blocked'] },
          solutionPath: { type: ['string', 'null'] },
          testPath: { type: ['string', 'null'] },
          benchmarkPath: { type: ['string', 'null'] },
          primitivesUsed: { type: ['string', 'null'] },
          notes: { type: ['string', 'null'] },
        },
        required: ['id', 'status'],
      },
    },
  },
  required: ['results'],
}

const VERIFY_SCHEMA = {
  type: 'object',
  properties: {
    buildSucceeded: { type: 'boolean' },
    testsPassed: { type: 'boolean' },
    benchmarksCompiled: { type: 'boolean' },
    failureSummary: { type: ['string', 'null'] },
  },
  required: ['buildSucceeded', 'testsPassed', 'benchmarksCompiled'],
}

function buildRoleAndGoal(list) {
  return (
    `You are extending the DSA repo at the current working directory (F:\\repos\\DSA) - a generic C# ` +
    `algorithms/data-structures FRAMEWORK with a LeetCode solutions tier on top of it. READ ARCHITECTURE.md ` +
    `SECTION 17 FIRST - it defines the five tiers you must file code into, and it overrides any older ` +
    `convention you infer from an unmigrated test or benchmark you happen to open. Your job: for each of the ` +
    `following LeetCode problems, prove this repo's EXISTING primitives are sufficient to solve it, by ` +
    `writing (a) a solution class, (b) a test harness around it and (c) a BenchmarkDotNet harness around it. ` +
    `Do NOT invent new production primitives in DataStructures/ or Algorithms/ - if a problem genuinely ` +
    `cannot be solved by composing what already exists, mark it "blocked" with a note naming the missing ` +
    `primitive, do not build it ad hoc.\n\n` +
    `Problems in this cluster:\n${list}\n\n`
  )
}

function buildConventions() {
  return (
    `WHERE EACH FILE GOES (ARCHITECTURE.md section 17 is the authority; these are its rules in brief):\n` +
    `1. DSAExperimentation.LeetCode/<Name>/<Name>Solution.cs (its OWN project, referencing DSAExperimentation) - an \`internal static class ` +
    `<Name>Solution\` in namespace \`DSAExperimentation.LeetCode.<Name>\`, holding EVERY strategy for ` +
    `the problem as a public static method named \`<Operation>By<Strategy>\` (e.g. AddByBitStack, ` +
    `TryFindIndicesByBruteForce, MinTurnsByReduceGraph). The naive/brute-force baseline is a first-class ` +
    `method here too, NOT a private helper hidden in the benchmark - that is what gets it under test. If a ` +
    `benchmark needs to hoist input construction into [GlobalSetup], give the strategy a second overload ` +
    `taking the prepared input; that overload must take a Domain type or one of this repo's own containers ` +
    `(e.g. Set<string>, which is not IEnumerable, so the overloads can never be ambiguous), never a BCL ` +
    `collection the LeetCode-shaped overload could also bind. A witness type (IFoldAlgebra, ITopology, ...) ` +
    `used by this problem ALONE also lives in this folder.\n` +
    `2. Shared building blocks. WHICH TIER a shared type goes in is decided by ARCHITECTURE.md section 2's ` +
    `axes, NOT by how many problems use it - "used by more than one problem" is a sharing test, not a ` +
    `classification test, and getting that wrong is exactly the mistake section 17.6 documents. A ` +
    `Representation or Topology witness (node types, ITopology/IChildren witnesses, graph builders) goes in ` +
    `DSAExperimentation/DataStructures/; an Operations or strategy witness (heuristics, algebras, search ` +
    `orders) goes in DSAExperimentation/Algorithms/; DSAExperimentation/Domain/ is ONLY for things that fix ` +
    `CONTENT - a specific modulus, a specific vertex set, one problem family's semantics.\n` +
    `   BROWSE THESE FIRST, they very often already cover the problem you are given: ` +
    `DataStructures/Graph/Hamming (one-character-mutation graphs over any Alphabet, plus HammingSearch), ` +
    `DataStructures/Graph/Grids (Grid and WeightedGrid), ` +
    `DataStructures/Graph/Engines/Dags/Trees (BinaryTree, tries, RootedTreeNode + ParentArrayTree), ` +
    `Algorithms/ShortestPaths (Dijkstra/AStar plus the Zero/Manhattan/Chebyshev heuristics, and ` +
    `Grids//Hamming/ distance helpers), Algorithms/Reducing, Algorithms/Folding, ` +
    `Domain/Locks (the 4-wheel lock instance), Domain/Modular (mod 1e9+7).\n` +
    `3. DSAExperimentation.Tests/LeetCodeCoverage/<Name>/<Name>Tests.cs - HARNESS ONLY, no algorithm ` +
    `whatsoever. A \`public sealed class <Name>Tests\` with LeetCode's published examples stated ONCE as ` +
    `\`public static TheoryData<...> Examples\`, then ONE \`[Theory] [MemberData(nameof(Examples))]\` ` +
    `method PER STRATEGY, so a failure names the strategy that broke. No Fixtures/ subfolder - if you were ` +
    `about to create one, that type belongs in Domain/ or the LeetCode/ problem folder instead.\n` +
    `4. DSAExperimentation.Benchmarks/ProblemSolutions/<Name>Benchmarks.cs - HARNESS ONLY. [Benchmark] ` +
    `methods that are one-line calls into <Name>Solution, one per strategy, with [Params] input sizes and a ` +
    `[GlobalSetup] that builds the workload. Only workload SIZING/seeding may live in ` +
    `DSAExperimentation.Benchmarks/Fixtures/ (see LockWorkloads, HammingWorkloads, WeightedGridWorkloads); ` +
    `what it builds FROM is Domain code.\n\n` +
    `Read these as the reference shape before writing anything - they are migrated and correct:\n` +
    `- DSAExperimentation.LeetCode/OpenTheLock/OpenTheLockSolution.cs (two strategies, hoisted overloads)\n` +
    `- DSAExperimentation.Tests/LeetCodeCoverage/OpenTheLock/OpenTheLockTests.cs\n` +
    `- DSAExperimentation.Benchmarks/ProblemSolutions/OpenTheLockBenchmarks.cs\n` +
    `Most existing tests/benchmarks are NOT yet migrated and still carry their algorithm inline in both ` +
    `files - do not copy that shape, and do not treat it as evidence about the convention.\n\n`
  )
}

function buildNamingGuidance() {
  return (
    `Naming: PascalCase derived from the problem title (e.g. "House Robber II" -> HouseRobberII, keep roman ` +
    `numerals as-is). If the title starts with a digit (e.g. "3Sum", "132 Pattern"), spell out a natural C# ` +
    `identifier instead (ThreeSum, OneThreeTwoPattern) - use judgment, it just needs to be a valid, readable C# ` +
    `identifier. Check DSAExperimentation.LeetCode/ and DSAExperimentation.Tests/LeetCodeCoverage/ first for a ` +
    `folder that already covers this exact problem BY CONTENT (not just by name) and report it as already done ` +
    `if so, without duplicating it.\n\n` +
    `TESTING POLICY (ARCHITECTURE.md section 18): every data structure and algorithm carries its OWN direct ` +
    `unit tests at the mirrored path (DataStructures/Heap/Heap.cs -> Tests/DataStructures/Heap/HeapTests.cs), ` +
    `with test method names beginning with the member under test. A LeetCode coverage entry is a layer ON TOP ` +
    `of that and never a substitute: if you find yourself relying on a coverage test to exercise a primitive, ` +
    `the primitive is untested. You are not adding primitives in this workflow (mark such problems blocked), ` +
    `so in practice this means: do NOT count your new coverage test as testing anything in DataStructures/ or ` +
    `Algorithms/.

` +
    `Keep new code simple and idiomatic for this codebase (short, no unnecessary abstraction) - closely match ` +
    `the three reference files' style and structure; you do not need to run the full Nomos gate tool per problem ` +
    `at this scale, a separate periodic pass will handle gate compliance later.\n\n`
  )
}

function buildGitSafetyNotice() {
  return (
    `GIT SAFETY: this repo commonly has other uncommitted work in progress from concurrent sessions/agents. ` +
    `Never run a git command that discards changes (git checkout -- <path>, git restore, git reset --hard, git ` +
    `clean) on ANY file, manifest.json included - doing so silently destroys someone else's uncommitted work ` +
    `with no error. If manifest.json or any other file looks unexpected mid-task, that is normal (another agent ` +
    `is editing concurrently) - just re-read the current file and proceed; only git status/diff/log are safe to ` +
    `run.\n\n`
  )
}

function buildOutputContract() {
  return (
    `After writing all files for this cluster, return one result object per problem id: status "done" (with the ` +
    `real solutionPath/testPath/benchmarkPath - as REPO-ROOT-RELATIVE paths using forward slashes, e.g. ` +
    `"DSAExperimentation.LeetCode/TwoSum/TwoSumSolution.cs", matching the existing manifest entries' ` +
    `style, NOT an absolute Windows path - and a short primitivesUsed note naming any Domain/ types you reused ` +
    `or added) or "blocked" (with a note on exactly what's missing and why it's a genuine primitive gap, not ` +
    `just effort).`
  )
}

function buildImplementPrompt(cluster) {
  const list = cluster.map((p) => `- #${p.id} "${p.title}" (${p.difficulty})`).join('\n')
  return (
    buildRoleAndGoal(list) +
    buildConventions() +
    buildNamingGuidance() +
    buildGitSafetyNotice() +
    buildOutputContract()
  )
}

phase('Select')
const selection = await agent(
  `In the repo at the current working directory, read the JSON file ${MANIFEST_PATH} using a Bash one-liner ` +
  `with jq (do NOT try to read and reason over the whole 300+KB file by hand) - the file is a JSON object with ` +
  `a top-level "problems" array, each item having id/title/difficulty/status. Run:\n` +
  `  jq '[.problems[] | select(.status=="pending")] | length' ${MANIFEST_PATH}\n` +
  `to get the total pending count, then:\n` +
  `  jq --argjson n ${batchSize} '[.problems[] | select(.status=="pending")] | sort_by(.id) | .[0:$n] | map({id,title,difficulty})' ${MANIFEST_PATH}\n` +
  `to get the next batch. Report the batch array and the total pending count (from the first command, BEFORE ` +
  `removing this batch). Do not modify the file.`,
  { phase: 'Select', schema: SELECT_SCHEMA },
)

log(
  `Selected ${selection.batch.length} problems (${selection.totalPendingBeforeThisBatch} pending total before this batch).`,
)

if (selection.batch.length === 0) {
  return { done: true, message: 'No pending problems remain in the manifest.' }
}

const clusters = []
for (let i = 0; i < selection.batch.length; i += clusterSize) {
  clusters.push(selection.batch.slice(i, i + clusterSize))
}

phase('Implement')
const implementResults = await pipeline(clusters, (cluster) =>
  agent(buildImplementPrompt(cluster), {
    phase: 'Implement',
    schema: IMPLEMENT_SCHEMA,
    label: `impl:${cluster[0].id}-${cluster[cluster.length - 1].id}`,
  }),
)

const allResults = implementResults.filter(Boolean).flatMap((r) => r.results)
log(
  `Implemented ${allResults.filter((r) => r.status === 'done').length}/${selection.batch.length}, ` +
    `blocked ${allResults.filter((r) => r.status === 'blocked').length}.`,
)

phase('Verify')
const verifyReport = await agent(
  `In the repo at the current working directory (no top-level .sln - build/test per-project). Run, from the ` +
  `repo root:\n` +
  `  dotnet build DSAExperimentation/DSAExperimentation.csproj -c Release\n` +
  `  dotnet build DSAExperimentation.LeetCode/DSAExperimentation.LeetCode.csproj -c Release\n` +
  `  dotnet build DSAExperimentation.Tests/DSAExperimentation.Tests.csproj -c Release\n` +
  `  dotnet test DSAExperimentation.Tests/DSAExperimentation.Tests.csproj --filter FullyQualifiedName~LeetCodeCoverage -c Release --no-build\n` +
  `  dotnet build DSAExperimentation.Benchmarks/DSAExperimentation.Benchmarks.csproj -c Release\n` +
  `(the last one just confirms the new benchmark classes compile - do NOT actually run BenchmarkDotNet jobs, ` +
  `that's too slow for this pass). Report whether the test-project build succeeded, whether the ` +
  `LeetCodeCoverage-filtered tests passed, whether the benchmarks project built, and if anything failed, a ` +
  `concise summary of the first few real failures (file/line/message) - not the full raw log.`,
  { phase: 'Verify', schema: VERIFY_SCHEMA },
)

log(
  `Verify: build=${verifyReport.buildSucceeded} tests=${verifyReport.testsPassed} ` +
    `benchmarksCompiled=${verifyReport.benchmarksCompiled}`,
)

const batchIsClean = verifyReport.buildSucceeded && verifyReport.testsPassed && verifyReport.benchmarksCompiled
const finalResults = batchIsClean
  ? allResults
  : allResults.map((r) => ({
      ...r,
      status: 'blocked',
      notes:
        `Batch build/test failed, reverted to blocked for retry: ${verifyReport.failureSummary || 'see verify phase log'}. ` +
        `Original note: ${r.notes || '(none)'}`,
    }))

phase('Record')
const recordSummary = await agent(
  `In the repo at the current working directory. Write EXACTLY this JSON array, verbatim, to a new file at ` +
  `${UPDATE_SCRATCH_PATH} using your file-write tool:\n\n${JSON.stringify(finalResults, null, MANIFEST_INDENT_SPACES)}\n\n` +
  `Then run this Python one-liner via Bash to apply it to ${MANIFEST_PATH} (adjust only if python3 is not on ` +
  `PATH - node is also available):\n\n` +
  `python3 -c "` +
  `import json; ` +
  `m=json.load(open('${MANIFEST_PATH}', encoding='utf-8')); ` +
  `u={x['id']: x for x in json.load(open('${UPDATE_SCRATCH_PATH}', encoding='utf-8'))}; ` +
  `[p.update({k:v for k,v in u[p['id']].items() if k!='id'}) for p in m['problems'] if p['id'] in u]; ` +
  `json.dump(m, open('${MANIFEST_PATH}','w',encoding='utf-8'), indent=2, ensure_ascii=False); ` +
  `print('updated', len(u), 'entries')` +
  `"\n\n` +
  `Then delete ${UPDATE_SCRATCH_PATH}. Return the "updated N entries" line.`,
  { phase: 'Record' },
)

log(recordSummary)

return {
  batchRequested: selection.batch.length,
  implemented: finalResults.filter((r) => r.status === 'done').length,
  blocked: finalResults.filter((r) => r.status === 'blocked').length,
  totalPendingBeforeThisBatch: selection.totalPendingBeforeThisBatch,
  verify: verifyReport,
  recordSummary,
}
