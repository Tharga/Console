## Session Continuity

### Starting a session
1. Run `git status` to check for uncommitted changes
   - If uncommitted changes exist, alert me immediately and stop
   - Do not proceed until I have confirmed how to handle them (commit, stash, or discard)
2. Check if `.claude/mission.md` exists and read it.
   - Follow **all** file references listed under "External References" — read each one (shared-instructions, backlog, incoming requests, etc.)
   - If a path uses `$DOC_ROOT`, resolve it via the environment variable defined in `~/.claude/settings.json`.
3. Check if `plan/` exists in the project root.
   - If `plan/plan.md` exists, summarize what has been done and what the next step is.
   - If `plan/feature.md` exists, read the current feature scope.
   - If neither exists, ask me how I would like to proceed.

### During a session
After completing each step in the plan:
- Mark it as `[x]` done in `plan/plan.md`
- Add a brief note about what was done and any important decisions made
- Mark the next step as `[~]` in progress

### Ending a session
- Update `plan/plan.md` with the current status of all steps
- Add a "Last session" note summarizing what was completed and what comes next
- Note any README.md changes that will be needed when the feature is complete

## Testing Rules
- Run relevant tests after completing each step before marking it done
- If tests fail, fix the issue before moving on — do not proceed with a failing test
- Run the full test suite before any git commit
- If no tests exist for the code being changed, write them first before implementing

## Build & Test
```bash
dotnet build -c Release
dotnet test -c Release
```

## Coding Guidelines
- Write tests for every new function
- Extract shared or repeated strings into named constants
- Suggest bumping of minor version when compatibility is broken
- Prefer functional programming patterns
- Prefer the command pattern for operations and side effects
- Use `init` over `set` wherever possible

### Feature and framework organization
- Place feature-specific code under `features/[name]`
- Place shared cross-functional code under `framework/[name]`
- Use `framework/` only for code that is reused across multiple features
- If code primarily belongs to one feature, keep it under that feature even if it has some reuse potential
- When in doubt, prefer `features/[name]` over `framework/[name]`
- Do not introduce shared abstractions in `framework/` prematurely; promote code there only when cross-feature reuse is clear

## Workflow Rules
- Before making changes, explain what you plan to do
- After completing a task, summarize what was changed
- If unsure about something, ask before proceeding
- **Cross-project guard:** If an instruction or change targets a different project than the one currently open, ask the user for confirmation before proceeding. Do not silently apply changes to other projects.

## Git Rules
- Never push to remote without explicit approval from me
- Never force push under any circumstances
- Create branch `feature/<feature-name>` at the start of each feature
- Commit at logical milestones (e.g. a component is complete and tested)
- Never commit failing tests
- Use conventional commits: `feat:`, `fix:`, `test:`, `docs:`
- Never merge to master/main — leave that for me to review and merge
- Default branch strategy: `master` is production, `develop` is integration. Feature branches branch from and merge to `develop`.
- When merging a completed feature back to the originating branch, use `--no-ff` (no fast-forward) to preserve the feature branch history as a merge commit

## Feature Workflow

Active feature tracking lives in `plan/` in the project root (committed with the feature branch).
Planned and completed features are stored externally in the **Plan directory** defined in `.claude/mission.md`.

### Planning features
- Future features are stored in the Plan directory under `planned/`
- Each file represents one feature, executed in order (e.g. `01-feature-name.md`, `02-feature-name.md`)
- When starting a new feature, check the Obsidian `planned/` directory first

### Starting a feature
When told to start a new feature:
1. Ask for the feature name and goal if not provided
2. Note the current branch as the originating branch for the feature
3. Create a new branch: `git checkout -b feature/<feature-name>`
4. Create `plan/feature.md` with goal, scope, acceptance criteria, and done condition
5. Create `plan/plan.md` with the steps to implement the feature
6. Confirm the plan before starting any code changes

### During implementation
- Update `plan/plan.md` continuously as changes are made
- Commit `plan/` together with code changes at logical milestones
- Run tests before each commit

### Completing implementation
When all planned steps are done:
- All tests pass
- Commit all changes
- Summarize what was done and ask the user to test and provide feedback
- Do NOT close the feature — wait for the user to confirm it is done

### Closing a feature (only when the user says it is done)
- All acceptance criteria in `plan/feature.md` are met
- All tests pass
- README.md has been updated to reflect the new feature
- Archive `plan/feature.md` to the Plan directory `done/<feature-name>.md`
- Delete the `plan/` directory from the project
- A final commit is made with message: `feat: <feature-name> complete`
- Merge to originating branch with `--no-ff` and delete feature branch only when the user explicitly asks

## Feature Requests (cross-project)

Cross-project requests are handled via `mission.md` — see the "Incoming requests" reference there for the central location.

- On startup, check `mission.md` for the requests location and show pending requests to the user
- Writing feature requests is **exempt from the cross-project guard**
- Never mark a request as done without user confirmation
- When a request is completed:
  1. Update its status to Done in the central requests file, add completion date and summary
  2. Add a follow-up entry under `## Uppföljning` at the top of the central requests file so the consuming project knows to update:
     ```
     - [ ] <Consuming project> ska uppdatera <package> till <version> — <kort beskrivning av vad som är nytt>
     ```
  3. The follow-up is checked off when the consuming project has updated and verified the new version

## Backlog Hygiene
- When a task from the backlog (in `mission.md` or linked external files) is completed, mark it as done or remove it
- When fixing a bug listed in the backlog, remove the bug entry after the fix is verified
- Keep the backlog current — do not leave completed items lingering
