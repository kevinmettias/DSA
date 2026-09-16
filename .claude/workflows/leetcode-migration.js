export const meta = {
  name: 'leetcode-migration',
  description:
    'Migrates already-covered LeetCode problems from the pre-section-17 shape (algorithm inlined separately in the test and the benchmark) into the tiered architecture: one solution class in DSAExperimentation.LeetCode holding every strategy, with the test and benchmark reduced to harnesses',
  phases: [
    { title: 'Select', detail: 'read manifest, pick the next batch still in the pre-section-17 shape' },
    { title: 'Migrate', detail: 'one agent per cluster: extract solution class, reduce both harnesses, relocate fixtures' },
    { title: 'Verify', detail: 'build all four projects, run the full test suite including LayeringTests' },
    { title: 'Record', detail: 'tag migrated problems section-17 in the manifest' },
  ],
}

const MANIFEST_PATH = '.claude/leetcode-coverage/manifest.json'
const UPDATE_SCRATCH_PATH = '.claude/leetcode-coverage/_pending-migration.json'
const RECORD_SCRIPT_PATH = '.claude/leetcode-coverage/_apply-migration.py'
const DEFAULT_BATCH_SIZE = 10
const DEFAULT_CLUSTER_SIZE = 2
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
          name: { type: 'string' },
          testPath: { type: ['string', 'null'] },
          benchmarkPath: { type: ['string', 'null'] },
        },
        required: ['id', 'title', 'name'],
      },
    },
    remainingBeforeThisBatch: { type: 'integer' },
  },
  required: ['batch', 'remainingBeforeThisBatch'],
}

const MIGRATE_SCHEMA = {
  type: 'object',
  properties: {
    results: {
      type: 'array',
      items: {
        type: 'object',
        properties: {
          id: { type: 'integer' },
          status: { type: 'string', enum: ['migrated', 'already', 'blocked'] },
          solutionPath: { type: ['string', 'null'] },
          testPath: { type: ['string', 'null'] },
          benchmarkPath: { type: ['string', 'null'] },
          strategies: { type: ['string', 'null'] },
          relocatedTypes: { type: ['string', 'null'] },
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
    layeringHeld: { type: 'boolean' },
    testCount: { type: ['integer', 'null'] },
    failureSummary: { type: ['string', 'null'] },
  },
  required: ['buildSucceeded', 'testsPassed', 'layeringHeld'],
}

function buildRoleAndGoal(list) {
  return (
    `You are migrating the DSA repo at the current working directory (F:\\repos\\DSA) onto the tiered ` +
    `architecture. READ ARCHITECTURE.md SECTIONS 17 AND 18 FIRST - they are the authority and they ` +
    `override any older shape you infer from a file you happen to open. Most of the ~870 remaining ` +
    `LeetCode entries are still in the PRE-section-17 shape and are NOT evidence about the convention.\n\n` +
    `THE DEFECT YOU ARE FIXING, in the problems below: the algorithm is written TWICE - once inlined in ` +
    `the test as a private helper, once again inlined in the benchmark - so the two drift (LC 1's test ` +
    `solved for indices while its benchmark returned a bool), and the benchmark's ` +
    `[Benchmark(Baseline = true)] arm is never asserted by anything.\n\n` +
    `Problems in this cluster:\n${list}\n\n`
  )
}

function buildProcedure() {
  return (
    buildProcedureIntake() +
    buildProcedureAuthoring() +
    buildProcedureFixturePlacement() +
    buildProcedureHarnesses()
  )
}

// Steps 1-2: is this problem ours to migrate, and what do the existing test and
// benchmark actually measure?
function buildProcedureIntake() {
  return (
    `FOR EACH PROBLEM, in order:\n` +
    `1. If DSAExperimentation.LeetCode/<Name>/<Name>Solution.cs ALREADY EXISTS, this problem is already ` +
    `migrated (another session may have done it). Report status "already" and move on - do NOT rewrite it.\n` +
    `2. Read the existing test and benchmark named in the problem list. Inventory EVERY distinct strategy ` +
    `across BOTH files - the test's private solve helper, and each [Benchmark] method including the ` +
    `baseline. Two arms that answer the same question with different signatures (indices vs bool) are one ` +
    `strategy each; reconcile them onto LeetCode's actual answer shape.\n`
  )
}

// Steps 3-4: the solution class itself, and the overload that keeps input
// construction out of the measured method.
function buildProcedureAuthoring() {
  return (
    `3. Write DSAExperimentation.LeetCode/<Name>/<Name>Solution.cs - \`internal static class <Name>Solution\` ` +
    `in namespace \`DSAExperimentation.LeetCode.<Name>\`, one public static method per strategy named ` +
    `<Operation>By<Strategy> (AddByBitStack, TryFindIndicesByBruteForce, MinTurnsByReduceGraph). The naive ` +
    `baseline is a first-class method here, NOT a private helper left behind in the benchmark - putting it ` +
    `here is what finally gets it under test. Its INTERNALS stay BCL (a baseline represents "what you would ` +
    `write without this repo"); only the input container it is handed may be a repo type.\n` +
    `4. If the benchmark hoists input construction into [GlobalSetup], give the strategy a SECOND overload ` +
    `taking the prepared input, so construction is not charged to the measured method. That overload must ` +
    `take a domain object or a repo container that is NOT IEnumerable (e.g. Set<string>), never a BCL ` +
    `collection the LeetCode-shaped overload could also bind - otherwise the overloads are ambiguous.\n`
  )
}

// Step 5: which tier a carried fixture belongs in.
function buildProcedureFixturePlacement() {
  return (
    `5. Relocate any fixture types the pair carried (test Fixtures/ folder, or Benchmarks/Fixtures entries ` +
    `used only by this problem). WHICH TIER is decided by ARCHITECTURE.md section 2's axes, NOT by how many ` +
    `problems use the type:\n` +
    `   - a Representation or Topology witness (node types, ITopology/IChildren witnesses, graph builders) ` +
    `-> DSAExperimentation/DataStructures/**, beside its existing siblings;\n` +
    `   - an Operations/strategy witness (heuristics, search orders) -> DSAExperimentation/Algorithms/**;\n` +
    `   - something that fixes CONTENT (a specific modulus, a specific vertex set, one problem family's ` +
    `semantics) -> DSAExperimentation/Domain/**;\n` +
    `   - a witness used by THIS PROBLEM ALONE (a bespoke IFoldAlgebra, say) -> the LeetCode problem folder.\n` +
    `   BROWSE FOR AN EXISTING TYPE FIRST - DataStructures/Graph/Hamming, /Grids, /Engines/Dags/Trees, ` +
    `Algorithms/ShortestPaths, Domain/Locks, Domain/Modular already cover a lot, and duplicating one of ` +
    `them is the exact defect this migration exists to remove.\n`
  )
}

// Steps 6-8: the two harnesses, and the Section 18 obligation a new shared type
// brings with it.
function buildProcedureHarnesses() {
  return (
    `6. Rewrite the test as a HARNESS ONLY, no algorithm: \`public sealed class <Name>Tests\` with LeetCode's ` +
    `examples stated ONCE as \`public static TheoryData<...> Examples\`, then ONE ` +
    `\`[Theory] [MemberData(nameof(Examples))]\` method PER STRATEGY so a failure names the strategy that ` +
    `broke. KEEP every example the original test asserted and add cases if the original was thin. Delete the ` +
    `problem's Fixtures/ subfolder once its types have moved.\n` +
    `7. Rewrite the benchmark as a HARNESS ONLY: [Benchmark] methods that are one-line calls into ` +
    `<Name>Solution, one per strategy, preserving the original [Params] sizes and [GlobalSetup] workload. ` +
    `Only workload SIZING/seeding may stay in DSAExperimentation.Benchmarks/Fixtures.\n` +
    `8. SECTION 18: if step 5 put a NEW type into DataStructures/ or Algorithms/, it owes its own direct ` +
    `unit tests - <Name>Tests.cs at the mirrored path under DSAExperimentation.Tests, with method names ` +
    `beginning with the member under test (Count_..., GetChildren_...). A LeetCode harness does NOT ` +
    `discharge that. Write them in the same change.\n\n`
  )
}

function buildReferencesAndLimits() {
  return (
    `REFERENCE PAIRS - already migrated and correct, read one before writing anything:\n` +
    `- DSAExperimentation.LeetCode/OpenTheLock/OpenTheLockSolution.cs (two strategies, hoisted overloads, ` +
    `domain graph) + its Tests/LeetCodeCoverage/OpenTheLock + Benchmarks/ProblemSolutions pair;\n` +
    `- DSAExperimentation.LeetCode/AddBinary/AddBinarySolution.cs (simplest possible shape);\n` +
    `- DSAExperimentation.LeetCode/CountWaysToBuildRoomsInAnAntColony/ (problem-specific algebras kept ` +
    `beside the solution).\n\n` +
    `BEHAVIOUR MUST NOT CHANGE. The migrated strategies must return what the originals returned on the ` +
    `original inputs. Where the test arm and the benchmark arm disagreed, LeetCode's real answer wins, and ` +
    `you say so in notes. If a benchmark arm measured something weaker than the real answer (counting ` +
    `results instead of building them, say), it is fine to promote it to the real answer - note it.\n\n` +
    `DO NOT invent new DataStructures/ or Algorithms/ primitives to make a migration work. If a problem ` +
    `genuinely cannot be migrated without one, leave ALL its files untouched, report status "blocked" with ` +
    `the reason, and move on.\n\n` +
    `TIER ORDER IS ENFORCED: DataStructures may not reference Algorithms/Domain/LeetCode, Algorithms may not ` +
    `reference Domain/LeetCode, and so on. Tests/Architecture/LayeringTests.cs fails the build if you break ` +
    `it. There is exactly one allow-listed exception and you should not be adding another.\n\n`
  )
}

function buildSafetyNotice() {
  return (
    `CONCURRENCY + GIT SAFETY: other sessions edit this repo at the same time, and some of their files are ` +
    `UNTRACKED. Never run a git command that discards changes (git checkout -- <path>, git restore, git ` +
    `reset --hard, git clean) on ANY path, manifest.json included - it silently destroys their work with no ` +
    `error. Only git status/diff/log are safe. If a file looks unexpected mid-task, re-read it and proceed. ` +
    `If a problem's solution file appeared while you were working on it, treat it as "already" rather than ` +
    `overwriting. Use \`git mv\` only for tracked files; plain \`mv\` moves untracked ones that git mv ` +
    `refuses as "empty".\n\n`
  )
}

function buildOutputContract() {
  return (
    `Return one result object per problem id: status "migrated" (with solutionPath/testPath/benchmarkPath as ` +
    `REPO-ROOT-RELATIVE forward-slash paths, e.g. ` +
    `"DSAExperimentation.LeetCode/TwoSum/TwoSumSolution.cs"; \`strategies\` naming the methods you produced; ` +
    `\`relocatedTypes\` naming any fixture types you moved and where, or null), or "already" (nothing to do), ` +
    `or "blocked" (with the reason in notes).`
  )
}

function buildMigratePrompt(cluster) {
  const list = cluster
    .map((p) => `- #${p.id} "${p.title}" -> folder ${p.name}\n    test: ${p.testPath}\n    bench: ${p.benchmarkPath}`)
    .join('\n')

  return (
    buildRoleAndGoal(list) +
    buildProcedure() +
    buildReferencesAndLimits() +
    buildSafetyNotice() +
    buildOutputContract()
  )
}

phase('Select')
const selection = await agent(
  `In the repo at the current working directory, read ${MANIFEST_PATH} with a Bash one-liner (do NOT read ` +
  `the whole ~1MB file by hand). A problem still needs migrating when status == "done" AND ` +
  `architectureTier != "section-17". Run exactly:\n\n` +
  `python3 -c "` +
  `import json,io;` +
  `m=json.load(io.open('${MANIFEST_PATH}',encoding='utf-8'));` +
  `r=[p for p in m['problems'] if p.get('status')=='done' and p.get('architectureTier')!='section-17'];` +
  `r.sort(key=lambda p:p['id']);` +
  `print(len(r));` +
  `print(json.dumps([{'id':p['id'],'title':p['title'],'testPath':p.get('testPath'),'benchmarkPath':p.get('benchmarkPath')} for p in r[:${batchSize}]],indent=1))` +
  `"\n\n` +
  `The first line is the total remaining; the JSON that follows is the batch. For each batch entry derive ` +
  `\`name\`, the PascalCase problem folder, from testPath - it is the second-to-last path segment (e.g. ` +
  `"DSAExperimentation.Tests/LeetCodeCoverage/TwoSum/TwoSumTests.cs" -> "TwoSum"). If testPath is null, ` +
  `derive it from the title the way ARCHITECTURE.md and existing folders do. Report the batch and the total. ` +
  `Do not modify any file.`,
  { phase: 'Select', schema: SELECT_SCHEMA },
)

log(`Selected ${selection.batch.length} problem(s); ${selection.remainingBeforeThisBatch} still un-migrated.`)

if (selection.batch.length === 0) {
  return { done: true, message: 'Every covered problem is already on the section-17 architecture.' }
}

const clusters = []
for (let i = 0; i < selection.batch.length; i += clusterSize) {
  clusters.push(selection.batch.slice(i, i + clusterSize))
}

phase('Migrate')
const migrateResults = await pipeline(clusters, (cluster) =>
  agent(buildMigratePrompt(cluster), {
    phase: 'Migrate',
    schema: MIGRATE_SCHEMA,
    label: `migrate:${cluster[0].name}`,
  }),
)

const allResults = migrateResults.filter(Boolean).flatMap((r) => r.results)
log(
  `Migrated ${allResults.filter((r) => r.status === 'migrated').length}, ` +
    `already-done ${allResults.filter((r) => r.status === 'already').length}, ` +
    `blocked ${allResults.filter((r) => r.status === 'blocked').length}.`,
)

phase('Verify')
const verifyReport = await agent(
  `In the repo at the current working directory (no .sln - build per project). Run, from the repo root:\n` +
  `  dotnet build DSAExperimentation/DSAExperimentation.csproj -c Release\n` +
  `  dotnet build DSAExperimentation.LeetCode/DSAExperimentation.LeetCode.csproj -c Release\n` +
  `  dotnet build DSAExperimentation.Benchmarks/DSAExperimentation.Benchmarks.csproj -c Release\n` +
  `  dotnet test DSAExperimentation.Tests/DSAExperimentation.Tests.csproj -c Release\n` +
  `(the benchmarks build only confirms the harnesses compile - do NOT run BenchmarkDotNet jobs, far too ` +
  `slow for this pass). Run the FULL test suite, not a filtered subset: this migration moves types between ` +
  `tiers, so a regression can surface anywhere, and Tests/Architecture/LayeringTests.cs is what catches a ` +
  `tier inversion. Report whether every build succeeded, whether the tests passed, whether the LayeringTests ` +
  `specifically passed, the total passing test count, and a concise summary of the first few real failures ` +
  `if anything broke (file/line/message, not the raw log).`,
  { phase: 'Verify', schema: VERIFY_SCHEMA },
)

log(
  `Verify: build=${verifyReport.buildSucceeded} tests=${verifyReport.testsPassed} ` +
    `layering=${verifyReport.layeringHeld} total=${verifyReport.testCount ?? '?'}`,
)

const batchIsClean = verifyReport.buildSucceeded && verifyReport.testsPassed && verifyReport.layeringHeld

// Only a clean batch is recorded as migrated. A broken one keeps its old tag so
// the next run picks it up again, with the failure written into notes for a human.
const finalResults = batchIsClean
  ? allResults
  : allResults.map((r) => ({
      ...r,
      status: r.status === 'already' ? 'already' : 'blocked',
      notes:
        `Batch verify failed, left un-migrated for retry: ${verifyReport.failureSummary || 'see verify phase log'}. ` +
        `Original note: ${r.notes || '(none)'}`,
    }))

phase('Record')
const recordSummary = await agent(
  `In the repo at the current working directory, apply this batch's outcome to the manifest.\n\n` +
  `STEP 1 - write EXACTLY this JSON array, verbatim, to ${UPDATE_SCRATCH_PATH} with your file-write tool:\n\n` +
  `${JSON.stringify(finalResults, null, MANIFEST_INDENT_SPACES)}\n\n` +
  `STEP 2 - write this Python to ${RECORD_SCRIPT_PATH} with your file-write tool, verbatim:\n\n` +
  `import io, json\n` +
  `\n` +
  `manifest = json.load(io.open(${JSON.stringify(MANIFEST_PATH)}, encoding='utf-8'))\n` +
  `updates = {x['id']: x for x in json.load(io.open(${JSON.stringify(UPDATE_SCRATCH_PATH)}, encoding='utf-8'))}\n` +
  `\n` +
  `applied = 0\n` +
  `for problem in manifest['problems']:\n` +
  `    update = updates.get(problem['id'])\n` +
  `    if update is None:\n` +
  `        continue\n` +
  `    # 'status' here is the MIGRATION outcome, not the manifest's own done/pending vocabulary.\n` +
  `    for key, value in update.items():\n` +
  `        if key not in ('id', 'status') and value is not None:\n` +
  `            problem[key] = value\n` +
  `    if update['status'] in ('migrated', 'already'):\n` +
  `        problem['architectureTier'] = 'section-17'\n` +
  `    applied += 1\n` +
  `\n` +
  `json.dump(manifest, io.open(${JSON.stringify(MANIFEST_PATH)}, 'w', encoding='utf-8'), indent=2, ensure_ascii=False)\n` +
  `print('updated', applied, 'entries')\n\n` +
  `STEP 3 - run it: python3 ${RECORD_SCRIPT_PATH}\n\n` +
  `A blocked entry keeps whatever architectureTier it had, so the next run retries it; migrated and ` +
  `already-migrated entries become "section-17" and are never selected again. Null fields are skipped so a ` +
  `blocked result never wipes an existing path. Then delete both ${UPDATE_SCRATCH_PATH} and ` +
  `${RECORD_SCRIPT_PATH}. Return the "updated N entries" line.`,
  { phase: 'Record' },
)

log(recordSummary)

return {
  done: false,
  batchRequested: selection.batch.length,
  migrated: finalResults.filter((r) => r.status === 'migrated').length,
  alreadyMigrated: finalResults.filter((r) => r.status === 'already').length,
  blocked: finalResults.filter((r) => r.status === 'blocked').length,
  remainingBeforeThisBatch: selection.remainingBeforeThisBatch,
  verify: verifyReport,
  recordSummary,
}
